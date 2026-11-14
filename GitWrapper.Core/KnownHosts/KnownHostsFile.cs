using Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries;
using Chris82111.GitManager.GitWrapper.Core.KnownHosts.Locking;
using System.Collections.Concurrent;
using System.Text;

namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts
{
    
    public class KnownHostsFile
    {
        private const int _defaultSshPort = 22;

        private static readonly ConcurrentDictionary<string, IAtomicReaderWriterLock> _locks = new();

        public string FilePath => _filePath;

        public List<IKnownHostEntry> KnownHostEntries { get; set; } = new List<IKnownHostEntry>();

        private readonly IAtomicReaderWriterLock _lockFile;

        private readonly IAtomicReaderWriterLock _lockList;

        private readonly string _filePath;

        public KnownHostsFile(string filePath, bool read = true)
        {
            ArgumentNullException.ThrowIfNull(filePath);

            _lockList = new AtomicReaderWriterLock();

            _filePath = Path.GetFullPath(filePath);

            _lockFile = GetLock(_filePath);

            if (read)
            {
                Read();
            }
        }

        public void Read()
        {
            string[] lines;

            _lockFile.EnterReadLock();
            try
            {
                lines = File.ReadAllLines(_filePath);
            }
            finally
            {
                _lockFile.ExitReadLock();
            }

            var knownHostEntries = KnownHostEntryFactory.Create(lines);

            _lockList.EnterWriteLock();
            try
            {
                KnownHostEntries = knownHostEntries;
            }
            finally
            {
                _lockList.ExitWriteLock();
            }

        }

        public async Task ReadAsync()
        {
            string[] lines;

            await _lockFile.EnterReadLockAsync();
            try
            {
                lines = await File.ReadAllLinesAsync(_filePath);
            }
            finally
            {
                _lockFile.ExitReadLock();
            }

            var knownHostEntries = KnownHostEntryFactory.Create(lines);

            await _lockList.EnterWriteLockAsync();
            try
            {
                KnownHostEntries = knownHostEntries;
            }
            finally
            {
                _lockList.ExitWriteLock();
            }

        }

        public bool IsFingerprintRevoked(string trustedFingerprint)
        {
            ArgumentNullException.ThrowIfNull(trustedFingerprint);

            _lockList.EnterReadLock();
            try
            {
                foreach (var knownHostEntry in KnownHostEntries)
                {
                    if (knownHostEntry is IKnownHost knownHost)
                    {
                        if (trustedFingerprint == knownHost.FingerprintSha256 && KnownHostMarkers.Revoked == knownHost.Marker)
                        {
                            return true;
                        }
                    }
                }
            }
            finally
            {
                _lockList.ExitReadLock();
            }

            return false;
        }

        public async Task<IKnownHost> VerifyAndAddHostAsync(
            string host,
            string keyType,
            string trustedFingerprint,
            TimeSpan? timeSpan = null,
            CancellationToken cancellationToken = default)
        {
            return await VerifyAndAddHostAsync(host, _defaultSshPort, keyType, trustedFingerprint, timeSpan, cancellationToken);
        }
        
        public async Task<IKnownHost> VerifyAndAddHostAsync(
            string host,
            int port,
            string keyType,
            string trustedFingerprint,
            TimeSpan? timeSpan = null,
            CancellationToken cancellationToken = default)
        {
            var knownHosts = FindHosts(host, port, keyType,
                knownHost => trustedFingerprint == knownHost.FingerprintSha256);

            IKnownHost? foundHost = null;
            if (0 < knownHosts.Count)
            {
                foundHost = knownHosts.First();

                // Ensure that there are no entries marked as "Revoked"
                if (KnownHostMarkers.Revoked == foundHost.Marker)
                {
                    throw new Exception("The fingerprint for the specified host has been revoked.");
                }
            }

            var unsafeHost = await ScanHostAsync(host, port, keyType, timeSpan, cancellationToken) ??
                throw new Exception("No host dates were received.");

            if (trustedFingerprint == unsafeHost.FingerprintSha256)
            {
                if (null != foundHost)
                {
                    return foundHost;
                }

                AddKnownHostEntry(unsafeHost);
                return unsafeHost;
            }

            throw new Exception($"The fingerprint received, '{unsafeHost.FingerprintSha256}', does not match the expected fingerprint, '{trustedFingerprint}'.");
        }


        // Only trusted host must be added
        // Does not check double entries
        public void AddKnownHostEntry(IKnownHostEntry entry)
        {
            ArgumentNullException.ThrowIfNull(entry);

            _lockList.EnterWriteLock();
            try
            {
                KnownHostEntries.Add(entry);
                KnownHostEntryFactory.ApplyRevokedMarkers(KnownHostEntries);
            }
            finally
            {
                _lockList.ExitWriteLock();
            }
        }


        // That is not revoked
        public async Task<bool> VerifyHostAsync(string host, string keyType)
        {
            return await VerifyHostAsync(host, _defaultSshPort, keyType);
        }

        // That is not revoked
        public async Task<bool> VerifyHostAsync(
            string host,
            int port,
            string keyType,
            TimeSpan? timeSpan = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(host);
            ArgumentNullException.ThrowIfNull(keyType);
            
            var hosts = FindHosts(host, port, keyType, 
                knownHost => KnownHostMarkers.None == knownHost.Marker);
            if (0 == hosts.Count)
            {
                return false;
            }

            var host2 = await ScanHostAsync(host, port, keyType, timeSpan, cancellationToken);
            if (null == host2)
            {
                return false;
            }

            foreach(var host1 in hosts)
            {
                return host1.FingerprintSha256 == host2.FingerprintSha256;
            }

            return false;
        }


        // timeSpan // The connection timeout. The default value is 30 seconds. 
        public async Task <KnownHostSingle?> ScanHostAsync(
            string host,
            string keyType,
            TimeSpan? timeSpan = null,
            CancellationToken cancellationToken = default)
        {
            return await SshHostScanner.ScanAsync(host, _defaultSshPort, keyType, timeSpan, cancellationToken);
        }
        
        public async Task<KnownHostSingle?> ScanHostAsync(
            string host,
            int port,
            string keyType,
            TimeSpan? timeSpan = null,
            CancellationToken cancellationToken = default)
        {
            return await SshHostScanner.ScanAsync(host, port, keyType, timeSpan, cancellationToken);
        }

        // Find all hosts
        // If you need none revoked hosts, use <c>knownHost => KnownHostMarkers.Revoked != knownHost.Marker</c>
        public List<IKnownHost> FindHosts(string host, string? keyType, Func<IKnownHost, bool>? predicate = null)
        {
            return FindHosts(host, _defaultSshPort, keyType, predicate);
        }

        public List<IKnownHost> FindHosts(string host, int port, string? keyType, Func<IKnownHost, bool>? predicate = null)
        {
            var result = new List<IKnownHost>();

            _lockList.EnterReadLock();
            try
            {
                if (null == predicate)
                {
                    foreach (var knownHostEntry in KnownHostEntries)
                    {
                        if (knownHostEntry is IKnownHost knownHost)
                        {
                            if (knownHost.MatchesHost(host, port, keyType))
                            {
                                result.Add(knownHost);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var knownHostEntry in KnownHostEntries)
                    {
                        if (knownHostEntry is IKnownHost knownHost)
                        {
                            if (predicate(knownHost) && knownHost.MatchesHost(host, port, keyType))
                            {
                                result.Add(knownHost);
                            }
                        }
                    }
                }
            }
            finally
            {
                _lockList.ExitReadLock();
            }

            return result;
        }


        public void EnterReadLock() => _lockFile.EnterReadLock();

        public void ExitReadLock() => _lockFile.ExitReadLock();

        public void EnterWriteLock() => _lockFile.EnterWriteLock();

        public void ExitWriteLock() => _lockFile.ExitWriteLock();


        public int RemoveHost(string host, string? keyType)
        {
            return RemoveHost(host, _defaultSshPort, keyType);
        }

        public int RemoveHost(string host, int port, string? keyType)
        {
            ArgumentNullException.ThrowIfNull(host);

            int removed = 0;

            _lockList.EnterWriteLock();
            try
            {
                for (int i = 0; i < KnownHostEntries.Count;)
                {
                    var knownHostEntry = KnownHostEntries[i];
                    if (knownHostEntry is IKnownHost knownHost)
                    {
                        if (knownHost.MatchesHost(host, port, keyType))
                        {
                            KnownHostEntries.RemoveAt(i);
                            removed++;
                            continue;
                        }
                    }
                    i++;
                }
                
            }
            finally
            {
                _lockList.ExitWriteLock();
            }

            return removed;
        }

        public int RemoveFingerprint(string fingerprint)
        {
            ArgumentNullException.ThrowIfNull(fingerprint);

            int removed = 0;

            _lockList.EnterWriteLock();
            try
            {
                for (int i = 0; i < KnownHostEntries.Count;)
                {
                    var knownHostEntry = KnownHostEntries[i];
                    if (knownHostEntry is IKnownHost knownHost)
                    {
                        if (fingerprint == knownHost.FingerprintSha256)
                        {
                            KnownHostEntries.RemoveAt(i);
                            removed++;
                            continue;
                        }
                    }
                    i++;
                }
            }
            finally
            {
                _lockList.ExitWriteLock();
            }

            return removed;
        }


        public async Task WriteAsync(CancellationToken cancellationToken = default)
        {
            string? tempFile = null;


            var lines = new List<string>();

            await _lockList.EnterReadLockAsync(cancellationToken);
            try
            {
                foreach (var knownHostEntry in KnownHostEntries)
                {
                    var line = knownHostEntry.ToString();
                    if (null != line)
                    {
                        lines.Add(line);
                    }
                }
            }
            finally
            {
                _lockList.ExitReadLock();
            }

            await _lockFile.EnterWriteLockAsync(cancellationToken);
            try
            {
                string? directory = Path.GetDirectoryName(_filePath);

                if (directory != null)
                {
                    Directory.CreateDirectory(directory);
                }

                tempFile = _filePath + "." +
                    Guid.NewGuid().ToString("N") + ".tmp";

                await File.WriteAllLinesAsync(
                    tempFile,
                    lines,
                    new UTF8Encoding(false),
                    cancellationToken);

                File.Move(tempFile, _filePath, true);
            }
            finally
            {
                _lockFile.ExitWriteLock();

                if (null != tempFile)
                {
                    File.Delete(tempFile);
                }
            }
        }

        public static IAtomicReaderWriterLock GetLock(string filePath)
        {
            string fullPath = Path.GetFullPath(filePath);

            return _locks.GetOrAdd(
                fullPath,
                static _ => new AtomicReaderWriterLock());
        }
    }
}
