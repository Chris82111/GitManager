namespace Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables
{
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

        public static EnvironmentVariable Instance => instance;

        protected override string? GetValue(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }

        protected override void SetValue(string variable, string value)
        {
            Environment.SetEnvironmentVariable(variable, value);
        }

        protected override void RemoveValue(string variable)
        {
            Environment.SetEnvironmentVariable(variable, null);
        }
    }
}
