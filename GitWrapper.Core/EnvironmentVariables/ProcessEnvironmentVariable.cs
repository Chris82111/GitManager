using System.Diagnostics;

namespace Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables
{
    /// <summary>
    /// This implementation allows you to modify the environment variables of a <see cref="ProcessStartInfo"/>
    /// </summary>
    public class ProcessEnvironmentVariable : EnvironmentVariableBase
    {
        ProcessStartInfo _processStartInfo;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="processStartInfo">The process start information whose environment variables are managed.</param>
        public ProcessEnvironmentVariable(ProcessStartInfo processStartInfo)
        {
            _processStartInfo = processStartInfo;
        }

        /// <inheritdoc/>
        protected override string? GetValue(string variable)
        {
            return _processStartInfo.Environment.TryGetValue(variable, out string? value) ? value : null;
        }

        /// <inheritdoc/>
        protected override void SetValue(string variable, string value)
        {
            _processStartInfo.Environment[variable] = value;
        }

        /// <inheritdoc/>
        protected override void RemoveValue(string variable)
        {
            _processStartInfo.Environment.Remove(variable);
        }
    }
}
