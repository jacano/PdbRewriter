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
        internal static List<string> GetFilesAndChecksums(this PdbFile pdbFile)
        {
            // const int LastInterestingByte = 47;
            const string FileIndicator = "/src/files/";

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
    }
}
