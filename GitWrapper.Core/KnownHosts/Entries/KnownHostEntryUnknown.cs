namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    /// Represents an unrecognized entry in a known_hosts file.
    /// </summary>
    public sealed class KnownHostEntryUnknown : KnownHostEntryBase
    {
        /// <summary>
        /// Gets the original content of the entry.
        /// </summary>
        public required string Content { get; init; }

        /// <inheritdoc />
        public override string Serialize() => Content;
    }
}
