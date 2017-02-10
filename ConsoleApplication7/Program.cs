using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication7
{
    static class Program
    {
        static unsafe void Main(string[] args)
        {
            var fileInput = @"C:\Users\jacano\Desktop\ConsoleApp.pdb";
            var sourceFiles = PdbReader.ReadSourceFiles(fileInput);

            var replaceString = @"D:\dev\";
            var replaceStringLower = replaceString.ToLowerInvariant();

            Func<string, string> rewrite = (s) => s.Replace(replaceString, "");
            Func<string, string> rewriteLower = (s) => s.Replace(replaceStringLower, "");

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

            var fileOutput = @"C:\Users\jacano\Desktop\ConsoleApp1.pdb";
            File.WriteAllBytes(fileOutput, bytes);
        }

        private static unsafe void RewriteSourceFiles(byte* bytesPointer, long byteLenght, string name, Func<string, string> rewrite)
        {
            var nameBytes = Encoding.ASCII.GetBytes(name);
            var nameBytesLength = nameBytes.LongLength;
            fixed (byte* nameBytesPointer = nameBytes)
            {
                var rewrittenName = rewrite(name);

                var rewrittenNameBytes = Encoding.ASCII.GetBytes(rewrittenName);
                var rewrittenNameBytesLength = rewrittenNameBytes.LongLength;

                if (rewrittenNameBytesLength > nameBytesLength)
                {
                    throw new Exception("Impossible to rewrite");
                }

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
    }
}
