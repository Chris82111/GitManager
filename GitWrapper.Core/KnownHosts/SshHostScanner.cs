using Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts
{
    /// <summary>
    /// Provides functionality for scanning SSH hosts and retrieving their public host keys.
    /// </summary>
    /// <remarks>
    ///         Requires the SSH.NET NuGet package.
    /// <br/>   Install using: <c>dotnet add package SSH.NET</c>
    /// </remarks>
    public class SshHostScanner
    {
        /// <summary>
        /// Scans an SSH host and retrieves its public host key without authenticating.
        /// </summary>
        /// <param name="host">The hostname to scan.</param>
        /// <param name="port">The SSH port. Defaults to 22.</param>
        /// <param name="algorithm">The optional host-key algorithm to use.</param>
        /// <param name="timeSpan">The optional connection timeout. Defaults to 30 seconds.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The retrieved host key.</returns>
        /// <exception cref="NotSupportedException">The specified host-key algorithm is not supported.</exception>
        /// <exception cref="InvalidOperationException">The host key could not be obtained.</exception>
        public static async Task<KnownHostSingle> ScanAsync(
            string host,
            int port = 22,
            string? algorithm = null,
            TimeSpan? timeSpan = null, // The connection timeout. The default value is 30 seconds.
            CancellationToken cancellationToken = default)
        {
            var connectionInfo = new ConnectionInfo(
                host, port, "ssh-keyscan", new NoneAuthenticationMethod("ssh-keyscan"));

            if (timeSpan.HasValue)
            {
                connectionInfo.Timeout = timeSpan.Value;
            }

            // Optionally restrict the host-key algorithm.
            if (null != algorithm)
            {
                if (false == connectionInfo.HostKeyAlgorithms.TryGetValue(algorithm, out var factory))
                {
                    throw new NotSupportedException(
                        $"Unsupported host-key algorithm: {algorithm}");
                }

                connectionInfo.HostKeyAlgorithms.Clear();
                connectionInfo.HostKeyAlgorithms.Add(algorithm, factory);
            }

            KnownHostSingle? result = null;

            using var client = new SshClient(connectionInfo);

            client.HostKeyReceived += (_, e) =>
            {
                result = new KnownHostSingle()
                {
                    Marker = KnownHostMarkers.None,
                    Hostname = host,
                    KeyType = e.HostKeyName,
                    PublicKey = Convert.ToBase64String(e.HostKey),
                };

                // Host key rejection: ~450 ms; acceptance: ~750 ms.
                e.CanTrust = false;
            };

            try
            {
                await client.ConnectAsync(cancellationToken);
            }
            catch (SshAuthenticationException) when (result is not null)
            {
                // Expected: connection rejected after receiving the host key.
            }
            catch (SshConnectionException) when (result is not null)
            {
                // Depending on SSH.NET version, rejecting the host key
                // may result in SshConnectionException instead.
            }

            return result ??
                throw new InvalidOperationException("Result could not be obtained.");
        }
    }
}
