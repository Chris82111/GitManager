namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    /// Represents single entry from a known_hosts file.
    /// </summary>
    /// <remarks>
    /// A entry can contains: <c>github.com or *.github.com or git??b.com or [github.com]:22</c>
    /// </remarks>
    public sealed class KnownHostSingle : KnownHostBase
    {
        private string _hostname = null!;

        /// <inheritdoc />
        public required override string Hostname
        {
            get => _hostname;
            init
            {
                _hostname = value;
                _cashedRegexHost = new CachedHost(value);
            }
        }

        private CachedHost _cashedRegexHost = null!;

        /// <inheritdoc />
        public override bool MatchesHost(string host, int port, string? keyType = null)
        {
            if (null != keyType && keyType != KeyType)
            {
                return false;
            }

            return _cashedRegexHost.MatchesHost(host, port);
        }        
    }
}
