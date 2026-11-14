namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Locking
{
    public interface IAtomicReaderWriterLock
    {
        void EnterReadLock();

        void ExitReadLock();

        void EnterWriteLock();

        void ExitWriteLock();

        Task EnterReadLockAsync(CancellationToken cancellationToken = default);

        Task EnterWriteLockAsync(CancellationToken cancellationToken = default);
    }
}
