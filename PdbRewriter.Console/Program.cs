using PdbRewriter.Core;
using System;
using System.IO;

namespace ConsoleApplication7
{
    static class Program
    {
        static void Main(string[] args)
        {
            var fileInput = @"E:\dev\PdbRewriter\ConsoleApplication8\GoogleAnalyticsTracker.Core.4.2.7\lib\portable45\GoogleAnalyticsTracker.Core.dll";
            var fileOutput = @"E:\dev\PdbRewriter\ConsoleApplication8\GoogleAnalyticsTracker.Core.4.2.7\lib\portable45\GoogleAnalyticsTracker.Core2.dll";

            var srcPath = string.Empty;
            var guid = string.Empty;

            PdbHelper.RewritePdb(fileInput, fileOutput, srcPath, out guid);
        }
    }
}
