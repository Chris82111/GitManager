namespace Chris82111.GitManager.GitWrapper.Core.Helpers
{
    /// <summary>
    /// Helps with working with environment variables
    /// </summary>
    public class EnvironmentVariableHelper
    {
        /// <summary>
        /// The individual environment variables are separated by different delimiters in Linux (':') and Windows (';') systems.
        /// </summary>
        private static readonly string PathEnvironmentSeparator = OperatingSystem.IsWindows() ? ";" : ":";

        /// <summary>
        /// Append additional values to an environment variable
        /// </summary>
        /// <param name="variable">Name of the environment variable</param>
        /// <param name="value">Value to be appended</param>
        public static void AddToVariable(string variable, string value)
        {
            Environment.SetEnvironmentVariable(
                variable,
                CombineVariable(variable, value));
        }

        /// <summary>
        /// Append additional values to the PATH environment variable
        /// </summary>
        /// <param name="newDirectory">Directory to be appended</param>
        public static void SetToPahtVariable(string newDirectory)
        {
            Environment.SetEnvironmentVariable(
                "PATH",
                CombineVariable("PATH", newDirectory));
        }

        /// <summary>
        /// Adds additional values to an environment variable and returns the result, but does not make any changes
        /// </summary>
        /// <param name="variable">Name of the environment variable</param>
        /// <param name="value">Value to be appended</param>
        /// <returns>The new and the old value</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static string CombineVariable(string variable, string value)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new NullReferenceException($"Variable {nameof(variable)} must not be null or empty");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new NullReferenceException($"Variable {nameof(value)} must not be null or empty");
            }

            value = PathHelper.ReplacePathSeparatorsOnly(value);

            var content = Environment.GetEnvironmentVariable(variable);

            content = string.IsNullOrEmpty(content)
                ? value
                : value + PathEnvironmentSeparator + content;

            return content;
        }
    }
}
