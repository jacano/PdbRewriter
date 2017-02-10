using GoogleAnalyticsTracker.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication8
{
    class Program
    {
        static void Main(string[] args)
        {
            EnumExtensions.IsNullableEnum(typeof(int));

            /*var BuildTimeFilePath = @"D:\temp\e086b63\GoogleAnalyticsTracker.Core\AnalyticsSession.cs";

            var t = Environment.GetEnvironmentVariable("_NT_SOURCE_PATH").Split(';');

            var m_parsedSourcePath = new List<string>();
            foreach (var path in t)
            {
                var normalizedPath = path.Trim();
                if (normalizedPath.EndsWith(@"\"))
                    normalizedPath = normalizedPath.Substring(0, normalizedPath.Length - 1);
                if (Directory.Exists(normalizedPath))
                {
                    m_parsedSourcePath.Add(normalizedPath);
                }
            }

            var locations = m_parsedSourcePath;

            var curIdx = 0;
            for (;;)
            {
                var sepIdx = BuildTimeFilePath.IndexOf('\\', curIdx);
                if (sepIdx < 0)
                    break;
                curIdx = sepIdx + 1;
                var tail = BuildTimeFilePath.Substring(sepIdx);

                foreach (string location in locations)
                {
                    var probe = location + tail;
                    Console.WriteLine("Probing {0}", probe);
                    if (File.Exists(probe))
                    {
                    }
                }
            }*/

        }
    }
}
