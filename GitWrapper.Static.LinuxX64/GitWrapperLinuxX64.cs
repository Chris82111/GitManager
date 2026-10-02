using Chris82111.GitManager.GitWrapper.Core;
using Chris82111.GitManager.GitWrapper.Core.Archives;
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

        /// <inheritdoc/>
        public static string ClassIdentifier { get; } = $"{OSPlatform.Linux}-{Architecture.X64}";

        /// <inheritdoc/>
        public override string Identifier { get; } = ClassIdentifier;

        /// <inheritdoc/>
        public override async Task ExtractArchiveAsync(string? destination = null)
        {
            EnsureSupported();

            var archive = Path.Combine(BaseDirectory, ArchivePaths.ArchiveFileRelative);

            if (false == File.Exists(archive))
            {
                throw new FileNotFoundException($"File was no found: {ArchivePaths.ArchiveFileRelative}");
            }

            if (false == PathHelper.IsExtension(archive, ".tar.gz"))
            {
                throw new Exception($"File Extension must be '.tar.gz'");
            }

            var output = GetOutputDirectoryForExtract(destination, ArchivePaths.ArchiveFileRelative);

            SymlinkHelper.EnsureSymlinkSupported(output);

            if (PathHelper.IsDirectoryMissingOrEmpty(output))
            {
                await ArchiveHandler.ExtractTarGzToDirectoryAsync(
                    archive,
                    output,
                    overwriteFiles: true);
            }

            EnvironmentVariableHelper.SetToPahtVariable(Path.Combine(output, "bin"));

            Environment.SetEnvironmentVariable("GIT_PREFIX", output);
            Environment.SetEnvironmentVariable("GIT_EXEC_PATH", Path.Combine(output, "libexec", "git-core"));
            Environment.SetEnvironmentVariable("GIT_TEMPLATE_DIR", Path.Combine(output, "share", "git-core", "templates"));
            Environment.SetEnvironmentVariable("GIT_SSL_CAINFO", Path.Combine(output, "ca", "ca.pem"));

            EnvironmentVariableHelper.AddToVariable("LD_LIBRARY_PATH", Path.Combine(output, "openssl", "lib64"));
            EnvironmentVariableHelper.AddToVariable("LD_LIBRARY_PATH", Path.Combine(output, "curl", "lib"));

            AppName = PathHelper.ReplacePathSeparatorsOnly(
                Path.Combine(output, ArchivePaths.ExecutableRelative));
        }
    }
}
