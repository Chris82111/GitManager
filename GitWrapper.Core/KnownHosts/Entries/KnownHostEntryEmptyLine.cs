namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    /// Represents an empty line in a known_hosts file.
    /// </summary>
    public sealed class KnownHostEntryEmptyLine : KnownHostEntryBase
    {
        /// <summary>
        /// Returns an empty string representing an empty line.
        /// </summary>
        /// <returns>An empty string.</returns>
        public override string Serialize() => string.Empty;
    }
}
