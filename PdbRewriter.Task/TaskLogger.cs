using PdbRewriter.Core;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace PdbRewriter.Task
{
    public class TaskLogger : Core.ILogger
    {
        private Microsoft.Build.Utilities.Task task;

        public TaskLogger(Microsoft.Build.Utilities.Task task)
        {
            this.task = task;
        }

        public void Log(string msg)
        {
            this.task.Log.LogMessage(MessageImportance.High, msg);
        }
    }
}
