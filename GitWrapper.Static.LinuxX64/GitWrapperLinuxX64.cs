using Chris82111.GitManager.GitWrapper.Core;
using Chris82111.GitManager.GitWrapper.Core.Archives;
using Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables;
using Chris82111.GitManager.GitWrapper.Core.Helpers;
using Chris82111.GitManager.GitWrapper.Static.LinuxX64.Lib;
using System.Runtime.InteropServices;

namespace Chris82111.GitManager.GitWrapper.Static.LinuxX64
{
    /// <summary>
    /// System-specific implementation to use the static Git version for the system in the class name.
    /// </summary>
    public sealed class GitWrapperLinuxX64 : GitWrapperBase
    {
        /// <inheritdoc/>
        public override string AppName { get; protected set; } = "";

        /// <inheritdoc cref="GitWrapperBase.Identifier"/>
        public static string ClassIdentifier { get; } = $"{OSPlatform.Linux}-{Architecture.X64}";

        /// <inheritdoc/>
        public override string Identifier { get; } = ClassIdentifier;

        /// <inheritdoc/>
        public override async Task ExtractArchiveAsync(string? destination = null, bool clearBeforeExtract = false)
        {
            EnsureSupported();

            InitializingPaths(
                destination,
                ArchivePaths.ArchiveFileRelative,
                ArchivePaths.ExecutableRelative);

            if (clearBeforeExtract)
            {
                await RemoveExtractedAsync();
            }

            if (PathHelper.IsDirectoryMissingOrEmpty(RuntimeDirectory))
            {
                if (false == File.Exists(ArchiveFile))
                {
                    throw new FileNotFoundException($"File was no found: {ArchiveFile}");
                }

                if (false == PathHelper.IsExtension(ArchiveFile, ".tar.gz"))
                {
                    throw new Exception($"File Extension must be '.tar.gz'");
                }

                SymlinkHelper.EnsureSymlinkSupported(RuntimeDirectory);

                await ArchiveHandler.ExtractTarGzToDirectoryAsync(
                    ArchiveFile,
                    RuntimeDirectory,
                    overwriteFiles: true);
            }

            ConfigureProcessStartInfo = processStartInfo =>
            {
                var envVar = new ProcessEnvironmentVariable(processStartInfo);

                envVar.Add("PATH", BinaryDirectory);

                envVar.Set("GIT_PREFIX", RuntimeDirectory);
                envVar.Set("GIT_EXEC_PATH", Path.Combine(RuntimeDirectory, "libexec", "git-core"));
                envVar.Set("GIT_TEMPLATE_DIR", Path.Combine(RuntimeDirectory, "share", "git-core", "templates"));
                envVar.Set("GIT_SSL_CAINFO", Path.Combine(RuntimeDirectory, "ca", "ca.pem"));

                envVar.Add("LD_LIBRARY_PATH", Path.Combine(RuntimeDirectory, "openssl", "lib64"));
                envVar.Add("LD_LIBRARY_PATH", Path.Combine(RuntimeDirectory, "curl", "lib"));
            };
        }
    }
}
