namespace Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables
{
    /// <summary>
    /// This implementation allows it to modify the system environment variables
    /// </summary>
    public sealed class EnvironmentVariable : EnvironmentVariableBase
    {
        private static readonly EnvironmentVariable instance = new EnvironmentVariable();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static EnvironmentVariable()
        {
        }

        private EnvironmentVariable()
        {
        }

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        public static EnvironmentVariable Instance => instance;

        /// <inheritdoc/>
        protected override string? GetValue(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }

        /// <inheritdoc/>
        protected override void SetValue(string variable, string value)
        {
            Environment.SetEnvironmentVariable(variable, value);
        }

        /// <inheritdoc/>
        protected override void RemoveValue(string variable)
        {
            Environment.SetEnvironmentVariable(variable, null);
        }
    }
}
