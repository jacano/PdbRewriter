using Mono.Cecil;
using Mono.Cecil.Pdb;
using System;
using System.IO;

namespace PdbRewriter.Core
{
    public static class PdbHelper
    {
        public static void RewritePdb(string pdbPath, string pdbOutPath, string srcPath, out string guid)
        {
            var searchString = @"D:\temp\e086b63\GoogleAnalyticsTracker.Core";

            var currentGuid = string.Empty;
            Action<Guid> guidAction = (u) =>
            {
                currentGuid = u.ToString("N").ToUpperInvariant();
            };

            Func<string, string> rewrite = (s) =>
            {
                return s.Replace(searchString, srcPath);
            };

            using (var fileStream = new FileStream(pdbPath, FileMode.Open))
            {
                using (var pdbStream = new FileStream(Path.ChangeExtension(pdbPath, "pdb"), FileMode.Open))
                {
                    using (var assembly = AssemblyDefinition.ReadAssembly(fileStream,
                        new ReaderParameters()
                        {
                            SymbolReaderProvider = new PdbReaderProvider(),
                            SymbolStream = pdbStream,
                            ReadSymbols = true,
                            GuidProvider = guidAction,
                        }
                    ))
                    {
                        assembly.Write(pdbOutPath, new WriterParameters()
                        {
                            SymbolWriterProvider = new PdbWriterProvider(),
                            WriteSymbols = true,
                            SourcePathRewriter = rewrite,
                        });
                    }
                }
            }

            guid = currentGuid;
        }
    }
}
