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
        static void Main(string[] args)
        {
            var file = @"E:\dev\pdbrewriter\ConsoleApplication7\ConsoleApplication7.pdb";

            using (var pdb = new PdbFile(file))
            {
                var sourceFiles = pdb.GetFiles();

                pdb.Rewrite((s) => s.Replace(@"e:\dev\", ""));
            }
        }
    }
}
