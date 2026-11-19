namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Locking
{
    /// <summary>
    /// Defines a thread-safe reader-writer lock that supports both synchronous and asynchronous lock acquisition.
    /// </summary>
    public interface IAtomicReaderWriterLock
    {
        /// <summary>
        /// Acquires a read lock synchronously.
        /// </summary>
        void EnterReadLock();

        /// <summary>
        /// Releases a previously acquired read lock.
        /// </summary>
        void ExitReadLock();

        /// <summary>
        /// Acquires a write lock synchronously.
        /// </summary>
        void EnterWriteLock();

        /// <summary>
        /// Releases a previously acquired write lock.
        /// </summary>
        void ExitWriteLock();

        /// <summary>
        /// Acquires a read lock asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the lock acquisition.</param>
        /// <returns>A task representing the asynchronous lock acquisition.</returns>
        Task EnterReadLockAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Acquires a write lock asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the lock acquisition.</param>
        /// <returns>A task representing the asynchronous lock acquisition.</returns>
        Task EnterWriteLockAsync(CancellationToken cancellationToken = default);
    }
}
