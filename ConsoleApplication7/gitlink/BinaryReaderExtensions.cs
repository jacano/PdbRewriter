using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication7.gitlink
{
    internal static class BinaryReaderExtensions
    {
        internal static string ReadCString(this BinaryReader binaryReader)
        {
            var list = new List<byte>();

            var b = binaryReader.ReadByte();
            while (b != '\0')
            {
                list.Add(b);
                b = binaryReader.ReadByte();
            }

            return Encoding.UTF8.GetString(list.ToArray());
        }
    }
}
