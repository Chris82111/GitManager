using Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries;
using System.Diagnostics.CodeAnalysis;

namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts
{

    // https://man.openbsd.org/sshd.8?utm_source=chatgpt.com#SSH_KNOWN_HOSTS_FILE_FORMAT
    public static class KnownHostEntryFactory
    {
        public static List<IKnownHostEntry> Create(string[] lines)
        {
            ArgumentNullException.ThrowIfNull(lines);

            var result = new List<IKnownHostEntry>(lines.Length);
            
            foreach (var line in lines)
            {
                result.Add(Create(line));
            }

            ApplyRevokedMarkers(result);

            return result;
        }

        // Ensures that all revoked public keys also affect all other matching public keys.
        public static void ApplyRevokedMarkers(List<IKnownHostEntry> knownHostEntries)
        {
            ArgumentNullException.ThrowIfNull(knownHostEntries);

            var knownHosts = new List<IKnownHost>();

            var revokedFingerprints = new HashSet<string>();

            foreach (var knownHostEntry in knownHostEntries)
            {
                if (knownHostEntry is IKnownHost knownHost)
                {
                    knownHosts.Add(knownHost);
                    if (KnownHostMarkers.Revoked == knownHost.Marker)
                    {
                        revokedFingerprints.Add(knownHost.FingerprintSha256);
                    }
                }
            }

            if (0 == revokedFingerprints.Count)
            {
                return;
            }

            foreach (var knownHost in knownHosts)
            {
                if (revokedFingerprints.Contains(knownHost.FingerprintSha256))
                {
                    knownHost.Marker = KnownHostMarkers.Revoked;
                }
            }
        }

        public static IKnownHostEntry Create(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return new KnownHostEntryEmptyLine();
            }

            if (line.TrimStart().StartsWith('#'))
            {
                return new KnownHostEntryComment() { Comment = line };
            }

            return TryCreateKnownHost(line, out var knownHost, out _)
                ? knownHost
                : new KnownHostEntryUnknown() { Content = line };
        }

        private static bool TryCreateKnownHost(string line, [NotNullWhen(true)] out IKnownHost? knownHost, out Exception? exception)
        {
            knownHost = null;
            exception = null;

            if (string.IsNullOrWhiteSpace(line))
            {
                exception = new ArgumentNullException(nameof(line));
                return false;
            }

            string[] parts = line.Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries);

            var marker = KnownHostMarkersExtensions.ParseValue(parts[0]);
            int markerOffset = KnownHostMarkers.None != marker ? 1 : 0;

            if (parts.Length < 3 + markerOffset)
            {
                exception = new FormatException("Invalid known_hosts entry.");
                return false;
            }

            string hostname = parts[0 + markerOffset];
            string keyType = parts[1 + markerOffset];
            string publicKey = parts[2 + markerOffset];

            int commentStartIndex = 3 + markerOffset;
            string? comment = parts.Length > commentStartIndex
                ? string.Join(' ', parts, commentStartIndex, parts.Length - commentStartIndex)
                : null;

            if (hostname.StartsWith('|'))
            {
                if (hostname.StartsWith("|1|", StringComparison.Ordinal))
                {
                    var hostParts = hostname.Substring(3).Split('|');

                    if (hostParts.Length != 2)
                    {
                        exception = new FormatException(
                            $"Invalid hashed hostname format. Expected 2 parts, got {hostParts.Length}. With content '{string.Join(", ", hostParts)}'.");
                        return false;
                    }

                    if (string.IsNullOrEmpty(hostParts[0]) ||
                        string.IsNullOrEmpty(hostParts[1]))
                    {
                        exception = new FormatException(
                            $"Invalid hashed hostname format, salt: '{hostParts[0]}', hash: '{hostParts[1]}'.");
                        return false;
                    }

                    try
                    {
                        Convert.FromBase64String(hostParts[0]);
                        Convert.FromBase64String(hostParts[1]);
                    }
                    catch (FormatException ex)
                    {
                        exception = ex;
                        return false;
                    }

                    try
                    {
                        knownHost = new KnownHostHashed()
                        {
                            Marker = marker,
                            Salt = hostParts[0],
                            Hash = hostParts[1],
                            KeyType = keyType,
                            PublicKey = publicKey,
                            Comment = comment,
                        };

                        return true;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        return false;
                    }
                }
                else
                {
                    exception = new FormatException("Invalid hashed hostname format.");
                    return false;
                }
            }
           

            var hosts = hostname.Split(',');
            if (hosts.Any(string.IsNullOrWhiteSpace))
            {
                exception = new FormatException("Invalid hostname list.");
                return false;
            }
            else if (1 == hosts.Length)
            {

                try
                {
                    knownHost = new KnownHostSingle()
                    {
                        Marker = marker,
                        Hostname = hosts[0],
                        KeyType = keyType,
                        PublicKey = publicKey,
                        Comment = comment,
                    };

                    return true;
                }
                    catch (Exception ex)
                    {
                    exception = ex;
                    return false;
                }
            }
            else
            {
                try
                {
                    knownHost = new KnownHostMultiple()
                    {
                        Marker = marker,
                        Hostnames = hosts.ToList(),
                        KeyType = keyType,
                        PublicKey = publicKey,
                        Comment = comment,
                    };
                    return true;
                }
                catch (Exception ex)
                {
                    exception = ex;
                    return false;
                }
            }
        }
    }
}
