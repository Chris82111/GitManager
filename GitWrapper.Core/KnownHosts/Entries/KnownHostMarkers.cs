namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    /// Specifies the marker associated with a known host entry.
    /// </summary>
    /// <remarks>
    ///         Use the extension methods to convert between enum values and their string representations:
    /// <br/>   From string to enum: <see cref="KnownHostMarkersExtensions.ParseValue(string?)"/>
    /// <br/>   From enum to string: <c>enumVariable.ToValue()</c> or <see cref="KnownHostMarkersExtensions.ToValue(KnownHostMarkers)"/>.
    /// </remarks>
    public enum KnownHostMarkers
    {
        /// <summary>
        /// Indicates that the entry has no marker.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the entry represents a certificate authority.
        /// </summary>
        CertAuthority,

        /// <summary>
        /// Indicates that the associated host key is revoked.
        /// </summary>
        Revoked,
    }
}
