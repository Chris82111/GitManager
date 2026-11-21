namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    /// Represents a known host entry that can be serialized.
    /// </summary>
    public interface IKnownHostEntry
    {
        /// <summary>
        ///         Serializes the known host entry into its string representation.
        /// <br/>   This means a line within the known_hosts file.
        /// </summary>
        /// <returns>The serialized known host entry.</returns>
        string Serialize();
    }
}