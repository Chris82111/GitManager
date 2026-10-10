using Chris82111.GitManager.GitWrapper.Core;
using Chris82111.GitManager.GitWrapper.Core.Archives;
using Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables;
using Chris82111.GitManager.GitWrapper.Core.Helpers;
using Chris82111.GitManager.GitWrapper.Static.WindowsX64.Lib;
using System.Runtime.InteropServices;

namespace Chris82111.GitManager.GitWrapper.Static.WindowsX64
{
    /// <summary>
    /// System-specific implementation to use the static Git version for the system in the class name.
    /// </summary>
   public sealed class GitWrapperWindowsX64 : GitWrapperBase
   {
        /// <inheritdoc/>
        public override string AppName { get; protected set; } = "";

        /// <inheritdoc/>
        public static string ClassIdentifier { get; } = $"{OSPlatform.Windows}-{Architecture.X64}";

        /// <inheritdoc/>
        public override string Identifier { get; } = ClassIdentifier;

        /// <inheritdoc/>
        public override async Task ExtractArchiveAsync(string? destination = null)
        {
            EnsureSupported();

            var output = GetOutputDirectoryForExtract(destination, ArchivePaths.ArchiveFileRelative);

            if (PathHelper.IsDirectoryMissingOrEmpty(output))
            {
                var archive = Path.Combine(BaseDirectory, ArchivePaths.ArchiveFileRelative);

                if (false == File.Exists(archive))
                {
                    throw new FileNotFoundException($"File was no found: {ArchivePaths.ArchiveFileRelative}");
                }

                if (".zip" != Path.GetExtension(archive))
                {
                    throw new Exception($"File Extension must be '.zip'");
                }
            
                await ArchiveHandler.ExtractZipToDirectoryAsync(
                    archive,
                    output,
                    overwriteFiles: true);
            }

            ConfigureProcessStartInfo = processStartInfo =>
            {
                var envVar = new ProcessEnvironmentVariable(processStartInfo);

                envVar.Add("PATH", Path.Combine(output, "cmd"));
            };

            AppName = PathHelper.ReplacePathSeparatorsOnly(
                Path.Combine(output, ArchivePaths.ExecutableRelative));
        }
    }
}
