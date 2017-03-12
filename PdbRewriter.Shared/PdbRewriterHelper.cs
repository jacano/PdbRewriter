using System.IO;

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

            var found = TryFindSrcDirInDllPath(dllPath, out var srcPath);
            if (found)
            {
                var srcDirExists = Directory.Exists(srcPath);
                if (srcDirExists)
                {
                    Logger.Log($"Nuget reference with symbols found at: {srcPath}");

                    PdbHelper.RewritePdb(dllPath, srcPath);
                }
            }
        }

        private static bool TryFindSrcDirInDllPath(string dllPath, out string srcPath)
        {
            var found = false;

            var path = dllPath;

            while (!found)
            {
                path = path.TrimEnd(Path.DirectorySeparatorChar);
                var indexOfDirSep = path.LastIndexOf(Path.DirectorySeparatorChar);
                if (indexOfDirSep == -1)
                {
                    break;
                }

                var folderName = path.Substring(indexOfDirSep + 1);
                if (folderName == nugetLib)
                {
                    found = true;
                }

                path = path.Substring(0, indexOfDirSep);
            }

            srcPath = Path.Combine(path, nugetSrc);

            return found;
        }
    }
}
