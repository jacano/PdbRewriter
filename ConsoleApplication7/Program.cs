﻿using Mono.Cecil;
using Mono.Cecil.Pdb;
using System;
using System.IO;

namespace ConsoleApplication7
{
    static class Program
    {
        static void Main(string[] args)
        {
            var fileInput = @"E:\dev\PdbRewriter\ConsoleApplication8\GoogleAnalyticsTracker.Core.4.2.7\lib\portable45\GoogleAnalyticsTracker.Core.dll";
            var fileOutput = @"E:\dev\PdbRewriter\ConsoleApplication8\GoogleAnalyticsTracker.Core.4.2.7\lib\portable45\GoogleAnalyticsTracker.Core1.dll";

            var searchString = @"D:\temp\e086b63\GoogleAnalyticsTracker.Core";
            var replaceString = @"test";

            Func<string, string> rewrite = (s) => s.Replace(searchString, replaceString);

            using (var fileStream = new FileStream(fileInput, FileMode.Open))
            {
                using (var pdbStream = new FileStream(Path.ChangeExtension(fileInput, "pdb"), FileMode.Open))
                {
                    using (var assembly = AssemblyDefinition.ReadAssembly(fileStream,
                        new ReaderParameters()
                        {
                            SymbolReaderProvider = new PdbReaderProvider(),
                            SymbolStream = pdbStream,
                            ReadSymbols = true,
                        }
                    ))
                    {
                        assembly.Write(fileOutput, new WriterParameters()
                        {
                            SymbolWriterProvider = new PdbWriterProvider(),
                            WriteSymbols = true,
                        });
                    }
                }
            }
        }

        /*static unsafe void Main(string[] args)
        {
            var fileInput = @"C:\Users\jacano\Desktop\GoogleAnalyticsTracker.Core.pdb";
            var fileOutput = @"C:\Users\jacano\Desktop\GoogleAnalyticsTracker.Core1.pdb";

            var searchString = @"D:\temp\e086b63\GoogleAnalyticsTracker.Core";
            var replaceString = @"%TVV%";

            var searchStringLower = searchString.ToLowerInvariant();
            var replaceStringLower = replaceString.ToLowerInvariant();

            Func<string, string> rewrite = (s) => s.Replace(searchString, replaceString);
            Func<string, string> rewriteLower = (s) => s.Replace(searchStringLower, replaceStringLower);

            var sourceFiles = PdbReader.ReadSourceFiles(fileInput);

            var bytes = File.ReadAllBytes(fileInput);
            var byteLenght = bytes.LongLength;
            fixed (byte* bytesPointer = bytes)
            {
                for (var i = 0; i < sourceFiles.Count; i++)
                {
                    var name = sourceFiles[i];
                    var lowerName = name.ToLowerInvariant();

                    RewriteSourceFiles(bytesPointer, byteLenght, name, rewrite);
                    RewriteSourceFiles(bytesPointer, byteLenght, lowerName, rewriteLower);
                }
            }

            File.WriteAllBytes(fileOutput, bytes);
        }

        private static unsafe void RewriteSourceFiles(byte* bytesPointer, long byteLenght, string name, Func<string, string> rewrite)
        {
            var nameBytes = Encoding.ASCII.GetBytes(name);
            var nameBytesLength = nameBytes.LongLength;

            var rewrittenName = rewrite(name);

            var rewrittenNameBytes = Encoding.ASCII.GetBytes(rewrittenName);
            var rewrittenNameBytesLength = rewrittenNameBytes.LongLength;

            if (rewrittenNameBytesLength > nameBytesLength)
            {
                throw new Exception("Impossible to rewrite");
            }

            fixed (byte* nameBytesPointer = nameBytes)
            {
                fixed (byte* rewrittenNameBytesPointer = rewrittenNameBytes)
                {
                    var offset = 0L;
                    while (offset >= 0)
                    {
                        offset = IndexOf(offset, bytesPointer, byteLenght, nameBytesPointer, nameBytesLength);
                        if (offset < 0)
                        {
                            break;
                        }

                        offset += Override(offset, bytesPointer, rewrittenNameBytesPointer, rewrittenNameBytesLength, nameBytesLength);
                    }
                }
            }
        }

        private static unsafe long IndexOf(long startOffset, byte* haystack, long haystackLength, byte* needle, long needleLength)
        {
            var hNext = haystack + startOffset;
            var hEnd = haystack + haystackLength + 1 - needleLength;
            var nEnd = needle + needleLength;

            for (; hNext < hEnd; hNext++)
            {
                var hInc = hNext;
                var nInc = needle;
                for (; *nInc == *hInc; hInc++)
                {
                    if (++nInc == nEnd)
                    {
                        return hNext - haystack;
                    }
                }
            }

            return -1;
        }

        private static unsafe long Override(long startOffset, byte* bytesPointer, byte* rewrittenBytes, long rewrittenBytesLength, long nameBytesLength)
        {
            var rInc = rewrittenBytes;
            var bInc = bytesPointer + startOffset;

            var rEnd = bInc + rewrittenBytesLength;
            var nEnd = bInc + nameBytesLength;

            for (; bInc < rEnd; bInc++, rInc++)
            {
                *bInc = *rInc;
            }

            for (; bInc < nEnd; bInc++)
            {
                *bInc = 0;
            }

            return nameBytesLength;
        }*/
    }
}
