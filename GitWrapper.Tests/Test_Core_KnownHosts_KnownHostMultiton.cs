using Chris82111.GitManager.GitWrapper.Core.KnownHosts;

namespace Chris82111.GitManager.GitWrapper.Tests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class Test_Core_KnownHosts_KnownHostMultiton
    {
        [TestMethod]
        public async Task Test_01Async()
        {
            var knownHost1 = await KnownHostMultiton.InstanceAsync("known_hosts");
            var knownHost2 = await KnownHostMultiton.InstanceAsync("known_hosts");
            var knownHost3 = await KnownHostMultiton.InstanceAsync("known_hosts");
            var knownHost4 = await KnownHostMultiton.InstanceAsync("does_not_exists_1");
            var knownHost5 = await KnownHostMultiton.InstanceAsync("does_not_exists_1");
            var knownHost6 = await KnownHostMultiton.InstanceAsync("does_not_exists_2");
            var knownHost7 = await KnownHostMultiton.InstanceAsync("does_not_exists_2");

            Assert.AreEqual(knownHost1, knownHost2);
            Assert.AreEqual(knownHost2, knownHost3);
            Assert.AreEqual(knownHost3, knownHost1);

            Assert.AreEqual(knownHost4, knownHost5);

            Assert.AreEqual(knownHost6, knownHost7);

            Assert.AreNotEqual(knownHost1, knownHost4);
            Assert.AreNotEqual(knownHost4, knownHost6);
            Assert.AreNotEqual(knownHost6, knownHost1);

        }
    }
}
