using Chris82111.GitManager.GitWrapper.Core;

namespace Chris82111.GitManager.GitWrapper.Static.LinuxX64
{
    /// <summary>
    /// Plugin to use the static Git version for the system in the class name.
    /// </summary>
    public sealed class GitWrapperLinuxX64Plugin : IGitPlugin
    {
        /// <inheritdoc/>
        public void Register()
        {
            GitWrapperFactory.Register(
                GitWrapperLinuxX64.ClassIdentifier,
                static () => new GitWrapperLinuxX64());
        }
    }
}
