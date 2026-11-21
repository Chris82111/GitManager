using System.Collections.Concurrent;

namespace Chris82111.GitManager.GitWrapper.Core.Utilities
{
    /// <summary>
    ///         Provides a thread-safe multiton for <typeparamref name="TObject"/> instances.
    /// <br/>   A separate shared instance is maintained for each unique string.
    /// </summary>
    /// <remarks>
    ///         There is one limitation to be aware of: <see cref="Multiton{TObject}"/> does not
    /// <br/>   provide a disposal or removal mechanism. Consequently, all instances stored in
    /// <br/>   its internal collection remain referenced and available for the lifetime of the
    /// <br/>   application.
    /// </remarks>
    /// <typeparam name="TObject">The type of object maintained by the multiton.</typeparam>
    public class Multiton<TObject>
        where TObject : class
    {
        private sealed class Entry
        {
            public readonly SemaphoreSlim Semaphore = new(1, 1);

            public TObject? Value = null;
        }

        private static readonly ConcurrentDictionary<string, Entry> _entries =
            new(OperatingSystem.IsWindows()
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal);

        /// <summary>
        ///         Gets the shared <typeparamref name="TObject"/> instance for the specified string,
        /// <br/>   creating it if it does not already exist.
        /// </summary>
        /// <param name="factory">
        ///         The factory method used to create a new instance of <typeparamref name="TObject"/>.
        /// </param>
        /// <param name="filePath">
        ///         The path to the file. The path is converted to an absolute path
        /// <br/>   before looking up the shared instance.
        /// </param>
        /// <returns>
        /// Returns the shared
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="factory"/> or <paramref name="filePath"/> is null.</exception>
        /// <exception cref="ArgumentException">The specified file path is empty or contains invalid characters.</exception>
        /// <exception cref="IOException">An I/O error occurs while creating or reading the file.</exception>
        /// <exception cref="InvalidOperationException">The factory returns null.</exception>
        public static async Task<TObject> InstanceAsync(Func<string, Task<TObject>> factory, string filePath)
        {
            ArgumentNullException.ThrowIfNull(factory);
            ArgumentNullException.ThrowIfNull(filePath);

            string fullFilePath = Path.GetFullPath(filePath);

            Entry entry = _entries.GetOrAdd(
                fullFilePath,
                _ => new Entry());

            await entry.Semaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                if (null != entry.Value)
                {
                    return entry.Value;
                }

                entry.Value = await factory(fullFilePath).ConfigureAwait(false)
                    ?? throw new InvalidOperationException("The factory returned null.");

                return entry.Value;
            }
            finally
            {
                entry.Semaphore.Release();
            }
        }

        /// <inheritdoc cref="InstanceAsync(Func{string, Task{TObject}}, string)"/>
        public static TObject Instance(Func<string, TObject> factory, string filePath)
        {
            ArgumentNullException.ThrowIfNull(factory);
            ArgumentNullException.ThrowIfNull(filePath);

            string fullFilePath = Path.GetFullPath(filePath);

            Entry entry = _entries.GetOrAdd(
                fullFilePath,
                _ => new Entry());

            entry.Semaphore.Wait();

            try
            {
                if (null != entry.Value)
                {
                    return entry.Value;
                }

                entry.Value = factory(fullFilePath)
                    ?? throw new InvalidOperationException("The factory returned null.");

                return entry.Value;
            }
            finally
            {
                entry.Semaphore.Release();
            }
        }
    }
}
