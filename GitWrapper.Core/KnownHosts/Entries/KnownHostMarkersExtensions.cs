namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    /// Provides extension and conversion methods for <see cref="KnownHostMarkers"/>.
    /// </summary>
    public static class KnownHostMarkersExtensions
    {
        /// <summary>
        /// Converts the specified marker into its known_hosts string representation.
        /// </summary>
        /// <param name="marker">The marker to convert.</param>
        /// <returns>
        ///         The corresponding marker string, or 
        /// <br/>   <see cref="string.Empty"/> if the marker is <see cref="KnownHostMarkers.None"/> or has no known representation.
        /// </returns>
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

        /// <summary>
        /// Parses a known_hosts marker string into its corresponding enum value.
        /// </summary>
        /// <param name="marker">The marker string to parse.</param>
        /// <returns>
        ///         The corresponding <see cref="KnownHostMarkers"/> value, or
        /// <br/>   <see cref="KnownHostMarkers.None"/> if the input is <see langword="null"/> or does not start with <c>@</c>.
        /// </returns>
        /// <exception cref="ArgumentException">The marker starts with <c>@</c> but is not recognized.</exception>
        public static KnownHostMarkers ParseValue(string? marker)
        {
            if (null == marker || false == marker.StartsWith('@'))
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

        /// <summary>
        /// Attempts to parse a known_hosts marker string into its corresponding enum value.
        /// </summary>
        /// <param name="marker">The marker string to parse.</param>
        /// <param name="knownHostMarkers">
        ///         When successful, contains the parsed marker or <see cref="KnownHostMarkers.None"/> if the input is <see langword="null"/> or does not start with <c>@</c>
        /// <br/>   <see langword="null"/> otherwise.
        /// </param>
        /// <returns>
        ///         <see langword="true"/>  if the marker was successfully parsed,
        /// <br/>   <see langword="false"/> otherwise.
        /// </returns>
        public static bool TryParseValue(string? marker, out KnownHostMarkers? knownHostMarkers)
        {
            if (null == marker || false == marker.StartsWith('@'))
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
