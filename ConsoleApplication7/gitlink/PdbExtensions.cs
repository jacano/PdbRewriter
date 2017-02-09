using GitLink.Pdb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication7.gitlink
{
    internal static class PdbExtensions
    {
        const string FileIndicator = "/src/files/";

        internal static List<string> GetFiles(this PdbFile pdbFile)
        {
            var results = new List<string>();
            foreach (var value in pdbFile.Info.PdbNames)
            {
                if (!value.Name.StartsWith(FileIndicator))
                {
                    continue;
                }

                var name = value.Name.Substring(FileIndicator.Length);

                results.Add(name);
            }

            return results;
        }

        internal static void Rewrite(this PdbFile pdbFile, Func<string,string> rewrite)
        {
            var fileIndicatorLenght = FileIndicator.Length;

            using (var fs = File.Open(pdbFile.Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (var bw = new BinaryWriter(fs, Encoding.UTF8, true))
                {
                    foreach (var value in pdbFile.Info.PdbNames)
                    {
                        if (!value.Name.StartsWith(FileIndicator))
                        {
                            continue;
                        }

                        var name = value.Name.Substring(fileIndicatorLenght);
                        var nameLength = name.Length;

                        var rewrittenName = rewrite(name);
                        var rewrittenNameLength = rewrittenName.Length;

                        if (rewrittenNameLength > nameLength)
                        {
                            throw new Exception("Impossible to rewrite");
                        }

                        var rewrittenBytes = Encoding.ASCII.GetBytes(rewrittenName);
                        var newBytesToOverride = new byte[nameLength];
                        for (var i = 0; i < rewrittenNameLength; i++)
                        {
                            newBytesToOverride[i] = rewrittenBytes[i];
                        }

                        var offset = (int)value.Stream + fileIndicatorLenght;
                        bw.Seek(offset, SeekOrigin.Begin);

                        bw.Write(newBytesToOverride);
                    }
                }
            }
        }
    }
}
