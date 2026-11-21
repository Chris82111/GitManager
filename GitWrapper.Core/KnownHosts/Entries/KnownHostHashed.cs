using System.Security.Cryptography;
using System.Text;

namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    /// Represents an encrypted entry in a known_hosts file.
    /// </summary>
    public sealed class KnownHostHashed : KnownHostBase
    {
        private string _salt = null!;

        /// <summary>
        /// The Base64-encoded salt used to hash the hostname.
        /// </summary>
        /// <value>
        /// The Base64-encoded salt.
        /// </value>
        public required string Salt
        {
            get => _salt;
            init
            {
                _salt = value;
                _saltByte = Convert.FromBase64String(value);
            }
        }

        private string _hash = null!;

        /// <summary>
        /// The Base64-encoded SHA-1 hash of the hostname.
        /// </summary>
        /// <value>
        /// The Base64-encoded hostname hash.
        /// </value>
        public required string Hash
        {
            get => _hash;
            init
            {
                _hash = value;
                _hashByte = Convert.FromBase64String(value);
            }
        }

        /// <inheritdoc />
        public override string Hostname
        {
            get => $"|1|{Salt}|{Hash}";
            init { }
        }

        private byte[] _saltByte = null!;
        private byte[] _hashByte = null!;

        /// <inheritdoc />
        public override bool MatchesHost(string host, int port, string? keyType = null)
        {
            byte[] hostBytes;
            byte[] actualHash;

            if (22 == port)
            {
                hostBytes = Encoding.UTF8.GetBytes(host);
                actualHash = HMACSHA1.HashData(_saltByte, hostBytes);

                if (CryptographicOperations.FixedTimeEquals(actualHash, _hashByte))
                {
                    return true;
                }
            }

            hostBytes = Encoding.UTF8.GetBytes($"[{host}]:{port}");
            actualHash = HMACSHA1.HashData(_saltByte, hostBytes);

            return CryptographicOperations.FixedTimeEquals(actualHash, _hashByte);
        }
    }
}
