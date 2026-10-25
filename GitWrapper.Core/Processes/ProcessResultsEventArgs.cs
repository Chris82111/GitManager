namespace Chris82111.GitManager.GitWrapper.Core.Processes
{
    /// <summary>
    /// Provides data for an event containing the results of a process
    /// </summary>
    public class ProcessResultsEventArgs(ProcessResultsDto process) : EventArgs
    {
        /// <summary>
        /// Process result information
        /// </summary>
        public ProcessResultsDto ProcessResults { get; } = process;
    }
}
