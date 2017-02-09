using ConsoleApplication7.gitlink;
using GitLink.Pdb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication7
{
    class Program
    {
        // srctool WaveEngine.Common.pdb -x -r

        static void Main(string[] args)
        {
            var file = @"D:\dev\Engine\Deploy\NugetPackages\WaveEngine.Common.Symbols\lib\net45\WaveEngine.Common.pdb";

            using (var pdb = new PdbFile(file))
            {
                var sourceFiles = pdb.GetFilesAndChecksums();
            }

            var t = PdbReader.ReadSourceFiles(file);
        }
    }
}
