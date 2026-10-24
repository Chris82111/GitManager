using Chris82111.GitManager.GitWrapper.Core;
using Chris82111.GitManager.GitWrapper.Core.Archives;
using Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables;
using Chris82111.GitManager.GitWrapper.Static.WindowsX64.Lib;
using Chris82111.GitManager.GitWrapper.Static.WindowsX64.LibSshHelper;
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
        public override string SshPassphrase { get; protected set; } = LibSshHelperPaths.FileRelative;

        /// <inheritdoc cref="GitWrapperBase.Identifier"/>
        public static string ClassIdentifier { get; } = $"{OSPlatform.Windows}-{Architecture.X64}";

        /// <inheritdoc/>
        public override string Identifier { get; } = ClassIdentifier;

        /// <inheritdoc/>
        public override async Task ExtractArchiveAsync(string? destination = null, ExtractConditions? extractConditions = null)
        {
            EnsureSupported();

            InitializingPaths(
                destination,
                ArchivePaths.ArchiveFileRelative,
                ArchivePaths.ExecutableRelative);

            if (await PrepareAndCheckExtractAsync(extractConditions))
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
