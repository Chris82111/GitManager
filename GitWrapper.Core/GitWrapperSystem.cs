namespace Chris82111.GitManager.GitWrapper.Core
{
    /// <summary>
    /// The default implementation uses the system's built-in Git installation.
    /// </summary>
    public class GitWrapperSystem : GitWrapperBase
    {
        /// <inheritdoc/>
        public override string AppName { get; protected set; } = "git";

        /// <inheritdoc/>
        public override string? SshPassphrase { get; protected set; } = null;

        /// <inheritdoc cref="GitWrapperBase.Identifier"/>
        public static string ClassIdentifier { get; } = "default";

        /// <inheritdoc/>
        public override string Identifier { get; } = ClassIdentifier;
    }
}
