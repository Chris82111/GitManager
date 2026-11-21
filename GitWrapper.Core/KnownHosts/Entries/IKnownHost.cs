namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    /// Represents a known SSH host, including its hostname, key information, and optional marker.
    /// </summary>
    public interface IKnownHost : IKnownHostEntry
    {
        /// <summary>
        /// Gets or sets the marker associated with the known host.
        /// </summary>
        KnownHostMarkers Marker { get; set; }

        /// <summary>
        /// Gets the hostname associated with the known host.
        /// </summary>
        string Hostname { get; }

        /// <summary>
        /// Gets the SSH public key algorithm type.
        /// </summary>
        string KeyType { get; }

        /// <summary>
        /// Gets the Base64-encoded SSH public key.
        /// </summary>
        string PublicKey { get; }

        /// <summary>
        /// Gets the SHA-256 fingerprint of the public key.
        /// </summary>
        /// <value>
        /// The SHA-256 fingerprint, or <see langword="string.Empty"/> if no hash could be calculated.
        /// </value>
        string FingerprintSha256 { get; }

        /// <summary>
        /// Gets the optional comment associated with the known host.
        /// </summary>
        string? Comment { get; }

        /// <inheritdoc cref="MatchesHost(string, int, string?)"/>
        bool MatchesHost(string host, string? keyType = null);

        /// <summary>
        /// Determines whether the entry matches the specification
        /// </summary>
        /// <param name="host">The hostname to match.</param>
        /// <param name="port">The port to match.</param>
        /// <param name="keyType">The optional SSH key type to match.</param>
        /// <returns>
        ///         <see langword="true"/>  if the entry matches,
        /// <br/>   <see langword="false"/> otherwise.
        /// </returns>
        bool MatchesHost(string host, int port, string? keyType = null);
    }
}