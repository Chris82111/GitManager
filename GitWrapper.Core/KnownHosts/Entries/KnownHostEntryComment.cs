namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    /// Represents a comment entry in a known_hosts file.
    /// </summary>
    public sealed class KnownHostEntryComment : KnownHostEntryBase
    {
        /// <summary>
        /// Gets the comment text.
        /// </summary>
        public required string Comment { get; init; }

        /// <inheritdoc />
        public override string Serialize() => Comment;
    }
}
