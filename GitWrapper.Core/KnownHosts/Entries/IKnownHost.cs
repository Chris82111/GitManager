namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    public interface IKnownHost : IKnownHostEntry
    {
        KnownHostMarkers Marker { get; set; }

        string Hostname { get; }

        string KeyType { get; }

        string PublicKey { get; }

        string FingerprintSha256 { get; }

        string? Comment { get; }

        bool MatchesHost(string host, string? keyType = null);

        bool MatchesHost(string host, int port, string? keyType = null);
    }
}
