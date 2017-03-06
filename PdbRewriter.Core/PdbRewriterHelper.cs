using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdbRewriter.Core
{
    public static class PdbRewriterHelper
    {
        public static void TryRewrite(string dllPath)
        {
            var nugetLib = "lib";
            var nugetSrc = "src";

            var t = -1;
            do
            {
                dllPath = dllPath.TrimEnd(Path.DirectorySeparatorChar);

                t = dllPath.LastIndexOf(Path.DirectorySeparatorChar);
                var tt = dllPath.Substring(0, 0);
            }
            while (t != -1);
            var libIndex = dllPath.LastIndexOf(nugetLib);
            var srcPath = Path.Combine(dllPath.Substring(0, libIndex), nugetSrc);

            var srcDirExists = Directory.Exists(srcPath);
            if (srcDirExists)
            {
                PdbHelper.RewritePdb(dllPath, srcPath);
            }
        }
    }
}
