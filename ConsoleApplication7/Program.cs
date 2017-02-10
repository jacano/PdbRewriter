using ConsoleApplication7.gitlink;
using GitLink.Pdb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication7
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = @"C:\Users\jacano\Desktop\ConsoleApp.pdb";

            var sourceFiles = PdbReader.ReadSourceFiles(file);

            Func<string, string> rewrite = (s) => s.Replace(@"d:\dev\", "");

            using (var fs = File.Open(file, FileMode.Open))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    foreach (var pdbName in sourceFiles)
                    {
                        var name = pdbName;
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

                        bw.
                        var offset = (int)pdbName.Stream;
                        bw.Seek(offset, SeekOrigin.Begin);

                        bw.Write(newBytesToOverride);
                    }
                }
            }
        }
    }
}
