using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication7
{
    static class PdbReader
    {
        public static List<string> ReadSourceFiles(string pdbPath)
        {
            var clsid = new Guid("3BFCEA48-620F-4B6B-81F7-B9AF75454C7D");
            var type = Type.GetTypeFromCLSID(clsid);
            var source = (DiaSource)Activator.CreateInstance(type);
            source.loadDataFromPdb(pdbPath);

            IDiaSession session;
            source.openSession(out session);

            IDiaEnumTables enumTables;
            session.getEnumTables(out enumTables);

            var result = new List<string>();

            foreach (IDiaTable diaEnumTable in enumTables)
            {
                var sourceFiles = diaEnumTable as IDiaEnumSourceFiles;
                if (sourceFiles == null)
                {
                    continue;
                }

                foreach (IDiaSourceFile sourceFile in sourceFiles)
                {
                    result.Add(sourceFile.fileName);
                }
            }

            return result;
        }
    }
}
