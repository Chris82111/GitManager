using Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries;
using Chris82111.GitManager.GitWrapper.Core.KnownHosts.Locking;
using System.Collections.Concurrent;
using System.Text;

namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts
{
    /// <summary>
    ///         Manages SSH known host entries, including reading, writing, adding,
    /// <br/>   removing and verifying trusted host fingerprints.
    /// <br/>   Provides thread-safe access and file synchronization.
    /// </summary>
    /// <remarks>
    ///         If this class is not used, the commands must be entered manually.
    /// <br/>   Provides commands to search for hosts and remove host entries or
    /// <br/>   specific public keys from SSH known_hosts files.
    /// <para>
    /// <b>1. Search for hosts</b>
    /// </para>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Description</term>
    ///         <description>Command</description>
    ///     </listheader>
    ///     <item>
    ///         <term>Search for a host in a specific file</term>
    ///         <description><c>ssh-keygen -F github.com -f ~/.ssh/known_hosts</c></description>
    ///     </item>
    ///     <item>
    ///         <term>Search in the default user known_hosts file</term>
    ///         <description><c>ssh-keygen -F github.com</c></description>
    ///     </item>
    ///     <item>
    ///         <term>Search in a custom known_hosts file</term>
    ///         <description><c>ssh-keygen -F github.com -f ./github_known_hosts</c></description>
    ///     </item>
    ///     <item>
    ///         <term>Check if a specific key type exists</term>
    ///         <description><c>ssh-keygen -F github.com -f ./github_known_hosts | grep 'ssh-ed25519'</c></description>
    ///     </item>
    /// </list>
    /// <para>
    /// <b>2. Remove hosts and keys</b>
    /// </para>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Description</term>
    ///         <description>Command</description>
    ///     </listheader>
    ///     <item>
    ///         <term>Remove a host from the user known_hosts file</term>
    ///         <description><c>ssh-keygen -R github.com -f ~/.ssh/known_hosts</c></description>
    ///     </item>
    ///     <item>
    ///         <term>Remove a host from a custom file</term>
    ///         <description><c>ssh-keygen -R github.com -f ./github_known_hosts</c></description>
    ///     </item>
    ///     <item>
    ///         <term>Remove all entries for a host</term>
    ///         <description><c>ssh-keygen -R github.com -f ./github_known_hosts</c></description>
    ///     </item>
    /// </list>
    /// </remarks>
    public class KnownHostsFile
    {
        /// <summary>
        /// The path to the known_hosts file
        /// </summary>
        public string FilePath => _filePath;

        /// <summary>
        /// Individual entries of the known hosts file
        /// </summary>
        public List<IKnownHostEntry> KnownHostEntries { get; set; } = new List<IKnownHostEntry>();

        private const int _defaultSshPort = 22;

        private readonly IAtomicReaderWriterLock _lockFile;

        private readonly IAtomicReaderWriterLock _lockList;

        private readonly string _filePath;

        private readonly struct SkipRead;

        /// <summary>
        ///         Creates a new class to access the individual entries in a known_hosts file,
        /// <br/>   with an synchronous read
        /// </summary>
        /// <param name="filePath">The path to the known_hosts file</param>
        public KnownHostsFile(string filePath) : this(filePath, new SkipRead())
        {
            Read();
        }

        /// <summary>
        ///         Creates a new class to access the individual entries in a known_hosts file,
        /// <br/>   without reading the file's contents
        /// </summary>
        /// <param name="filePath">The path to the known_hosts file</param>
        /// <param name="_">A dummy structure with no content, used to change the constructor’s signature</param>
        private KnownHostsFile(string filePath, SkipRead _)
        {
            ArgumentNullException.ThrowIfNull(filePath);

            _lockList = new AtomicReaderWriterLock();

            _filePath = Path.GetFullPath(filePath);

            _lockFile = GetLock(_filePath);
        }

        /// <summary>
        ///         Creates a new class to access the individual entries in a known_hosts file,
        /// <br/>   with an asynchronous read
        /// </summary>
        /// <param name="filePath">The path to the known_hosts file</param>
        /// <returns>The newly created object</returns>
        public static async Task<KnownHostsFile> CreateAsync(string filePath)
        {
            var instance = new KnownHostsFile(filePath, new SkipRead());

            await instance.ReadAsync().ConfigureAwait(false);

            return instance;
        }

        /// <summary>
        ///         Creates a new class to access the individual entries in a known_hosts file,
        /// <br/>   with an synchronous read
        /// </summary>
        /// <param name="filePath">The path to the known_hosts file</param>
        /// <returns>The newly created object</returns>
        public static KnownHostsFile Create(string filePath)
        {
            return new KnownHostsFile(filePath); ;
        }

        /// <summary>
        ///         Reads the known_hosts file and overwrites all existing entries in <see cref="KnownHostEntries"/>.
        /// <br/>   The read is synchronous.
        /// </summary>
        public void Read()
        {
            string[] lines;

            _lockFile.EnterReadLock();
            try
            {
                using (File.Open(_filePath, FileMode.OpenOrCreate))
                {
                }

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

        /// <summary>
        ///         Reads the known_hosts file and overwrites all existing entries in <see cref="KnownHostEntries"/>.
        /// <br/>   The read is asynchronous.
        /// </summary>
        public async Task ReadAsync()
        {
            var lines = new List<string>();

            await _lockFile.EnterReadLockAsync();
            try
            {
                using FileStream stream = File.Open(_filePath, FileMode.OpenOrCreate);
                using StreamReader reader = new(stream);

                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                }
            }
            finally
            {
                _lockFile.ExitReadLock();
            }

            var knownHostEntries = KnownHostEntryFactory.Create(lines.ToArray());

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

        ///  <summary>
        /// Determines whether a trusted SSH fingerprint is marked as revoked.
        /// </summary>
        /// <param name="trustedFingerprint">The SHA-256 fingerprint to check.</param>
        /// <returns>
        ///         true:   If the fingerprint is marked as revoked.
        /// <br/>   false:  If the fingerprint is not marked as revoked.
        /// </returns>
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

        /// <inheritdoc cref="VerifyAndAddHostAsync(string, int, string, string, TimeSpan?, CancellationToken)"/>
        public async Task<IKnownHost> VerifyAndAddHostAsync(
            string host,
            string keyType,
            string trustedFingerprint,
            TimeSpan? timeSpan = null,
            CancellationToken cancellationToken = default)
        {
            return await VerifyAndAddHostAsync(host, _defaultSshPort, keyType, trustedFingerprint, timeSpan, cancellationToken);
        }

        /// <summary>
        /// Verifies a host's SSH fingerprint and adds it to the known hosts if it is not already trusted.
        /// </summary>
        /// <param name="host">The hostname to verify.</param>
        /// <param name="port">The SSH port.</param>
        /// <param name="keyType">The SSH key type.</param>
        /// <param name="trustedFingerprint">The expected SHA-256 fingerprint.</param>
        /// <param name="timeSpan">The optional timeout for scanning the host.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// The verified and trusted host entry.
        /// </returns>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// Adds a known host entry and applies revoked markers to the entries.
        /// </summary>
        /// <remarks>
        ///         Only trusted hosts must be added.
        /// <br/>   Does not check for duplicate entries.
        /// </remarks>
        /// <param name="entry">The known host entry to add.</param>
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


        /// <inheritdoc cref="VerifyHostAsync(string, int, string, TimeSpan?, CancellationToken)"/>
        public async Task<bool> VerifyHostAsync(string host, string keyType)
        {
            return await VerifyHostAsync(host, _defaultSshPort, keyType);
        }

        /// <summary>
        /// Verifies a host against trusted known host entries.
        /// </summary>
        /// <remarks>
        /// Only hosts that are not marked as revoked are considered.
        /// </remarks>
        /// <param name="host">The hostname to verify.</param>
        /// <param name="port">The SSH port.</param>
        /// <param name="keyType">The SSH key type.</param>
        /// <param name="timeSpan">The optional timeout for scanning the host.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        ///         true:   If the scanned host fingerprint matches a trusted, non-revoked entry.
        /// <br/>   false:  If no matching trusted entry is found or the host cannot be scanned.
        /// </returns>
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

#pragma warning disable CA1822 // Replace with the actual warning ID

        /// <inheritdoc cref="SshHostScanner.ScanAsync(string, int, string?, TimeSpan?, CancellationToken)"/>
        public async Task <KnownHostSingle?> ScanHostAsync(
            string host,
            string keyType,
            TimeSpan? timeSpan = null,
            CancellationToken cancellationToken = default)
        {
            return await SshHostScanner.ScanAsync(host, _defaultSshPort, keyType, timeSpan, cancellationToken);
        }

        /// <inheritdoc cref="SshHostScanner.ScanAsync(string, int, string?, TimeSpan?, CancellationToken)"/>
        public async Task<KnownHostSingle?> ScanHostAsync(
            string host,
            int port,
            string keyType,
            TimeSpan? timeSpan = null,
            CancellationToken cancellationToken = default)
        {
            return await SshHostScanner.ScanAsync(host, port, keyType, timeSpan, cancellationToken);
        }

#pragma warning restore CA1822

        /// <inheritdoc cref="FindHosts(string, int, string?, Func{IKnownHost, bool}?)"/>
        public List<IKnownHost> FindHosts(string host, string? keyType, Func<IKnownHost, bool>? predicate = null)
        {
            return FindHosts(host, _defaultSshPort, keyType, predicate);
        }

        /// <summary>
        /// Finds known host entries matching the specified host, port and optional key type.
        /// </summary>
        /// <remarks>
        /// To exclude revoked hosts, use: <c>knownHost => KnownHostMarkers.Revoked != knownHost.Marker</c>.
        /// </remarks>
        /// <param name="host">The hostname to search for.</param>
        /// <param name="port">The SSH port.</param>
        /// <param name="keyType">The optional SSH key type filter.</param>
        /// <param name="predicate">An optional predicate to filter matching host entries.</param>
        /// <returns>
        /// A list of matching known host entries.
        /// </returns>
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

        /// <summary>
        /// Enters a read lock.
        /// </summary>
        /// <remarks>
        ///         Each call to <see cref="EnterReadLock"/> must be followed
        /// <br/>   by a corresponding call to <see cref="ExitReadLock"/>.
        /// <br/>   Use a <c>try-finally</c> structure to ensure the lock is always released.
        /// </remarks>
        public void EnterReadLock() => _lockFile.EnterReadLock();

        /// <summary>
        /// Exits a read lock.
        /// </summary>
        /// <remarks>
        ///         Each call of <see cref="ExitReadLock"/> must follow
        /// <br/>   a corresponding call to <see cref="EnterReadLock"/>.
        /// </remarks>
        public void ExitReadLock() => _lockFile.ExitReadLock();

        /// <summary>
        /// Enters a write lock.
        /// </summary>
        /// <remarks>
        ///         Each call to <see cref="EnterWriteLock"/> must be followed
        /// <br/>   by a corresponding call to <see cref="ExitWriteLock"/>.
        /// <br/>   Use a <c>try-finally</c> structure to ensure the lock is always released.
        /// </remarks>
        public void EnterWriteLock() => _lockFile.EnterWriteLock();

        /// <summary>
        /// Exits a write lock.
        /// </summary>
        /// <remarks>
        ///         Each call to <see cref="ExitWriteLock"/> must follow
        /// <br/>   a corresponding call to <see cref="EnterWriteLock"/>.
        /// </remarks>
        public void ExitWriteLock() => _lockFile.ExitWriteLock();

        /// <inheritdoc cref="RemoveHost(string, int, string?)"/>
        public int RemoveHost(string host, string? keyType)
        {
            return RemoveHost(host, _defaultSshPort, keyType);
        }

        /// <summary>
        /// Removes all known host entries matching the specified host, port and optional key type.
        /// </summary>
        /// <param name="host">The hostname to remove.</param>
        /// <param name="port">The SSH port.</param>
        /// <param name="keyType">The optional SSH key type filter.</param>
        /// <returns>
        /// The number of removed known host entries.
        /// </returns>
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

        /// <summary>
        /// Removes all known host entries matching the specified SHA-256 fingerprint.
        /// </summary>
        /// <param name="fingerprint">The SHA-256 fingerprint to remove.</param>
        /// <returns>
        /// The number of removed known host entries.
        /// </returns>
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

        /// <summary>
        ///         Asynchronously writes all known host entries to the file.
        /// <br/>   Uses a temporary file and atomic replacement to avoid partially written files.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
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

        /// <summary>
        /// Stores shared atomic reader-writer locks indexed by their full file paths.
        /// </summary>
        private static readonly ConcurrentDictionary<string, IAtomicReaderWriterLock> _locks = new();
        
        /// <summary>
        /// Gets or creates a shared lock for the specified file path.
        /// </summary>
        /// <param name="filePath">The file path associated with the lock.</param>
        /// <returns>
        /// The shared atomic reader-writer lock for the specified file.
        /// </returns>
        private static IAtomicReaderWriterLock GetLock(string filePath)
        {
            string fullPath = Path.GetFullPath(filePath);

            return _locks.GetOrAdd(
                fullPath,
                static _ => new AtomicReaderWriterLock());
        }
    }
}
