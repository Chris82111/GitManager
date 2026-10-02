namespace Chris82111.GitManager.GitWrapper.Core
{

    /// <summary>
    /// Plugin to use the system's built-in Git version.
    /// </summary>
    public sealed class GitWrapperPlugin : IGitPlugin
    {
        /// <inheritdoc/>
        public void Register()
        {
            GitWrapperFactory.Register(
                GitWrapperSystem.ClassIdentifier,
                static () => new GitWrapperSystem());
        }
    }
}
