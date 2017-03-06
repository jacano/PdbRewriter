using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdbRewriter.Core
{
    public static class SymbolHelper
    {
        public static string GetSymbolCacheDir()
        {
            var version = "14.0";
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\VisualStudio\" + version + @"\Debugger"))
            {
                if (key != null)
                {
                    var o = key.GetValue("SymbolCacheDir");
                    if (o != null)
                    {
                        var symbolCacheDir = o as string;

                        return symbolCacheDir;
                    }
                }
            }

            return string.Empty;
        }
    }
}
