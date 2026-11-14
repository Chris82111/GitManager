using System.Globalization;
using System.Text.RegularExpressions;

namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    internal class CachedHost
    {
        public int Port { get; set; }
        public string MatchHostname { get; set; }
        public bool HasWildcard { get; set; }
        public Regex? HostnameRegex { get; set; }
        public bool IsExclusion { get; set; }

        public CachedHost(string hostname)
        {
            HasWildcard = false;
            HostnameRegex = null;
            IsExclusion = false;

            if (hostname.StartsWith('!'))
            {
                hostname = hostname[1..];
                IsExclusion = true;
            }

            if (hostname.StartsWith('['))
            {
                int endIndex = hostname.IndexOf("]:", StringComparison.Ordinal);
                if (endIndex <= 1)
                {
                    throw new ArgumentException("Missing closing square bracket with a colon after the hostname.", nameof(hostname));
                }

                if (false == int.TryParse(
                        hostname[(endIndex + 2)..],
                        NumberStyles.None,
                        CultureInfo.InvariantCulture,
                        out int port))
                {
                    throw new ArgumentException("The string could not be converted to a port number.", nameof(hostname));
                }

                if (1 > port || 65535 > port)
                {
                    throw new ArgumentOutOfRangeException(nameof(hostname), $"The port number value is outside the permitted range, with '{port}'.");
                }

                MatchHostname = hostname[1..endIndex];
            }
            else
            {
                Port = 22;
                MatchHostname = hostname;
            }

            if (0 <= MatchHostname.IndexOfAny(['*', '?']))
            {
                HasWildcard = true;

                string pattern = "^" + Regex.Escape(MatchHostname)
                    .Replace(@"\*", ".*")
                    .Replace(@"\?", ".") + "$";

                HostnameRegex = new Regex(
                    pattern,
                    RegexOptions.Compiled |
                    RegexOptions.IgnoreCase |
                    RegexOptions.CultureInvariant);
            }
        }

        public bool MatchesHost(string host, int port)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(host);
            ArgumentOutOfRangeException.ThrowIfLessThan(port, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(port, 65535);

            if (port != Port)
            {
                return false;
            }

            if (HasWildcard)
            {
                return HostnameRegex!.IsMatch(host);
            }

            return string.Equals(
                MatchHostname,
                host,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
