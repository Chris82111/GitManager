namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Locking
{
    /// <summary>
    /// Releases an acquired read or write lock when disposed.
    /// Supports both synchronous and asynchronous disposal.
    /// </summary>
    public sealed class LockReleaser : IDisposable, IAsyncDisposable
    {
        private readonly IAtomicReaderWriterLock _lock;
        private readonly bool _writer;
        private int _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="LockReleaser"/> class.
        /// </summary>
        /// <param name="lock">The lock to release when this instance is disposed.</param>
        /// <param name="writer">
        ///         true:  if a write lock was acquired; otherwise, 
        /// <br/>   false: if a read lock was acquired.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="lock"/> is <see langword="null"/>.
        /// </exception>
        public LockReleaser(IAtomicReaderWriterLock @lock, bool writer)
        {
            ArgumentNullException.ThrowIfNull(@lock);

            _lock = @lock;
            _writer = writer;
        }

        /// <summary>
        /// Releases the acquired read or write lock.
        /// Subsequent calls have no effect.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            if (_writer)
            {
                _lock.ExitWriteLock();
            }
            else
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Asynchronously releases the acquired read or write lock.
        /// </summary>
        /// <returns>A completed <see cref="ValueTask"/>.</returns>
        public ValueTask DisposeAsync()
        {
            Dispose();

            return ValueTask.CompletedTask;
        }
    }
}
