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

        internal static void Rewrite(this PdbFile pdbFile, string rewrite)
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

                        var offset = (int)value.Stream + fileIndicatorLenght;
                        var name = value.Name.Substring(fileIndicatorLenght);

                        var newBytes = new byte[name.Length];

                        bw.Seek(offset, SeekOrigin.Begin);
                        bw.Write(newBytes);
                    }
                }
            }
        }
    }
}
