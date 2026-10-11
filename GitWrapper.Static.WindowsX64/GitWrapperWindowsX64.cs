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

        /// <inheritdoc cref="GitWrapperBase.Identifier"/>
        public static string ClassIdentifier { get; } = $"{OSPlatform.Windows}-{Architecture.X64}";

        /// <inheritdoc/>
        public override string Identifier { get; } = ClassIdentifier;

        /// <inheritdoc/>
        public override async Task ExtractArchiveAsync(string? destination = null, bool clearBeforeExtract = false)
        {
            EnsureSupported();

            InitialisingThePaths(
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

                if (".zip" != Path.GetExtension(ArchiveFile))
                {
                    throw new Exception($"File Extension must be '.zip'");
                }
            
                await ArchiveHandler.ExtractZipToDirectoryAsync(
                    ArchiveFile,
                    RuntimeDirectory,
                    overwriteFiles: true);
            }

            ConfigureProcessStartInfo = processStartInfo =>
            {
                var envVar = new ProcessEnvironmentVariable(processStartInfo);

                envVar.Add("PATH", BinaryDirectory);
            };
        }
    }
}
