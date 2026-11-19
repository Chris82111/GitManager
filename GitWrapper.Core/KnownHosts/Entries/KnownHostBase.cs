using System.Security.Cryptography;
using System.Text;

namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    ///         Provides a base implementation of <see cref="IKnownHost"/>, with virtual and
    /// <br/>   abstract members for representing and matching known SSH hosts.
    /// </summary>
    public abstract class KnownHostBase : KnownHostEntryBase, IKnownHost
    {
        /// <inheritdoc />
        public KnownHostMarkers Marker { get; set; } = KnownHostMarkers.None;

        /// <inheritdoc />
        public abstract string Hostname { get; init; }

        /// <inheritdoc />
        public required string KeyType { get; init; }

        private string _publicKey = string.Empty;

        /// <inheritdoc />
        public required string PublicKey
        {
            get => _publicKey;
            init
            {
                _publicKey = value;
                FingerprintSha256 = CalculateFingerprintSha256(value);
            }
        }

        /// <inheritdoc />
        public string? Comment { get; init; }

        /// <inheritdoc />
        public string FingerprintSha256 { get; private set; } = string.Empty;

        /// <inheritdoc />
        public virtual bool MatchesHost(string host, string? keyType = null)
        {
            return MatchesHost(host, 22, keyType);
        }

        /// <inheritdoc />
        public abstract bool MatchesHost(string host, int port, string? keyType = null);

        /// <inheritdoc />
        public override string Serialize()
        {
            var str = new StringBuilder();

            if (KnownHostMarkers.None != Marker)
            {
                str.Append(Marker.ToValue());
                str.Append(' ');
            }

            str.Append(Hostname);
            str.Append(' ');
            str.Append(KeyType);
            str.Append(' ');
            str.Append(PublicKey);

            if (false == string.IsNullOrEmpty(Comment))
            {
                str.Append(' ');
                str.Append(Comment);
            }

            return str.ToString();
        }

        /// <summary>
        /// Calculates the SHA-256 fingerprint of the specified Base64-encoded public key.
        /// </summary>
        /// <param name="publicKey">The Base64-encoded public key.</param>
        /// <returns>
        ///         The SHA-256 fingerprint in Base64 format without trailing padding characters,
        /// <br/>   or <see langword="string.Empty"/> if the fingerprint could not be calculated.
        /// </returns>
        public static string CalculateFingerprintSha256(string publicKey)
        {
            if (string.IsNullOrWhiteSpace(publicKey))
            {
                return string.Empty;
            }

            try
            {
                byte[] keyBytes = Convert.FromBase64String(publicKey);

                byte[] hash = SHA256.HashData(keyBytes);

                return Convert.ToBase64String(hash).TrimEnd('=');
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
