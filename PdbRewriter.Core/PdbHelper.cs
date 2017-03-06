using Microsoft.Win32;
using Mono.Cecil;
using Mono.Cecil.Pdb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PdbRewriter.Core
{
    public static class PdbHelper
    {
        public static void RewritePdb(string dllPath, string srcPath)
        {
            var currentGuid = Guid.Empty;
            Action<Guid> guidAction = (u) =>
            {
                currentGuid = u;
            };

            var loweredFilesInSrc = Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories).Select(p => p.ToLowerInvariant()).ToList();

            var lowerSrcPath = srcPath.ToLowerInvariant();
            Func<string, string> rewrite = (s) =>
            {
                var lowerPdbFile = s.ToLowerInvariant();

                var index = Process(loweredFilesInSrc, lowerPdbFile);
                var invalidIndex = index == 0 || index == lowerPdbFile.Length - 1;
                if (!invalidIndex)
                {
                    var commonPart = lowerPdbFile.Substring(index);
                    var finalPdb = lowerSrcPath + commonPart;

                    return finalPdb;
                }
                else
                {
                    return lowerPdbFile;
                }
            };

            var filename = Path.GetFileName(dllPath);
            var pdbPath = Path.ChangeExtension(dllPath, "pdb");

            var tmpPath = Path.GetTempPath();

            var dllOutputPath = Path.Combine(tmpPath, filename);

            using (var fileStream = new FileStream(dllPath, FileMode.Open))
            {
                using (var pdbStream = new FileStream(pdbPath, FileMode.Open))
                {
                    using (var assembly = AssemblyDefinition.ReadAssembly(fileStream,
                        new ReaderParameters()
                        {
                            SymbolReaderProvider = new PdbReaderProvider(),
                            SymbolStream = pdbStream,
                            ReadSymbols = true,
                            GuidProvider = guidAction,
                        }
                    ))
                    {
                        assembly.Write(dllOutputPath, new WriterParameters()
                        {
                            SymbolWriterProvider = new PdbWriterProvider(),
                            WriteSymbols = true,
                            SourcePathRewriter = rewrite,
                        });
                    }
                }
            }

            var pdbBackupPath = Path.ChangeExtension(pdbPath, "pdb.bak");
            File.Move(pdbPath, pdbBackupPath);

            var pdbOutputPath = Path.ChangeExtension(dllOutputPath, "pdb");
            File.Move(pdbOutputPath, pdbPath);

            var guidString = currentGuid.ToString("N").ToLowerInvariant();

            CopyPdbToSymbolCache(pdbPath, guidString);
        }

        static void CopyPdbToSymbolCache(string pdbPath, string currentGuid)
        {
            var symbolCacheDir = GetSymbolCacheDir();
            if(!string.IsNullOrEmpty(symbolCacheDir))
            {
                var pdbFilename = Path.GetFileName(pdbPath);
                var pdbFolder = Path.Combine(symbolCacheDir, pdbFilename);
                if (!Directory.Exists(pdbFolder))
                {
                    Directory.CreateDirectory(pdbFolder);
                }

                var pdbFolderGuid = Path.Combine(pdbFolder, currentGuid);
                if (!Directory.Exists(pdbFolderGuid))
                {
                    Directory.CreateDirectory(pdbFolderGuid);
                }

                var finalPdbPath = Path.Combine(pdbFolderGuid, pdbFilename);
                File.Copy(pdbPath, finalPdbPath, true);
            }
        }

        private static string GetSymbolCacheDir()
        {
            var version = "14.0";
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\VisualStudio\" + version + @"\Debugger"))
            {
                if (key != null)
                {
                    var o = key.GetValue("SymbolCacheDir");
                    if (o != null)
                    {
                        var symbolCacheDir = o as string;

                        return symbolCacheDir;
                    }
                }
            }

            return string.Empty;
        }

        static int Process(List<string> srcFiles, string pdbFile)
        {
            var bestIndex = pdbFile.Length - 1;
            foreach (var file in srcFiles)
            {
                var length = Math.Min(pdbFile.Length, file.Length);

                var pdbIndex = pdbFile.Length - 1;
                var fileIndex = file.Length - 1;

                var i = length - 1;
                while (i >= 0)
                {
                    var c1 = file[fileIndex];
                    var c2 = pdbFile[pdbIndex];

                    if (c1 != c2)
                    {
                        break;
                    }

                    bestIndex = Math.Min(bestIndex, pdbIndex);
                    fileIndex--;
                    pdbIndex--;
                    i--;
                }
            }

            return bestIndex;
        }
    }
}
