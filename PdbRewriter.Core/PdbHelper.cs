using Mono.Cecil;
using Mono.Cecil.Pdb;
using System;
using System.IO;

namespace PdbRewriter.Core
{
    public static class PdbHelper
    {
        public static void Test()
        {
            var fileInput = @"E:\dev\PdbRewriter\ConsoleApplication8\GoogleAnalyticsTracker.Core.4.2.7\lib\portable45\GoogleAnalyticsTracker.Core.dll";
            var fileOutput = @"E:\dev\PdbRewriter\ConsoleApplication8\GoogleAnalyticsTracker.Core.4.2.7\lib\portable45\GoogleAnalyticsTracker.Core2.dll";

            var searchString = @"D:\temp\e086b63\GoogleAnalyticsTracker.Core";
            var replaceString = @"test";

            var id = string.Empty;

            Action<Guid> guid = (u) =>
            {
                id = u.ToString("N").ToUpperInvariant();
            };

            Func<string, string> rewrite = (s) =>
            {
                return s.Replace(searchString, replaceString);
            };

            using (var fileStream = new FileStream(fileInput, FileMode.Open))
            {
                using (var pdbStream = new FileStream(Path.ChangeExtension(fileInput, "pdb"), FileMode.Open))
                {
                    using (var assembly = AssemblyDefinition.ReadAssembly(fileStream,
                        new ReaderParameters()
                        {
                            SymbolReaderProvider = new PdbReaderProvider(),
                            SymbolStream = pdbStream,
                            ReadSymbols = true,
                            GuidProvider = guid,
                        }
                    ))
                    {
                        assembly.Write(fileOutput, new WriterParameters()
                        {
                            SymbolWriterProvider = new PdbWriterProvider(),
                            WriteSymbols = true,
                            SourcePathRewriter = rewrite,
                        });
                    }
                }
            }
        }
    }
}
