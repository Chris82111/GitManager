using System.Diagnostics;

namespace Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables
{
    public class ProcessEnvironmentVariable : EnvironmentVariableBase
    {
        ProcessStartInfo _processStartInfo;

        public ProcessEnvironmentVariable(ProcessStartInfo processStartInfo)
        {
            _processStartInfo = processStartInfo;
        }

        protected override string? GetValue(string variable)
        {
            return _processStartInfo.Environment.TryGetValue(variable, out string? value) ? value : null;
        }

        protected override void SetValue(string variable, string value)
        {
            _processStartInfo.Environment[variable] = value;
        }

        protected override void RemoveValue(string variable)
        {
            _processStartInfo.Environment.Remove(variable);
        }
    }
}
