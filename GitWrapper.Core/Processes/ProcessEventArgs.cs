using System.Diagnostics;

namespace Chris82111.GitManager.GitWrapper.Core.Processes
{
    public class ProcessEventArgs(Process process) : EventArgs
    {
        public Process Process { get; } = process;
    }
}
