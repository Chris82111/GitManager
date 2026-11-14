namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    ///         Use these static functions to convert to or from a string:
    /// <br/>   
    /// <br/>   From string to enum: <see cref="KnownHostMarkersExtensions.ParseValue(string?)"/>
    /// <br/>   From enum to string: <c>enumVariable.ToValue()</c> or <see cref="KnownHostMarkersExtensions.ToValue(KnownHostMarkers)"/>
    /// </summary>
    public enum KnownHostMarkers
    {
        None,

        CertAuthority,

        Revoked,
    }
}
