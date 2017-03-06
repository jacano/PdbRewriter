using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using PdbRewriter.Core;
using PdbRewriter.Task;

namespace PdbRewriter
{
    public class PdbRewriterTask : Microsoft.Build.Utilities.Task
    {
        public PdbRewriterTask()
        {
            PdbRewriterHelper.Logger = new TaskLogger(this);
        }

        public override bool Execute()
        { 
            return true;
        }
    }
}
