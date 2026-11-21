namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    /// Represents multiple entries from a known_hosts file.
    /// </summary>
    /// <remarks>
    /// A entry can contains: <c>github.com,github.org,*.github.com,git??b.com,!git?ab.com,[github.com]:22</c>
    /// </remarks>
    public sealed class KnownHostMultiple : KnownHostBase
    {
        private List<string> _hostnames = null!;

        /// <summary>
        /// Contains the individual hosts as a list
        /// </summary>
        public required List<string> Hostnames
        {
            get => _hostnames;
            init
            {
                _hostnames = value;
                foreach (var host in value)
                {
                    var cashedRegexHost = new CachedHost(host);

                    (cashedRegexHost.IsExclusion ? _excludeHostnames : _includeHostnames)
                        .Add(cashedRegexHost);
                }
            }
        }

        /// <inheritdoc />
        public override string Hostname
        {
            get => string.Join(',', Hostnames);
            init { }
        }

        private List<CachedHost> _includeHostnames = new List<CachedHost>();

        private List<CachedHost> _excludeHostnames = new List<CachedHost>();

        /// <inheritdoc />
        public override bool MatchesHost(string host, int port, string? keyType = null)
        {
            if (null != keyType && keyType != KeyType)
            {
                return false;
            }

            bool matched = false;
            foreach (var hostname in _includeHostnames)
            {
                if(hostname.MatchesHost(host, port))
                {
                    matched = true;
                    break;
                }
            }

            if (false == matched)
            {
                return false;
            }

            foreach (var hostname in _excludeHostnames)
            {
                if (hostname.MatchesHost(host, port))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
