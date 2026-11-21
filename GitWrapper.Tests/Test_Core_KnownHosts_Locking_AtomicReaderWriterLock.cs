using Chris82111.GitManager.GitWrapper.Core.KnownHosts.Locking;

namespace Chris82111.GitManager.GitWrapper.Tests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class Test_Core_KnownHosts_Locking_AtomicReaderWriterLock
    {
        [TestMethod]
        public void Test_01()
        {
            var l = new AtomicReaderWriterLock();
            Assert.AreEqual(int.MaxValue, l.Limit);

            l.EnterReadLock();
            Assert.AreEqual(1, l.LockCount);
            l.EnterReadLock();
            Assert.AreEqual(2, l.LockCount);
            l.ExitReadLock();
            Assert.AreEqual(1, l.LockCount);
            l.ExitReadLock();
            Assert.AreEqual(0, l.LockCount);

            l.EnterWriteLock();
            Assert.AreEqual(-1, l.LockCount);
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                l.ExitWriteLock();
            });

            l.EnterWriteLock();
            Assert.AreEqual(-1, l.LockCount);
            l.ExitWriteLock();
            Assert.AreEqual(0, l.LockCount);
        }

        [TestMethod]
        public void Test_02()
        {
            var l = new AtomicReaderWriterLock(4);
            Assert.AreEqual(4, l.Limit);

            l.EnterReadLock();
            Assert.AreEqual(1, l.LockCount);
            l.EnterReadLock();
            Assert.AreEqual(2, l.LockCount);
            l.EnterReadLock();
            Assert.AreEqual(3, l.LockCount);
            l.EnterReadLock();
            Assert.AreEqual(4, l.LockCount);

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                l.ExitReadLock();
            });

            l.EnterReadLock();
            Assert.AreEqual(4, l.LockCount);

            l.ExitReadLock();
            l.ExitReadLock();
            l.ExitReadLock();
            Assert.AreEqual(1, l.LockCount);

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                l.ExitReadLock();
            });

            l.EnterWriteLock();
            Assert.AreEqual(-1, l.LockCount);
            l.ExitWriteLock();
            Assert.AreEqual(0, l.LockCount);
        }

        [TestMethod]
        public void Test_03()
        {
            var l = new AtomicReaderWriterLock(4);

            using (var autoLock1 = ((Func<IDisposable>)(() => { l.EnterReadLock(); return new LockReleaser(l, writer: false); }))())
            {
                Assert.AreEqual(1, l.LockCount);

                using (var autoLock2 = ((Func<IDisposable>)(() => { l.EnterReadLock(); return new LockReleaser(l, writer: false); }))())
                {
                    Assert.AreEqual(2, l.LockCount);

                    using (var autoLock3 = ((Func<IDisposable>)(() => { l.EnterReadLock(); return new LockReleaser(l, writer: false); }))())
                    {
                        Assert.AreEqual(3, l.LockCount);
                    }

                    Assert.AreEqual(2, l.LockCount);
                }

                Assert.AreEqual(1, l.LockCount);
            }

            Assert.AreEqual(0, l.LockCount);
        }
    }
}
