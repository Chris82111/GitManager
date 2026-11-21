namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Locking
{
    /// <summary>
    /// Provides a thread-safe reader-writer lock supporting synchronous and asynchronous
    /// lock acquisition, with an optional limit on concurrent readers.
    /// </summary>
    public sealed class AtomicReaderWriterLock : IAtomicReaderWriterLock
    {
        private int _lockCount;

        /// <summary>
        /// Gets the maximum number of concurrent readers allowed.
        /// </summary>
        public int Limit { get; }

        /// <summary>
        /// Gets the current lock state.
        /// <br/>0: Unlocked
        /// <br/>1 to <see cref="Limit"/>: Number of active readers
        /// <br/>-1: Writer active
        /// </summary>
        /// <value>
        /// The current lock state.
        /// </value>
        public int LockCount => Volatile.Read(ref _lockCount);

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicReaderWriterLock"/> class.
        /// </summary>
        /// <param name="limit">The maximum number of concurrent readers. Defaults to <see cref="int.MaxValue"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified limit is less than 1.</exception>
        public AtomicReaderWriterLock(int limit = int.MaxValue)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(limit, 1);

            Limit = limit;
        }


#if false

        /// <summary>
        /// Acquires a read lock and returns a disposable object that automatically releases the lock when disposed.
        /// </summary>
        /// <remarks>
        /// The returned object can be used with a <see langword="using"/> statement:
        /// <code>
        /// using (_lock.ReaderLock())
        /// {
        ///     // Protected code
        /// }
        /// </code>
        /// </remarks>
        /// <returns>A disposable object that releases the acquired read lock.</returns>
        public IDisposable ReaderLock()
        {
            EnterReadLock();

            return new LockReleaser(this, false);
        }

        /// <summary>
        /// Acquires a write lock and returns a disposable object that automatically releases the lock when disposed.
        /// </summary>
        /// <remarks>
        /// The returned object can be used with a <see langword="using"/> statement:
        /// <code>
        /// using (_lock.WriterLock())
        /// {
        ///     // Protected code
        /// }
        /// </code>
        /// </remarks>
        /// <returns>A disposable object that releases the acquired write lock.</returns>
        public IDisposable WriterLock()
        {
            EnterWriteLock();

            return new LockReleaser(this, true);
        }

        /// <summary>
        /// Acquires a read lock asynchronously and returns a disposable object that automatically releases the lock when disposed.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the lock acquisition.</param>
        /// <remarks>
        /// The returned object can be used with a <see langword="using"/> statement:
        /// <code>
        /// using (await _lock.ReaderLockAsync())
        /// {
        ///     // Protected code
        /// }
        /// </code>
        /// </remarks>
        /// <returns>
        /// A task representing the asynchronous operation, containing a disposable object that releases the read lock.
        /// </returns>
        public async Task<IDisposable> ReaderLockAsync(CancellationToken cancellationToken = default)
        {
            await EnterReadLockAsync(cancellationToken).ConfigureAwait(false);

            return new LockReleaser(this, false);
        }

        /// <summary>
        /// Acquires a write lock asynchronously and returns a disposable object that automatically releases the lock when disposed.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the lock acquisition.</param>
        /// <remarks>
        /// The returned object can be used with a <see langword="using"/> statement:
        /// <code>
        /// using (await _lock.WriterLockAsync())
        /// {
        ///     // Protected code
        /// }
        /// </code>
        /// </remarks>
        /// <returns>
        /// A task representing the asynchronous operation, containing a disposable object that releases the write lock.
        /// </returns>
        public async Task<IDisposable> WriterLockAsync(CancellationToken cancellationToken = default)
        {
            await EnterWriteLockAsync(cancellationToken).ConfigureAwait(false);

            return new LockReleaser(this, true);
        }
#endif


        /// <inheritdoc />
        public async Task EnterReadLockAsync(CancellationToken cancellationToken = default)
        {
            while (false == TryEnterRead())
            {
                await Task.Delay(1, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public async Task EnterWriteLockAsync(CancellationToken cancellationToken = default)
        {
            while (false == TryEnterWrite())
            {
                await Task.Delay(1, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Attempts to acquire a read lock without waiting.
        /// </summary>
        /// <returns>
        ///         <see langword="true"/> if the read lock was acquired,
        /// <br/>   <see langword="false"/> otherwise.
        /// </returns>
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

        /// <inheritdoc />
        public void EnterReadLock()
        {
            SpinWait spin = new();

            while (false == TryEnterRead())
            {
                spin.SpinOnce();
            }
        }

        /// <inheritdoc />
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

        /// <summary>
        /// Attempts to acquire a write lock without waiting.
        /// </summary>
        /// <returns>
        ///         <see langword="true"/>  if the write lock was acquired,
        /// <br/>   <see langword="false"/> otherwise.
        /// </returns>
        public bool TryEnterWrite()
        {
            return Interlocked.CompareExchange(
                ref _lockCount,
                -1,
                0) == 0;
        }

        /// <inheritdoc />
        public void EnterWriteLock()
        {
            SpinWait spin = new();

            while (false == TryEnterWrite())
            {
                spin.SpinOnce();
            }
        }

        /// <inheritdoc />
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
