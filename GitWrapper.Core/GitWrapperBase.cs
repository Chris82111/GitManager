using Chris82111.GitManager.GitWrapper.Core.Helpers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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

        /// <inheritdoc/>
        public virtual string ExtractDirectory { get; set; } = ".bin";

        /// <inheritdoc/>
        public string? ArchiveFile { get; protected set; } = null;

        /// <inheritdoc/>
        public virtual string? RuntimeDirectory { get; protected set; } = null;

        /// <inheritdoc/>
        public virtual string? BinaryDirectory { get; protected set; } = null;

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

        /// <summary>
        /// Directory of *.dll and *.exe files
        /// </summary>
        protected static string BaseDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        ///         Action that is called after a new instance of the <see cref="ProcessStartInfo"/> class has been created
        /// <br/>   This action can be used to set environment variables.
        /// </summary>
        protected Action<ProcessStartInfo>? ConfigureProcessStartInfo { get; set; }

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

            ConfigureProcessStartInfo?.Invoke(psi);

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

            ConfigureProcessStartInfo?.Invoke(psi);

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
        public virtual ProcessResultsDto Execute(string fileName, string parameter, string? directory = null)
        {
            EnsureSupported();

            var processResults = new ProcessResultsDto();

            if (string.IsNullOrEmpty(directory))
            {
                directory = Directory;
            }

            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = parameter,
                WorkingDirectory = directory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            psi.Environment["GIT_ASKPASS"] = "echo";

            ConfigureProcessStartInfo?.Invoke(psi);

            var process = new Process { StartInfo = psi };

            if (null == process)
            {
                return processResults;
            }

            process.Start();

            processResults.StandardOutput = process.StandardOutput.ReadToEnd().TrimEnd('\n').TrimEnd('\r');
            processResults.StandardError = process.StandardError.ReadToEnd().TrimEnd('\n').TrimEnd('\r');

            process.WaitForExit();

            processResults.ExitCode = process.ExitCode;

            return processResults;
        }

        /// <inheritdoc/>
        public virtual async Task<ProcessResultsDto> ExecuteAsync(string fileName, string parameter, string? directory = null)
        {
            EnsureSupported();

            var processResults = new ProcessResultsDto();

            if (string.IsNullOrEmpty(directory))
            {
                directory = Directory;
            }

            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = parameter,
                WorkingDirectory = directory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            psi.Environment["GIT_ASKPASS"] = "echo";

            ConfigureProcessStartInfo?.Invoke(psi);

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
        public virtual void ExtractArchive(string? destination = null, bool clearBeforeExtract = false)
        {
            // Every Extract function must implement EnsureSupported.
            // Since the Asynchronous function is called here, this
            // is omitted to avoid a duplicate call.
            // EnsureSupported();

            ExtractArchiveAsync(destination, clearBeforeExtract)
                .GetAwaiter()
                .GetResult();
        }

        /// <inheritdoc/>
        public virtual async Task ExtractArchiveAsync(string? destination = null, bool clearBeforeExtract = false)
        {
            EnsureSupported();
        }

        /// <inheritdoc/>
        public virtual void RemoveExtracted()
        {
            EnsureSupported();

            if (null == RuntimeDirectory)
            {
                return;
            }

            var runtimeDirectory = RuntimeDirectory;

            System.IO.Directory.CreateDirectory(runtimeDirectory);

            var files = System.IO.Directory.EnumerateFiles(
                runtimeDirectory,
                "*",
                SearchOption.AllDirectories);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            System.IO.Directory.Delete(runtimeDirectory, recursive: true);

            RuntimeDirectory = null;
            BinaryDirectory = null;
        }

        /// <inheritdoc/>
        public virtual async Task RemoveExtractedAsync(ParallelOptions? parallelOptions = null)
        {
            EnsureSupported();

            if (null == RuntimeDirectory)
            {
                return;
            }

            var runtimeDirectory = RuntimeDirectory;

            System.IO.Directory.CreateDirectory(runtimeDirectory);

            var files = System.IO.Directory.EnumerateFiles(
                runtimeDirectory,
                "*",
                SearchOption.AllDirectories);

            await Parallel.ForEachAsync(
                files,
                parallelOptions ?? new ParallelOptions(),
                (file, cancellationToken) =>
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    return ValueTask.CompletedTask;
                });

            System.IO.Directory.Delete(runtimeDirectory, recursive: true);

            RuntimeDirectory = null;
            BinaryDirectory = null;
        }

        /// <summary>
        ///         Determines the required paths to the directories and the application to be launched.
        /// <br/>   In this way, all plugins use the same paths.
        /// <br/>   
        /// <br/>   The full path of the archive file is set here:
        /// <br/>   <see cref="ArchiveFile"/>
        /// <br/>   
        /// <br/>   Is only changed if the parameter <paramref name="destination"/> is set:
        /// <br/>   <see cref="ExtractDirectory"/>
        /// <br/>   
        /// <br/>   The full path of the directories are set here:
        /// <br/>   <see cref="RuntimeDirectory"/>
        /// <br/>   <see cref="BinaryDirectory"/>
        /// <br/>   
        /// <br/>   The full path of the file is set here:
        /// <br/>   <see cref="AppName"/>
        /// </summary>
        /// <param name="destination">The target specified by the user via parameters of the <see cref="ExtractArchive(string?, bool)"/> or <see cref="ExtractArchiveAsync(string?, bool)"/> methods.</param>
        /// <param name="archiveFileRelative">Relative path to and including the archive</param>
        /// <param name="binaryFileRelative">Relative path to and including the binary</param>
        [MemberNotNull(nameof(ArchiveFile))]
        [MemberNotNull(nameof(RuntimeDirectory))]
        [MemberNotNull(nameof(BinaryDirectory))]
        [MemberNotNull(nameof(AppName))]
        protected void InitializingPaths(string? destination, string archiveFileRelative, string binaryFileRelative)
        {
            archiveFileRelative = PathHelper.ReplacePathSeparatorsOnly(archiveFileRelative);
            binaryFileRelative = PathHelper.ReplacePathSeparatorsOnly(binaryFileRelative);

            ArchiveFile = Path.Combine(BaseDirectory, archiveFileRelative);

            var relativeExtractPath = Path.GetDirectoryName(Path.GetDirectoryName(archiveFileRelative)!)!;
            var relativeBinaryPath = Path.GetDirectoryName(binaryFileRelative)!;

            if (null == destination)
            {
                RuntimeDirectory = Path.Combine(BaseDirectory, ExtractDirectory, relativeExtractPath);
            }
            else
            {
                if (Path.IsPathRooted(destination))
                {
                    ExtractDirectory = destination;
                    RuntimeDirectory = Path.Combine(destination, relativeExtractPath);
                }
                else
                {
                    ExtractDirectory = Path.Combine(BaseDirectory, destination);
                    RuntimeDirectory = Path.Combine(ExtractDirectory, relativeExtractPath);
                }
            }

            BinaryDirectory = Path.Combine(RuntimeDirectory, relativeBinaryPath);
            
            AppName = Path.Combine(
                RuntimeDirectory,
                binaryFileRelative);
        }
    }
}
