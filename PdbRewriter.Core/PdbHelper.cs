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
        public static void RewritePdb(string dllPath, string srcPath, out string guid)
        {
            var currentGuid = string.Empty;
            Action<Guid> guidAction = (u) =>
            {
                currentGuid = u.ToString("N").ToUpperInvariant();
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

            var pdbPath = Path.ChangeExtension(dllPath, "pdb");
            var pdbBackupPath = Path.ChangeExtension(dllPath, "pdb.bak");
            var dllBackupPath = Path.ChangeExtension(dllPath, "dll.bak");

            File.Copy(dllPath, dllBackupPath, true);
            File.Copy(pdbPath, pdbBackupPath, true);

            File.Delete(dllPath);
            File.Delete(pdbPath);

            using (var fileStream = new FileStream(dllBackupPath, FileMode.Open))
            {
                using (var pdbStream = new FileStream(pdbBackupPath, FileMode.Open))
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
                        assembly.Write(dllPath, new WriterParameters()
                        {
                            SymbolWriterProvider = new PdbWriterProvider(),
                            WriteSymbols = true,
                            SourcePathRewriter = rewrite,
                        });
                    }
                }
            }

            guid = currentGuid;
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
