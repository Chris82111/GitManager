namespace Chris82111.GitManager.GitWrapper.Core
{
    /// <summary>
    ///         Interface used by the <see cref="GitWrapperBase"/> class and by all system-specific classes.
    /// <br/>   Enables the implementation of a factory pattern, <see cref="GitWrapperFactory"/>.
    /// </summary>
    public interface IGitWrapper
    {
        /// <summary>
        ///         Name of the Git application `git` (to use the system installation) or
        /// <br/>   the absolute path to the static Git version.
        /// </summary>
        string AppName { get; }

        /// <summary>
        /// The directory in which the Git commands are executed.
        /// </summary>
        string Directory { get; set; }

        /// <summary>
        ///         The functions <see cref="ExtractArchive(string?)"/> and <see cref="ExtractArchiveAsync(string?)"/>
        /// <br/>   extract the system-specific binaries to the directory specified here.
        /// <br/>   The directory can be changed and specified as either a relative or absolute path.
        /// </summary>
        string ExtractDirectory { get; set; }

        /// <summary>
        ///         An identifier string consisting of the operating system platform and the
        /// <br/>   architecture, separated by a hyphen ("-").
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// A property that indicates whether the class can be used on the current system.
        /// </summary>
        /// <returns>
        ///         true:  If the class can be used on the system
        /// <br/>   false: If the class can not be used on the system
        /// </returns>
        bool IsSupported { get; }

        /// <summary>
        /// Ensures that the current platform supports this operation.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException">
        /// Thrown if the current platform does not support this operation.
        /// </exception>
        void EnsureSupported();

        /// <summary>
        /// Checks whether Git is available
        /// </summary>
        /// <returns>
        ///         true:  If Git is available
        /// <br/>   false: if Git is not available
        /// </returns>
        bool IsGitAvailable();

        /// <inheritdoc cref="IsGitAvailable"/>
        Task<bool> IsGitAvailableAsync();

        /// <summary>
        /// Determine the version of Git
        /// </summary>
        /// <returns>Git version</returns>
        string GitVersion();

        /// <inheritdoc cref="GitVersion"/>
        Task<string> GitVersionAsync();

        /// <summary>
        /// Mirrors the repository
        /// </summary>
        /// <param name="url">The repository's URL</param>
        /// <param name="directory"><inheritdoc cref="Git" path="//param[@name='directory']/node()"/></param>
        ProcessResultsDto Mirror(string url, string? directory = null);

        /// <inheritdoc cref="Mirror"/>
        Task<ProcessResultsDto> MirrorAsync(string url, string? directory = null);

        /// <summary>
        ///         Calls the Git program, using the variable <see cref="AppName"/> for this purpose
        /// </summary>
        /// <param name="parameter">The arguments of git</param>
        /// <param name="directory">The working directory in which the command is executed.
        /// <br/>   If the parameter is null or empty, the <see cref="Directory"/> property is used.
        /// <br/>   The directory can only be specified for this execution.</param>
        /// <returns>Returns information about the process</returns>
        ProcessResultsDto Git(string parameter, string? directory = null);

        /// <inheritdoc cref="Git"/>
        Task<ProcessResultsDto> GitAsync(string parameter, string? directory = null);

        /// <summary>
        ///         Unlike <see cref="Git(string, string?)"/>, this executes a command
        /// <br/>   for which the application itself can be specified. If <see cref="AppName"/>
        /// <br/>   is used, it functions the same as <see cref="Git(string, string?)"/>.
        /// <br/>
        /// <br/>   Consider using these:
        /// <br/>   - <see cref="AppName"/> property or
        /// <br/>   - <see cref="Git(string, string?)"/> method
        /// </summary>
        /// <param name="fileName">
        ///         The application or document to start
        /// <br/>
        /// <br/>   Attention: Process- and platform-specific rules are used to resolve the path.
        /// <br/>   See: <see cref="Execute(string, string, string?)"/></param>
        /// <param name="parameter">Command-line arguments to use when starting the application</param>
        /// <param name="directory">Working directory for the process to be started</param>
        /// <returns>Returns information about the process</returns>
        /// <remarks>
        ///         <strong> Warning: </strong>
        /// <br/>   On Unix-like systems, .NET's process-start implementation resolves executable
        /// <br/>   names using the process environment and platform-specific rules.
        /// <br/>   If <paramref name="fileName"/> is set to "git", the system's native Git is used.
        /// <br/>   However, the program that is launched recognizes the modified environment
        /// <br/>   variables. If the started program/script or something else calls "git", for example,
        /// <br/>   the program is resolved and launched based on the modified process environment and
        /// <br/>   platform-specific rules. So the wrapper's Git is used.
        /// </remarks>
        ProcessResultsDto Execute(string fileName, string parameter, string? directory = null);

        /// <inheritdoc cref="Execute"/>
        Task<ProcessResultsDto> ExecuteAsync(string fileName, string parameter, string? directory = null);

        /// <summary>
        ///         Checks various sources of the archive, defines paths,
        /// <br/>   and unpacks the archive depending on the system.
        /// <br/>
        /// <br/>   Must be called before using other functions.
        /// </summary>
        /// <param name="destination">Output path to which the archive is extracted</param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        void ExtractArchive(string? destination = null);

        /// <inheritdoc cref="ExtractArchive"/>
        Task ExtractArchiveAsync(string? destination = null);
    }
}
