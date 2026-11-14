using System.Security.Cryptography;
using System.Text;

namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    public abstract class KnownHostBase : KnownHostEntryBase, IKnownHost
    {
        public KnownHostMarkers Marker { get; set; } = KnownHostMarkers.None;

        public abstract string Hostname { get; init; }

        public required string KeyType { get; init; }

        private string _publicKey = string.Empty;

        // publicKeyBase64
        public required string PublicKey
        {
            get => _publicKey;
            init
            {
                _publicKey = value;
                FingerprintSha256 = CalculateFingerprintSha256(value);
            }
        }

        public string? Comment { get; init; }

        // Fingerprint is string.Empty if no hash can be calculated from PublicKey
        public string FingerprintSha256 { get; private set; } = string.Empty;
        
        public virtual bool MatchesHost(string host, string? keyType = null)
        {
            return MatchesHost(host, 22, keyType);
        }

        public abstract bool MatchesHost(string host, int port, string? keyType = null);

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
