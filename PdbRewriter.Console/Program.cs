using PdbRewriter.Core;
using System;
using System.IO;

namespace ConsoleApplication7
{
    static class Program
    {
        static void Main(string[] args)
        {
            var t = @"D:\dev\PdbRewriter\ConsoleApplication8\GoogleAnalyticsTracker.Core.4.2.7\lib\portable45\GoogleAnalyticsTracker.Core.dll";

            PdbRewriterHelper.TryRewrite(t);
        }
    }
}
