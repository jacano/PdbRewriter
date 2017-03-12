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

                if(indexOfDirSep == -1)
                {
                    break;
                }

                var folderName = path.Substring(indexOfDirSep + 1);
                if(folderName == nugetLib)
                {
                    break;
                }

                path = path.Substring(0, indexOfDirSep);

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
