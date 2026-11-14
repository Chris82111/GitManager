namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Locking
{
    public sealed class AtomicReaderWriterLock : IAtomicReaderWriterLock
    {
        private int _lockCount;

        public int Limit { get; }

        public int LockCount => Volatile.Read(ref _lockCount);

        public AtomicReaderWriterLock(int limit = int.MaxValue)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(limit, 1);

            Limit = limit;
        }

        public async Task EnterReadLockAsync(CancellationToken cancellationToken = default)
        {
            while (false == TryEnterRead())
            {
                await Task.Delay(1, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task EnterWriteLockAsync(CancellationToken cancellationToken = default)
        {
            while (false == TryEnterWrite())
            {
                await Task.Delay(1, cancellationToken).ConfigureAwait(false);
            }
        }

        public bool TryEnterRead()
        {
            while (true)
            {
                int current = Volatile.Read(ref _lockCount);

                if (0 > current || current >= Limit)
                {
                    return false;
                }

                if (Interlocked.CompareExchange(
                        ref _lockCount,
                        current + 1,
                        current) == current)
                {
                    return true;
                }
            }
        }

        public void EnterReadLock()
        {
            SpinWait spin = new();

            while (false == TryEnterRead())
            {
                spin.SpinOnce();
            }
        }

        public void ExitReadLock()
        {
            while (true)
            {
                int current = Volatile.Read(ref _lockCount);

                if (0 == current)
                {
                    throw new SynchronizationLockException("No read lock is currently held.");
                }

                if (0 > current)
                {
                    throw new SynchronizationLockException("A write lock is currently active, but there is no read lock.");
                }

                if (Interlocked.CompareExchange(
                        ref _lockCount,
                        current - 1,
                        current) == current)
                {
                    return;
                }
            }
        }

        public bool TryEnterWrite()
        {
            return Interlocked.CompareExchange(
                ref _lockCount,
                -1,
                0) == 0;
        }

        public void EnterWriteLock()
        {
            SpinWait spin = new();

            while (false == TryEnterWrite())
            {
                spin.SpinOnce();
            }
        }

        public void ExitWriteLock()
        {
            if (Interlocked.CompareExchange(
                    ref _lockCount,
                    0,
                    -1) != -1)
            {
                throw new SynchronizationLockException(
                    "No write lock is currently held.");
            }
        }
    }
}
