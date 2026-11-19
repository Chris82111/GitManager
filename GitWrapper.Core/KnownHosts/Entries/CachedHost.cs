using System.Globalization;
using System.Text.RegularExpressions;

namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    /// Stores parsed hostname matching information for reuse.
    /// </summary>
    internal class CachedHost
    {
        /// <summary>
        /// Gets or sets the port associated with the hostname.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the hostname used for matching.
        /// </summary>
        public string MatchHostname { get; set; }

        /// <summary>
        /// Gets or sets whether the hostname contains wildcard characters.
        /// </summary>
        public bool HasWildcard { get; set; }

        /// <summary>
        /// Gets or sets the compiled regular expression used for wildcard matching.
        /// </summary>
        public Regex? HostnameRegex { get; set; }

        /// <summary>
        /// Gets or sets whether the hostname represents an exclusion pattern.
        /// </summary>
        public bool IsExclusion { get; set; }

        /// <summary>
        ///         Initializes a new instance of the <see cref="CachedHost"/> class
        /// <br/>   by parsing the specified hostname.
        /// </summary>
        /// <param name="hostname">The hostname pattern to parse.</param>
        /// <exception cref="ArgumentNullException">The hostname is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The hostname contains an invalid port format.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified port is outside the permitted range.</exception>
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

                if (1 > port || 65535 < port)
                {
                    throw new ArgumentOutOfRangeException(nameof(hostname), $"The port number value is outside the permitted range, with '{port}'.");
                }

                MatchHostname = hostname[1..endIndex];
                Port = port;
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

        /// <summary>
        /// Determines whether the specified hostname and port match this cached entry.
        /// </summary>
        /// <param name="host">The hostname to match.</param>
        /// <param name="port">The port to match.</param>
        /// <returns>
        ///         <see langword="true"/>  if the hostname and port match,
        /// <br/>   <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">The hostname is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The hostname is empty or contains only whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The port is outside the permitted range.</exception>
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
