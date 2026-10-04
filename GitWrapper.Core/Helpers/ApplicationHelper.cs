using Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Chris82111.GitManager.GitWrapper.Core.Helpers
{
    /// <summary>
    /// Help with using applications
    /// </summary>
    public class ApplicationHelper
    {
        /// <summary>
        /// Defines which command is used to determine the location of an application
        /// </summary>
        private static readonly string WhichCommand = OperatingSystem.IsWindows() ? "where" : "which";

        /// <summary>
        ///         Checks whether a program is available.
        /// <br/>   
        /// <br/>   The program name can be specified.
        /// <br/>   A path to the program can also be specified.
        /// </summary>
        /// <param name="programName">The program to be tested</param>
        /// <returns>
        ///         true, the program is available
        /// <br/>   false, the program is not available</returns>
        public static bool IsProgramAvailable(string? programName)
        {
            if (string.IsNullOrEmpty(programName))
            {
                return false;
            }

            var path = PathHelper.ReplacePathSeparatorsOnly(Path.GetDirectoryName(programName));
            if (false == string.IsNullOrEmpty(path))
            {
                path = Path.GetFullPath(path);

                programName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? Path.GetFileNameWithoutExtension(programName)
                    : Path.GetFileName(programName);
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = WhichCommand,
                    Arguments = programName,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            if (false == string.IsNullOrEmpty(path))
            {
                var envVar = new ProcessEnvironmentVariable(process.StartInfo);
                envVar.Add("PATH", path);
            }

            process.Start();

            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return 0 == process.ExitCode && false == string.IsNullOrEmpty(output);
        }

        /// <inheritdoc cref="IsProgramAvailable(string?)"/>
        public static async Task<bool> IsProgramAvailableAsync(string? programName)
        {
            if (string.IsNullOrEmpty(programName))
            {
                return false;
            }

            var path = PathHelper.ReplacePathSeparatorsOnly(Path.GetDirectoryName(programName));
            if (false == string.IsNullOrEmpty(path))
            {
                path = Path.GetFullPath(path);

                programName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? Path.GetFileNameWithoutExtension(programName)
                    : Path.GetFileName(programName);
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = WhichCommand,
                    Arguments = programName,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            if (false == string.IsNullOrEmpty(path))
            {
                var envVar = new ProcessEnvironmentVariable(process.StartInfo);
                envVar.Add("PATH", path);
            }

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();

            await process.WaitForExitAsync();

            return 0 == process.ExitCode && false == string.IsNullOrWhiteSpace(output);
        }

    }
}
