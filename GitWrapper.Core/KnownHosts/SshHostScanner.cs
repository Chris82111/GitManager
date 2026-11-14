using Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries;
using Renci.SshNet;
using Renci.SshNet.Common;


namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts
{
    // install: dotnet add package SSH.NET
    public class SshHostScanner
    {
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
