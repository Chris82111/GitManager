using Chris82111.GitManager.GitWrapper.Core.KnownHosts;
using Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries;

namespace Chris82111.GitManager.GitWrapper.Tests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class Test_Core_KnownHosts_KnownHostEntryFactory
    {
        public TestContext TestContext { get; set; } = null!;

        [TestMethod]
        public void Test_01_Create()
        {
            string[] lines =
            """
            
             #
              #
            		#
              	  #
            	  	#
             # any content
              # any content
            		# any content
              	  # any content
            	  	# any content






            # Comments allowed at start of line
            cvs.example.net,192.0.2.10 ssh-rsa AAAA1234.....=
            # A hashed hostname (under "AUTHORIZED_KEYS FILE FORMAT" the following is writte "[...] Each line of the file contains one key [...]"
            |1|JfKTdBh7rNbXkVAQCRp4OQoPfmI=|USECr3SWf1JUPsms5AqfD5QfxkM= ssh-rsa AAAA1235.....=
            # A revoked key
            @revoked * ssh-rsa AAAAB5X...
            # A CA key, accepted for any host in *.mydomain.com or *.mydomain.org
            @cert-authority *.mydomain.org,*.mydomain.com ssh-rsa AAAAB5W...

            @revoked |1|K68J9ow1e0T1rDpSkw+m9yHSz/s=|6vTdsjLX0of81vwmP43Rl6/8U80= ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIOMqqnkVzrm0SdG6UOoqKLsabgH5C9okWi0dh2l9GKJl
            github.com ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIOMqqnkVzrm0SdG6UOoqKLsabgH5C9okWi0dh2l9GKJl
            github.com,github.org ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIOMqqnkVzrm0SdG6UOoqKLsabgH5C9okWi0dh2l9GKJl
            github.com,github.org,!github.com ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIOMqqnkVzrm0SdG6UOoqKLsabgH5C9okWi0dh2l9GKJl
            gitlab.com,git???.org,!github.com ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIOMqqnkVzrm0SdG6UOoqKLsabgH5C9okWi0dh2l9GKJl
            """.Split(Environment.NewLine);

            var factory = KnownHostEntryFactory.Create(lines);
            Assert.HasCount(31, factory);

            var result = new Type[]
            {
                typeof(KnownHostEntryEmptyLine),
                typeof(KnownHostEntryComment),
                typeof(KnownHostEntryComment),
                typeof(KnownHostEntryComment),
                typeof(KnownHostEntryComment),
                typeof(KnownHostEntryComment),
                typeof(KnownHostEntryComment),
                typeof(KnownHostEntryComment),
                typeof(KnownHostEntryComment),
                typeof(KnownHostEntryComment),
                typeof(KnownHostEntryComment),
                typeof(KnownHostEntryEmptyLine),
                typeof(KnownHostEntryEmptyLine),
                typeof(KnownHostEntryEmptyLine),
                typeof(KnownHostEntryEmptyLine),
                typeof(KnownHostEntryEmptyLine),
                typeof(KnownHostEntryEmptyLine),
                typeof(KnownHostEntryComment),
                typeof(KnownHostMultiple),
                typeof(KnownHostEntryComment),
                typeof(KnownHostHashed),
                typeof(KnownHostEntryComment),
                typeof(KnownHostSingle),
                typeof(KnownHostEntryComment),
                typeof(KnownHostMultiple),
                typeof(KnownHostEntryEmptyLine),
                typeof(KnownHostHashed),
                typeof(KnownHostSingle),
                typeof(KnownHostMultiple),
                typeof(KnownHostMultiple),
                typeof(KnownHostMultiple)
            };


            for (int i = 0; i < factory.Count; i++)
            {
                var entry = factory[i];
                Assert.IsInstanceOfType(entry, result[i]);
            }

            string[] newLines =
            """
            
             #
              #
            		#
              	  #
            	  	#
             # any content
              # any content
            		# any content
              	  # any content
            	  	# any content






            # Comments allowed at start of line
            cvs.example.net,192.0.2.10 ssh-rsa AAAA1234.....=
            # A hashed hostname (under "AUTHORIZED_KEYS FILE FORMAT" the following is writte "[...] Each line of the file contains one key [...]"
            |1|JfKTdBh7rNbXkVAQCRp4OQoPfmI=|USECr3SWf1JUPsms5AqfD5QfxkM= ssh-rsa AAAA1235.....=
            # A revoked key
            @revoked * ssh-rsa AAAAB5X...
            # A CA key, accepted for any host in *.mydomain.com or *.mydomain.org
            @cert-authority *.mydomain.org,*.mydomain.com ssh-rsa AAAAB5W...

            @revoked |1|K68J9ow1e0T1rDpSkw+m9yHSz/s=|6vTdsjLX0of81vwmP43Rl6/8U80= ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIOMqqnkVzrm0SdG6UOoqKLsabgH5C9okWi0dh2l9GKJl
            @revoked github.com ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIOMqqnkVzrm0SdG6UOoqKLsabgH5C9okWi0dh2l9GKJl
            @revoked github.com,github.org ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIOMqqnkVzrm0SdG6UOoqKLsabgH5C9okWi0dh2l9GKJl
            @revoked github.com,github.org,!github.com ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIOMqqnkVzrm0SdG6UOoqKLsabgH5C9okWi0dh2l9GKJl
            @revoked gitlab.com,git???.org,!github.com ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIOMqqnkVzrm0SdG6UOoqKLsabgH5C9okWi0dh2l9GKJl
            """.Split(Environment.NewLine);

            for (int i = 0; i < factory.Count; i++)
            {
                var entry = factory[i];
                Assert.AreEqual(newLines[i], entry.ToString());
            }
            ;
        }
    }
}
