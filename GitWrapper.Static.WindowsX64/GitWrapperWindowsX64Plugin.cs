using Chris82111.GitManager.GitWrapper.Core;

namespace Chris82111.GitManager.GitWrapper.Static.WindowsX64
{
    /// <summary>
    /// Plugin to use the static Git version for the system in the class name.
    /// </summary>
    public sealed class GitWrapperWindowsX64Plugin : IGitPlugin
    {
        /// <inheritdoc/>
        public void Register()
        {
            GitWrapperFactory.Register(
                GitWrapperWindowsX64.ClassIdentifier,
                static () => new GitWrapperWindowsX64());
        }
    }
}
