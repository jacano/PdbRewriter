using PdbRewriter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdbRewriter.Console
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string msg)
        {
            System.Console.WriteLine(msg);
        }
    }
}
