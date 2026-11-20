using Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries;

namespace Chris82111.GitManager.GitWrapper.Tests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class Test_Core_KnownHosts_Entries_CachedHost
    {
        [TestMethod]
        public void Test_01_Create()
        {
            CachedHost cachedHost;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            cachedHost = new CachedHost(null);
#pragma warning restore CS8625
            Assert.AreEqual("", cachedHost.MatchHostname);
            Assert.AreEqual(22, cachedHost.Port);
            Assert.IsNull(cachedHost.HostnameRegex);
            Assert.IsFalse(cachedHost.HasWildcard);
            Assert.IsFalse(cachedHost.IsExclusion);

            cachedHost = new CachedHost("");
            Assert.AreEqual("", cachedHost.MatchHostname);
            Assert.AreEqual(22, cachedHost.Port);
            Assert.IsNull(cachedHost.HostnameRegex);
            Assert.IsFalse(cachedHost.HasWildcard);
            Assert.IsFalse(cachedHost.IsExclusion);

            cachedHost = new CachedHost("github.com");
            Assert.AreEqual("github.com", cachedHost.MatchHostname);
            Assert.AreEqual(22, cachedHost.Port);
            Assert.IsNull(cachedHost.HostnameRegex);
            Assert.IsFalse(cachedHost.HasWildcard);
            Assert.IsFalse(cachedHost.IsExclusion);

            cachedHost = new CachedHost("GitHub.cOm");
            Assert.AreEqual("GitHub.cOm", cachedHost.MatchHostname);
            Assert.AreEqual(22, cachedHost.Port);
            Assert.IsNull(cachedHost.HostnameRegex);
            Assert.IsFalse(cachedHost.HasWildcard);
            Assert.IsFalse(cachedHost.IsExclusion);

            cachedHost = new CachedHost("github.*");
            Assert.AreEqual("github.*", cachedHost.MatchHostname);
            Assert.AreEqual(22, cachedHost.Port);
            Assert.IsNotNull(cachedHost.HostnameRegex);
            Assert.IsTrue(cachedHost.HasWildcard);
            Assert.IsFalse(cachedHost.IsExclusion);

            cachedHost = new CachedHost("GitHub.*");
            Assert.AreEqual("GitHub.*", cachedHost.MatchHostname);
            Assert.AreEqual(22, cachedHost.Port);
            Assert.IsNotNull(cachedHost.HostnameRegex);
            Assert.IsTrue(cachedHost.HasWildcard);
            Assert.IsFalse(cachedHost.IsExclusion);

            cachedHost = new CachedHost("git??b.com");
            Assert.AreEqual("git??b.com", cachedHost.MatchHostname);
            Assert.AreEqual(22, cachedHost.Port);
            Assert.IsNotNull(cachedHost.HostnameRegex);
            Assert.IsTrue(cachedHost.HasWildcard);
            Assert.IsFalse(cachedHost.IsExclusion);

            cachedHost = new CachedHost("Git??b.cOm");
            Assert.AreEqual("Git??b.cOm", cachedHost.MatchHostname);
            Assert.AreEqual(22, cachedHost.Port);
            Assert.IsNotNull(cachedHost.HostnameRegex);
            Assert.IsTrue(cachedHost.HasWildcard);
            Assert.IsFalse(cachedHost.IsExclusion);

            cachedHost = new CachedHost("!github.com");
            Assert.AreEqual("github.com", cachedHost.MatchHostname);
            Assert.AreEqual(22, cachedHost.Port);
            Assert.IsNull(cachedHost.HostnameRegex);
            Assert.IsFalse(cachedHost.HasWildcard);
            Assert.IsTrue(cachedHost.IsExclusion);

            cachedHost = new CachedHost("!GitHub.cOm");
            Assert.AreEqual("GitHub.cOm", cachedHost.MatchHostname);
            Assert.AreEqual(22, cachedHost.Port);
            Assert.IsNull(cachedHost.HostnameRegex);
            Assert.IsFalse(cachedHost.HasWildcard);
            Assert.IsTrue(cachedHost.IsExclusion);

            cachedHost = new CachedHost("!github.*");
            Assert.AreEqual("github.*", cachedHost.MatchHostname);
            Assert.AreEqual(22, cachedHost.Port);
            Assert.IsNotNull(cachedHost.HostnameRegex);
            Assert.IsTrue(cachedHost.HasWildcard);
            Assert.IsTrue(cachedHost.IsExclusion);

            cachedHost = new CachedHost("!git??b.com");
            Assert.AreEqual("git??b.com", cachedHost.MatchHostname);
            Assert.AreEqual(22, cachedHost.Port);
            Assert.IsNotNull(cachedHost.HostnameRegex);
            Assert.IsTrue(cachedHost.HasWildcard);
            Assert.IsTrue(cachedHost.IsExclusion);

            cachedHost = new CachedHost("[github.com]:22");
            Assert.AreEqual("github.com", cachedHost.MatchHostname);
            Assert.AreEqual(22, cachedHost.Port);
            Assert.IsNull(cachedHost.HostnameRegex);
            Assert.IsFalse(cachedHost.HasWildcard);
            Assert.IsFalse(cachedHost.IsExclusion);

            cachedHost = new CachedHost("[github.com]:23");
            Assert.AreEqual("github.com", cachedHost.MatchHostname);
            Assert.AreEqual(23, cachedHost.Port);
            Assert.IsNull(cachedHost.HostnameRegex);
            Assert.IsFalse(cachedHost.HasWildcard);
            Assert.IsFalse(cachedHost.IsExclusion);

            cachedHost = new CachedHost("[GitHub.cOm]:23");
            Assert.AreEqual("GitHub.cOm", cachedHost.MatchHostname);
            Assert.AreEqual(23, cachedHost.Port);
            Assert.IsNull(cachedHost.HostnameRegex);
            Assert.IsFalse(cachedHost.HasWildcard);
            Assert.IsFalse(cachedHost.IsExclusion);

            cachedHost = new CachedHost("[github.*]:23");
            Assert.AreEqual("github.*", cachedHost.MatchHostname);
            Assert.AreEqual(23, cachedHost.Port);
            Assert.IsNotNull(cachedHost.HostnameRegex);
            Assert.IsTrue(cachedHost.HasWildcard);
            Assert.IsFalse(cachedHost.IsExclusion);

            cachedHost = new CachedHost("[git??b.com]:23");
            Assert.AreEqual("git??b.com", cachedHost.MatchHostname);
            Assert.AreEqual(23, cachedHost.Port);
            Assert.IsNotNull(cachedHost.HostnameRegex);
            Assert.IsTrue(cachedHost.HasWildcard);
            Assert.IsFalse(cachedHost.IsExclusion);

            cachedHost = new CachedHost("![git??b.com]:23");
            Assert.AreEqual("git??b.com", cachedHost.MatchHostname);
            Assert.AreEqual(23, cachedHost.Port);
            Assert.IsNotNull(cachedHost.HostnameRegex);
            Assert.IsTrue(cachedHost.HasWildcard);
            Assert.IsTrue(cachedHost.IsExclusion);
            
            try
            {
                Assert.Throws<ArgumentException>(() => new CachedHost("[any]:c"));
                Assert.Throws<ArgumentOutOfRangeException>(() => new CachedHost("[any]:-1"));
                Assert.Throws<ArgumentOutOfRangeException>(() => new CachedHost("[any]:0"));
                
                // biggest allowed number
                cachedHost = new CachedHost("[any]:65535");
                
                Assert.Throws<ArgumentOutOfRangeException>(() => new CachedHost("[any]:65536"));
                Assert.Throws<ArgumentException>(() => new CachedHost("[any:22"));
                Assert.Throws<ArgumentException>(() => new CachedHost("any:22"));
                Assert.Throws<ArgumentException>(() => new CachedHost("any]:22"));
                Assert.Throws<ArgumentException>(() => new CachedHost("any]"));
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void Test_02()
        {
            CachedHost cachedHost = new CachedHost("![git??b.*]:23");
            Assert.AreEqual("git??b.*", cachedHost.MatchHostname);
            Assert.AreEqual(23, cachedHost.Port);
            Assert.IsNotNull(cachedHost.HostnameRegex);
            Assert.IsTrue(cachedHost.HasWildcard);
            Assert.IsTrue(cachedHost.IsExclusion);

            Assert.AreEqual("^git..b\\..*$", cachedHost.HostnameRegex.ToString());

        }
        
        [TestMethod]
        public void Test_03()
        {
            CachedHost cachedHost;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            cachedHost = new CachedHost(null);
#pragma warning restore CS8625
            Assert.IsFalse(cachedHost.MatchesHost("github.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsFalse(cachedHost.MatchesHost("github.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("");
            Assert.IsFalse(cachedHost.MatchesHost("github.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsFalse(cachedHost.MatchesHost("github.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("github.com");
            Assert.IsTrue(cachedHost.MatchesHost("github.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsFalse(cachedHost.MatchesHost("github.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("GitHub.cOm");
            Assert.IsTrue(cachedHost.MatchesHost("github.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsFalse(cachedHost.MatchesHost("github.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("github.*");
            Assert.IsTrue(cachedHost.MatchesHost("github.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsTrue(cachedHost.MatchesHost("github.org"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsFalse(cachedHost.MatchesHost("github.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("GitHub.*");
            Assert.IsTrue(cachedHost.MatchesHost("github.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsTrue(cachedHost.MatchesHost("github.org"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsFalse(cachedHost.MatchesHost("github.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("git??b.com");
            Assert.IsTrue(cachedHost.MatchesHost("github.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsTrue(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsFalse(cachedHost.MatchesHost("github.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("Git??b.cOm");
            Assert.IsTrue(cachedHost.MatchesHost("github.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsTrue(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsFalse(cachedHost.MatchesHost("github.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("!github.com");
            Assert.IsTrue(cachedHost.MatchesHost("github.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsFalse(cachedHost.MatchesHost("github.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("!GitHub.cOm");
            Assert.IsTrue(cachedHost.MatchesHost("github.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsFalse(cachedHost.MatchesHost("github.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("!github.*");
            Assert.IsTrue(cachedHost.MatchesHost("github.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsTrue(cachedHost.MatchesHost("github.org"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsFalse(cachedHost.MatchesHost("github.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("!git??b.com");
            Assert.IsTrue(cachedHost.MatchesHost("github.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsTrue(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsFalse(cachedHost.MatchesHost("github.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("[github.com]:22");
            Assert.IsTrue(cachedHost.MatchesHost("github.com"));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsFalse(cachedHost.MatchesHost("github.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("[github.com]:23");
            Assert.IsFalse(cachedHost.MatchesHost("github.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsTrue(cachedHost.MatchesHost("github.com", 23));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("[GitHub.cOm]:23");
            Assert.IsFalse(cachedHost.MatchesHost("github.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsTrue(cachedHost.MatchesHost("github.com", 23));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("[github.*]:23");
            Assert.IsFalse(cachedHost.MatchesHost("github.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsTrue(cachedHost.MatchesHost("github.com", 23));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsTrue(cachedHost.MatchesHost("github.org", 23));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("[git??b.com]:23");
            Assert.IsFalse(cachedHost.MatchesHost("github.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsTrue(cachedHost.MatchesHost("github.com", 23));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsTrue(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsTrue(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));

            cachedHost = new CachedHost("![git??b.com]:23");
            Assert.IsFalse(cachedHost.MatchesHost("github.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("github.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.com"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.CoM"));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org"));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG"));

            Assert.IsTrue(cachedHost.MatchesHost("github.com", 23));
            Assert.IsTrue(cachedHost.MatchesHost("gIThUb.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("github.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gIThUb.OrG", 23));
            Assert.IsTrue(cachedHost.MatchesHost("gitlab.com", 23));
            Assert.IsTrue(cachedHost.MatchesHost("gITlAB.CoM", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gitlab.org", 23));
            Assert.IsFalse(cachedHost.MatchesHost("gITlAB.OrG", 23));
        }
    }
}
