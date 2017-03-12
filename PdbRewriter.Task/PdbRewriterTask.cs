using Microsoft.Build.Framework;
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
            if (this.Files != null)
            {
                foreach (var item in Files)
                {
                    var dllPath = item.ToString();
                    PdbRewriterHelper.TryRewrite(dllPath);
                }
            }

            return true;
        }
    }
}
