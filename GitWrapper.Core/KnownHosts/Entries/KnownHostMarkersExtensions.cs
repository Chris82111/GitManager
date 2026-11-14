namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    public static class KnownHostMarkersExtensions
    {
        public static string ToValue(this KnownHostMarkers marker)
        {
            switch (marker)
            {
                case KnownHostMarkers.CertAuthority:
                    return "@cert-authority";

                case KnownHostMarkers.Revoked:
                    return "@revoked";

                default:
                    return string.Empty;
            }
        }

        public static KnownHostMarkers ParseValue(string? marker)
        {
            if (null == marker || false == marker.StartsWith("@", StringComparison.Ordinal))
            {
                return KnownHostMarkers.None;
            }

            switch (marker)
            {
                case "@cert-authority":
                    return KnownHostMarkers.CertAuthority;

                case "@revoked":
                    return KnownHostMarkers.Revoked;

                default:
                    throw new ArgumentException(
                        $"Unknown known_hosts marker '{marker}'.",
                        nameof(marker));
            }
        }

        public static bool TryParseValue(string? marker, out KnownHostMarkers? knownHostMarkers)
        {
            if (null == marker || false == marker.StartsWith("@", StringComparison.Ordinal))
            {
                knownHostMarkers = KnownHostMarkers.None;
                return true;
            }

            switch (marker)
            {
                case "@cert-authority":
                    knownHostMarkers = KnownHostMarkers.CertAuthority;
                    return true;

                case "@revoked":
                    knownHostMarkers = KnownHostMarkers.Revoked;
                    return true;

                default:
                    knownHostMarkers = null;
                    return false;
            }
        }
    }
}
