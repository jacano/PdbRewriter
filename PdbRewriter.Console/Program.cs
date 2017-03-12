using PdbRewriter.Console;
using PdbRewriter.Core;
using System;
using System.IO;

namespace ConsoleApplication7
{
    static class Program
    {
        static void Main(string[] args)
        {
            PdbRewriterHelper.Logger = new ConsoleLogger();

            var t2 = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\Microsoft.CSharp.dll";
            var t = @"E:\dev\PdbRewriter\ConsoleApplication8\GoogleAnalyticsTracker.Core.4.2.7\lib\portable45\GoogleAnalyticsTracker.Core.dll";

            PdbRewriterHelper.TryRewrite(t2);
        }
    }
}
