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
		
		public ITaskItem[] Files { get; set; }

        public override bool Execute()
        {
            foreach (var item in Files)
            {
                PdbRewriterHelper.Logger.Log(item.ToString());
            }
            return true;
        }
    }
}
