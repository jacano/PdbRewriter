using Mono.Cecil;
using Mono.Cecil.Mono.Cecil;
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
            var oldSignature = default(Signature);
            Action<Signature> oldSigAction = (u) =>
            {
                oldSignature = u;
            };

            var newSignature = default(Signature);
            Action<Signature> newSigAction = (u) =>
            {
                newSignature = u;
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
                            SignatureProvider = oldSigAction,
                        }
                    ))
                    {
                        assembly.Write(dllOutputPath, new WriterParameters()
                        {
                            SymbolWriterProvider = new PdbWriterProvider(),
                            WriteSymbols = true,
                            SourcePathRewriter = rewrite,
                            SignatureProvider = newSigAction,
                        });
                    }
                }
            }

            var pdbBackupPath = Path.ChangeExtension(pdbPath, "pdb.bak");
            File.Move(pdbPath, pdbBackupPath);

            var pdbOutputPath = Path.ChangeExtension(dllOutputPath, "pdb");
            File.Move(pdbOutputPath, pdbPath);

            ReplaceSignatureInPdb(pdbPath, oldSignature, newSignature);

            CopyPdbToSymbolCache(pdbPath, oldSignature);
        }

        static unsafe void ReplaceSignatureInPdb(string pdbPath, Signature oldSignature, Signature newSignature)
        {
            var oldGuidBytes = oldSignature.Guid.ToByteArray();
            var oldGuidLength = oldGuidBytes.LongLength;
            //var oldAgeBytes = BitConverter.GetBytes(oldSignature.Age);
            //var oldSignatureBytes = ArrayHelper.Concat(oldGuidBytes, oldAgeBytes);
            //var oldSignatureBytesLength = oldSignatureBytes.LongLength;

            var newGuidBytes = newSignature.Guid.ToByteArray();
            var newGuidLength = newGuidBytes.LongLength;
            //var newAgeBytes = BitConverter.GetBytes(newSignature.Age);
            //var newSignatureBytes = ArrayHelper.Concat(newGuidBytes, newAgeBytes);
            //var newSignatureBytesLength = newSignatureBytes.LongLength;

            var bytes = File.ReadAllBytes(pdbPath);
            var byteLenght = bytes.LongLength;

            fixed (byte* bytesPointer = bytes)
            {
                fixed (byte* newSigPointer = newGuidBytes)
                {
                    fixed (byte* oldSignPointer = oldGuidBytes)
                    {
                        var signatureIndex = -1L;
                        var offset = 0L;
                        do
                        {
                            signatureIndex = UnsafeHelper.IndexOf(offset, bytesPointer, byteLenght, newSigPointer, newGuidLength);
                            if (signatureIndex != -1)
                            {
                                UnsafeHelper.Override(signatureIndex, bytesPointer, oldSignPointer, oldGuidLength);
                                offset += signatureIndex;
                            }
                        }
                        while (signatureIndex != -1);
                    }
                }
            }

            File.WriteAllBytes(pdbPath, bytes);
        }

        static void CopyPdbToSymbolCache(string pdbPath, Signature oldSignature)
        {
            var ageString = oldSignature.Age.ToString("X");
            var guidString = oldSignature.Guid.ToString("N").ToUpperInvariant();

            var symbolCacheDir = SymbolHelper.GetSymbolCacheDir();
            if(!string.IsNullOrEmpty(symbolCacheDir))
            {
                var pdbFilename = Path.GetFileName(pdbPath);
                var pdbFolder = Path.Combine(symbolCacheDir, pdbFilename);
                if (!Directory.Exists(pdbFolder))
                {
                    Directory.CreateDirectory(pdbFolder);
                }

                var pdbFolderSignature = Path.Combine(pdbFolder, guidString + ageString);
                if (!Directory.Exists(pdbFolderSignature))
                {
                    Directory.CreateDirectory(pdbFolderSignature);
                }

                var finalPdbPath = Path.Combine(pdbFolderSignature, pdbFilename);
                File.Copy(pdbPath, finalPdbPath, true);
            }
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
