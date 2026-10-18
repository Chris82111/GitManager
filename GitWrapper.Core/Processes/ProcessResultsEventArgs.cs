namespace Chris82111.GitManager.GitWrapper.Core.Processes
{
    public class ProcessResultsEventArgs(ProcessResultsDto process) : EventArgs
    {
        public ProcessResultsDto ProcessResults { get; } = process;
    }
}
