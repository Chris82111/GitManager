using Chris82111.GitManager.GitWrapper.Core.Helpers;
using System.Diagnostics;

namespace Chris82111.GitManager.GitWrapper.Core
{
    /// <summary>
    /// A base class that provides several implementations for the respective system-specific classes.
    /// </summary>
    public abstract class GitWrapperBase : IGitWrapper
    {
        /// <inheritdoc/>
        public abstract string AppName { get; protected set; }

        /// <inheritdoc/>
        public string Directory { get; set; } = ".";

        /// <summary>
        /// Directory of *.dll and *.exe files
        /// </summary>
        protected static string BaseDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;

        /// <inheritdoc/>
        public virtual string ExtractDirectory { get; set; } = ".bin";

        /// <inheritdoc/>
        public abstract string Identifier { get; }

        /// <inheritdoc/>
        public virtual bool IsSupported
        {
            get
            {
                return RuntimeHelper.Identifier == Identifier || "default" == Identifier;
            }
        }

        /// <inheritdoc/>
        public virtual void EnsureSupported()
        {
            if (false == IsSupported)
            {
                throw new PlatformNotSupportedException(
                    $"The used plugin does not support '{RuntimeHelper.Identifier}'.{Environment.NewLine}"
                    + $"The plugin only supports '{Identifier}'");
            }
        }

        /// <inheritdoc/>
        public virtual bool IsGitAvailable()
        {
            return ApplicationHelper.IsProgramAvailable(AppName);
        }

        /// <inheritdoc/>
        public virtual async Task<bool> IsGitAvailableAsync()
        {
            return await ApplicationHelper.IsProgramAvailableAsync(AppName);
        }

        /// <inheritdoc/>
        public virtual string GitVersion()
        {
            return Git("-v").StandardOutput;
        }

        /// <inheritdoc/>
        public virtual async Task<string> GitVersionAsync()
        {
            return (await GitAsync("-v")).StandardOutput;
        }

        /// <inheritdoc/>
        public virtual ProcessResultsDto Mirror(string url, string? directory = null)
        {
            return Git($"clone --mirror \"{url}\"", directory);
        }

        /// <inheritdoc/>
        public virtual async Task<ProcessResultsDto> MirrorAsync(string url, string? directory = null)
        {
            return await GitAsync($"clone --mirror \"{url}\"", directory);
        }

        /// <inheritdoc/>
        public virtual ProcessResultsDto Git(string parameter, string? directory = null)
        {
            EnsureSupported();

            if (string.IsNullOrEmpty(AppName))
            {
                throw new InvalidOperationException(
                    $"The property '{nameof(AppName)}' is not set.{Environment.NewLine}" +
                    $"Before calling this method, call the '{nameof(ExtractArchive)}' or '{nameof(ExtractArchiveAsync)}' method.");
            }

            var processResults = new ProcessResultsDto();

            if (string.IsNullOrEmpty(directory))
            {
                directory = Directory;
            }

            var psi = new ProcessStartInfo
            {
                FileName = AppName,
                Arguments = parameter,
                WorkingDirectory = directory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            psi.Environment["GIT_ASKPASS"] = "echo";

            var process = new Process { StartInfo = psi };

            if (null == process)
            {
                return processResults;
            }

            process.Start();

            processResults.StandardOutput = process.StandardOutput.ReadToEnd().TrimEnd('\n').TrimEnd('\r'); ;
            processResults.StandardError = process.StandardError.ReadToEnd().TrimEnd('\n').TrimEnd('\r'); ;

            process.WaitForExit();

            processResults.ExitCode = process.ExitCode;

            return processResults;
        }

        /// <inheritdoc/>
        public virtual async Task<ProcessResultsDto> GitAsync(string parameter, string? directory = null)
        {
            EnsureSupported();

            if (string.IsNullOrEmpty(AppName))
            {
                throw new InvalidOperationException(
                    $"The property '{nameof(AppName)}' is not set.{Environment.NewLine}" +
                    $"Before calling this method, call the '{nameof(ExtractArchive)}' or '{nameof(ExtractArchiveAsync)}' method.");
            }

            var processResults = new ProcessResultsDto();

            if (string.IsNullOrEmpty(directory))
            {
                directory = Directory;
            }

            var psi = new ProcessStartInfo
            {
                FileName = AppName,
                Arguments = parameter,
                WorkingDirectory = directory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            psi.Environment["GIT_ASKPASS"] = "echo";

            var process = new Process { StartInfo = psi };

            if (null == process)
            {
                return processResults;
            }

            process.Start();
            
            processResults.StandardOutput = (await process.StandardOutput.ReadToEndAsync()).TrimEnd('\n').TrimEnd('\r');
            processResults.StandardError = (await process.StandardError.ReadToEndAsync()).TrimEnd('\n').TrimEnd('\r');

            await process.WaitForExitAsync();

            processResults.ExitCode = process.ExitCode;

            return processResults;
        }

        /// <inheritdoc/>
        public virtual void ExtractArchive(string? destination = null)
        {
            EnsureSupported();

            ExtractArchiveAsync(destination)
                .GetAwaiter()
                .GetResult();
        }

        /// <inheritdoc/>
        public virtual async Task ExtractArchiveAsync(string? destination = null)
        {
            EnsureSupported();
        }

        /// <summary>
        ///         Determines the output directory into which the archive will be extracted.
        /// <br/>   This allows individual plugins to determine the path in the same way every time.
        /// </summary>
        /// <param name="destination">The target specified by the user via parameters of the <see cref="ExtractArchive(string?)"/> or <see cref="ExtractArchiveAsync(string?)"/> methods.</param>
        /// <param name="archiveFileRelative">Relative path to and including the archive.</param>
        /// <returns>Output directory where the archive will be extracted</returns>
        protected string GetOutputDirectoryForExtract(string? destination, string archiveFileRelative)
        {
            var relativeExtractPath = Path.GetDirectoryName(archiveFileRelative)!;
            
            return (null == destination)
                ? Path.Combine(BaseDirectory, ExtractDirectory, relativeExtractPath)
                : Path.IsPathRooted(destination)
                    ? Path.Combine(destination, relativeExtractPath)
                    : Path.Combine(BaseDirectory, destination, relativeExtractPath);
        }
    }
}
