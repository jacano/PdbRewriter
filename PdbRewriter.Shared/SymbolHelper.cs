using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace PdbRewriter.Core
{
    public static class SymbolHelper
    {
        private static string[] vsVersions = new[] { "2017", "2015" };

        public static string GetFirstSymbolCacheDir()
        {
            var myDocuments = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            foreach (var vsVersion in vsVersions)
            {
                var vsSettings = $@"{myDocuments}\Visual Studio {vsVersion}\Settings\CurrentSettings.vssettings";
                if (File.Exists(vsSettings))
                {
                    var vsSettingContent = File.ReadAllText(vsSettings);

                    var symbolDirName = "SymbolCacheDir";
                    var xpath = $"//UserSettings/Category[@name='Debugger']/PropertyValue[@name='{symbolDirName}']";

                    var doc = XDocument.Parse(vsSettingContent);
                    var node = doc.XPathSelectElement(xpath);
                    if (node != null)
                    {
                        var symbolDirPath = node.FirstNode.ToString();

                        return symbolDirPath;
                    }
                }
            }

            return string.Empty;
        }
    }
}
