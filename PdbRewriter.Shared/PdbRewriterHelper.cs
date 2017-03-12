using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdbRewriter.Core
{
    public static class PdbRewriterHelper
    {
        public static ILogger Logger;

        private const string nugetLib = "lib";
        private const string nugetSrc = "src";

        public static void TryRewrite(string dllPath)
        {
            Logger.Log($"Trying to rewrite: {dllPath}");

            var path = dllPath;

            var indexOfDirSep = -1;
            do
            {
                path = path.TrimEnd(Path.DirectorySeparatorChar);

                indexOfDirSep = path.LastIndexOf(Path.DirectorySeparatorChar);

                var folderName = path.Substring(indexOfDirSep + 1);

                path = path.Substring(0, indexOfDirSep);

                if(folderName == nugetLib)
                {
                    break;
                }
            }
            while (indexOfDirSep != -1);

            var srcPath = Path.Combine(path, nugetSrc);

            var srcDirExists = Directory.Exists(srcPath);
            if (srcDirExists)
            {
                PdbHelper.RewritePdb(dllPath, srcPath);
            }
        }
    }
}
