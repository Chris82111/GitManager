using System.Diagnostics;

namespace Chris82111.GitManager.GitWrapper.Core.Processes
{
    /// <summary>
    /// Provides data for an event associated with a process
    /// </summary>
    public class ProcessEventArgs(Process process) : EventArgs
    {
        /// <summary>
        /// Gets the process associated with the event
        /// </summary>
        public Process Process { get; } = process;
    }
}
