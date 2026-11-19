using System.Collections.Concurrent;

namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts
{
    /// <summary>
    ///         Provides a thread-safe multiton for <see cref="KnownHostsFile"/> instances.
    /// <br/>   A separate shared instance is maintained for each unique file path.
    /// </summary>
    public static class KnownHostMultiton
    {
        private sealed class Entry
        {
            public readonly object Sync = new();

            public bool IsValid = false;

            public bool IsCreating = false;

            public KnownHostsFile? Value = null;
        }

        private static readonly ConcurrentDictionary<string, Entry> KnownHostsFiles =
            new(OperatingSystem.IsWindows()
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal);

        /// <summary>
        ///         Gets the shared <see cref="KnownHostsFile"/> instance for the specified file path,
        /// <br/>   creating it asynchronously if it does not already exist.
        /// </summary>
        /// <param name="filePath">
        ///         The path to the known_hosts file. The path is converted to an absolute path
        /// <br/>   before looking up the shared instance.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result is the shared
        /// <see cref="KnownHostsFile"/> instance associated with the specified file path.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null/>.</exception>
        /// <exception cref="ArgumentException">The specified file path is empty or contains invalid characters.</exception>
        /// <exception cref="IOException">An I/O error occurs while creating or reading the known_hosts file.</exception>
        public async static Task<KnownHostsFile> InstanceAsync(string filePath)
        {
            var fullFilePath = Path.GetFullPath(filePath);

            Entry entry = KnownHostsFiles.GetOrAdd(fullFilePath, _ => new Entry());

            lock (entry.Sync)
            {
                while (entry.IsCreating)
                {
                    Monitor.Wait(entry.Sync);
                }

                if (entry.IsValid && null != entry.Value)
                {
                    return entry.Value;
                }

                // Either this is the first construction
                // or the previous construction failed.
                entry.IsValid = false;
                entry.IsCreating = true;
            }

            KnownHostsFile? instance = null;

            try
            {
                instance = await KnownHostsFile.CreateAsync(fullFilePath);
            }
            finally
            {
                lock (entry.Sync)
                {
                    entry.Value = null;
                    entry.IsValid = true;
                    entry.IsCreating = false;

                    Monitor.PulseAll(entry.Sync);
                }
            }

            lock (entry.Sync)
            {
                entry.Value = instance;
                entry.IsValid = true;
                entry.IsCreating = false;

                Monitor.PulseAll(entry.Sync);
            }

            return instance;
        }

        /// <summary>
        ///         Gets the shared <see cref="KnownHostsFile"/> instance for the specified file path,
        /// <br/>   creating it synchronously if it does not already exist.
        /// </summary>
        /// <param name="filePath">
        ///         The path to the known_hosts file. The path is converted to an absolute path
        /// <br/>   before looking up the shared instance.
        /// </param>
        /// <returns>
        /// The shared <see cref="KnownHostsFile"/> instance associated with the specified file path.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null/>.</exception>
        /// <exception cref="ArgumentException">The specified file path is empty or contains invalid characters.</exception>
        /// <exception cref="IOException">An I/O error occurs while creating or reading the known_hosts file.</exception>
        public static KnownHostsFile Instance(string filePath)
        {
            var fullFilePath = Path.GetFullPath(filePath);

            Entry entry = KnownHostsFiles.GetOrAdd(fullFilePath, _ => new Entry());

            lock (entry.Sync)
            {
                while (entry.IsCreating)
                {
                    Monitor.Wait(entry.Sync);
                }

                if (entry.IsValid && null != entry.Value)
                {
                    return entry.Value;
                }

                // Either this is the first construction
                // or the previous construction failed.
                entry.IsValid = false;
                entry.IsCreating = true;
            }

            KnownHostsFile? instance = null;

            try
            {
                instance = KnownHostsFile.Create(fullFilePath);
            }
            finally
            {
                lock (entry.Sync)
                {
                    entry.Value = null;
                    entry.IsValid = true;
                    entry.IsCreating = false;

                    Monitor.PulseAll(entry.Sync);
                }
            }

            lock (entry.Sync)
            {
                entry.Value = instance;
                entry.IsValid = true;
                entry.IsCreating = false;

                Monitor.PulseAll(entry.Sync);
            }

            return instance;
        }
    }
}
