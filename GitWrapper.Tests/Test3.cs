using Chris82111.GitManager.GitWrapper.Core;
using Chris82111.GitManager.GitWrapper.Core.Authenticators;
using Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables;
using Chris82111.GitManager.GitWrapper.Core.Processes;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Chris82111.GitManager.GitWrapper.Tests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class Test3
    {
        private sealed class ConfigData
        {
            public required string Username { get; init; }
            public required string Token { get; init; }
            public required string PrivateRepo { get; init; }
            public required string PrivateRepoSsh { get; init; }
            public required string SshFile { get; init; }
            public required string SshFileInvalid { get; init; }
            public required string SshFilePwProtected { get; init; }
            public required string SshFilePassword { get; init; }
        }

        private const string _configFile = ".GitManager.json";

        private readonly ConfigData? _configData = null;

        private readonly string? _configErrorMessage = null;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        public Test3()
        {
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string filePath = Path.Combine(homeDirectory, _configFile);

            string json;

            ConfigData? configData =
#if true
                null;
#else
                new()
                {
                    Username = "",
                    Token = "",
                    PrivateRepo = "",
                    PrivateRepoSsh = "",
                    SshFile = "",
                    SshFileInvalid = "",
                    SshFilePwProtected = "",
                    SshFilePassword = "",
                };
#endif

            if (null != configData)
            {
                json = JsonSerializer.Serialize(configData, _jsonSerializerOptions);

                File.WriteAllText(filePath, json);

                _configData = configData;
            }

            try
            {
                json = File.ReadAllText(filePath);
            }
            catch
            {
                _configErrorMessage = $"This test requires a '{_configFile}' file containing configuration data; otherwise, these tests cannot be run.";
                return;
            }

            configData = JsonSerializer.Deserialize<ConfigData>(json);

            if (null == configData)
            {
                _configErrorMessage = $"The configuration file could not be deserialised; please check the configuration file '{_configFile}'";
            }

            _configData = configData;
        }
        
        [MemberNotNull(nameof(_configData))]
        private void EnsureConfigData()
        {
            if (null == _configData)
            {
                Assert.Inconclusive(_configErrorMessage ?? "The configuration DTO must not be null.");
            }
        }

        private static IGitWrapper EnsureGitWrapper()
        {
            var git = GitWrapperFactory.Create();

            if (false == git.IsSupported)
            {
                Assert.Inconclusive($"Current Git '{git.Identifier}' is not supported.");
            }

            git.ExtractArchive();

            if (false == git.IsGitAvailable())
            {
                Assert.Inconclusive($"Current Git '{git.Identifier}' is not available.");
            }

            return git;
        }

        private static void RemoveMirror(string url)
        {
            var mirror = MirrorDirectory(url);

            if (mirror.Exists)
            {
                var files = System.IO.Directory.EnumerateFiles(
                    mirror.FullName,
                    "*",
                    SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }

                System.IO.Directory.Delete(mirror.FullName, recursive: true);
            }
        }

        private static DirectoryInfo MirrorDirectory(string url)
        {
            string repoName = Path.GetFileNameWithoutExtension(url);
            var mirror = new DirectoryInfo($"{repoName}.git");
            return mirror;
        }

        [TestMethod]
        public void Test_01_SwitchingCredentials()
        {
            var git = EnsureGitWrapper();

            Assert.HasCount(6, Enum.GetNames<Credentials>());

            Assert.AreEqual(Credentials.Default, git.Credential);

            git.Credential = Credentials.Default;
            Assert.AreEqual(Credentials.Default, git.Credential);

            git.Credential = Credentials.Token;
            Assert.AreEqual(Credentials.Token, git.Credential);

            git.Credential = Credentials.Nothing;
            Assert.AreEqual(Credentials.Nothing, git.Credential);

            git.Credential = Credentials.UserAction;
            Assert.AreEqual(Credentials.UserAction, git.Credential);

            git.Credential = Credentials.Ssh;
            Assert.AreEqual(Credentials.Ssh, git.Credential);

            git.Credential = Credentials.SshNoPassphrase;
            Assert.AreEqual(Credentials.SshNoPassphrase, git.Credential);

            git.Credential = Credentials.Default;
            Assert.AreEqual(Credentials.Default, git.Credential);
        }

        [TestMethod]
        public void Test_02_SetAuthentication()
        {
            var git = EnsureGitWrapper();

            Assert.IsNotNull(git.Authentication);

            // Not normal, but possible
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            git.Authentication = null;
#pragma warning restore CS8625

            var authentication = new GitAuthenticationBase()
            {
                Name = "first"
            };

            git.Authentication = authentication;

            Assert.AreEqual(git.Authentication, authentication);
            Assert.AreEqual("first", authentication.Name);
            Assert.AreEqual("first", git.Authentication.Name);

            var git2 = GitWrapperFactory.Create();
            git2.Authentication = authentication;

            Assert.AreEqual(git2.Authentication, authentication);
            Assert.AreEqual("first", git2.Authentication.Name);

            authentication.Name = "second";
            Assert.AreEqual("second", authentication.Name);
            Assert.AreEqual("second", git.Authentication.Name);
            Assert.AreEqual("second", git2.Authentication.Name);
        }

        [TestMethod]
        public void Test_03_SwitchingCredentialsWithAuthenticationNull()
        {
            var git = EnsureGitWrapper();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            git.Authentication = null;
#pragma warning restore CS8625

            git.Credential = Credentials.Default;
            Assert.AreEqual(Credentials.Default, git.Credential);

            git.Credential = Credentials.Token;
            Assert.AreEqual(Credentials.Token, git.Credential);

            git.Credential = Credentials.Nothing;
            Assert.AreEqual(Credentials.Nothing, git.Credential);

            git.Credential = Credentials.UserAction;
            Assert.AreEqual(Credentials.UserAction, git.Credential);

            git.Credential = Credentials.Ssh;
            Assert.AreEqual(Credentials.Ssh, git.Credential);

            git.Credential = Credentials.SshNoPassphrase;
            Assert.AreEqual(Credentials.SshNoPassphrase, git.Credential);

            git.Credential = Credentials.Default;
            Assert.AreEqual(Credentials.Default, git.Credential);
        }

        [TestMethod]
        public void Test_04_SetAuthenticationAndDisposeAuthentication()
        {
            var git1 = EnsureGitWrapper();
            var git2 = EnsureGitWrapper();
            var git3 = EnsureGitWrapper();

            var authentication = new GitAuthenticationBase()
            {
                Name = "first"
            };

            git1.Authentication = authentication;
            git2.Authentication = authentication;
            git3.Authentication = authentication;
            Assert.AreEqual(3, authentication.DependentCount);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            git2.Authentication = null;
#pragma warning restore CS8625

            Assert.AreEqual(2, authentication.DependentCount);

            git2.Authentication = authentication;
            Assert.AreEqual(3, authentication.DependentCount);
            git2.Authentication = authentication;
            Assert.AreEqual(3, authentication.DependentCount);

            authentication.Dispose();
            Assert.AreEqual(0, authentication.DependentCount);

            Assert.ThrowsExactly<ObjectDisposedException>(() => git1.Authentication.IsTokenValid());
            Assert.ThrowsExactly<ObjectDisposedException>(() => git2.Authentication.IsTokenValid());
            Assert.ThrowsExactly<ObjectDisposedException>(() => git3.Authentication.IsTokenValid());
        }

        [TestMethod]
        public void Test_05_SwitchingCredentialsAndAuthentication()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var git = EnsureGitWrapper();

            var authentication = new GitAuthenticationBase()
            {
                Name = "first"
            };

            git.Credential = Credentials.Default;
            git.Authentication = null;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.Default, git.Credential);

            git.Credential = Credentials.Token;
            git.Authentication = null;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.Token, git.Credential);

            git.Credential = Credentials.Nothing;
            git.Authentication = null;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.Nothing, git.Credential);

            git.Credential = Credentials.UserAction;
            git.Authentication = null;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.UserAction, git.Credential);

            git.Credential = Credentials.Ssh;
            git.Authentication = null;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.Ssh, git.Credential);

            git.Credential = Credentials.SshNoPassphrase;
            git.Authentication = null;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.SshNoPassphrase, git.Credential);

            git.Credential = Credentials.Default;
            git.Authentication = null;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.Default, git.Credential);


            git.Authentication = null;
            git.Credential = Credentials.Nothing;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.Nothing, git.Credential);

            git.Authentication = null;
            git.Credential = Credentials.Token;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.Token, git.Credential);

            git.Authentication = null;
            git.Credential = Credentials.Nothing;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.Nothing, git.Credential);

            git.Authentication = null;
            git.Credential = Credentials.UserAction;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.UserAction, git.Credential);

            git.Authentication = null;
            git.Credential = Credentials.Ssh;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.Ssh, git.Credential);

            git.Authentication = null;
            git.Credential = Credentials.SshNoPassphrase;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.SshNoPassphrase, git.Credential);

            git.Authentication = null;
            git.Credential = Credentials.Default;
            git.Authentication = authentication;
            Assert.AreEqual(Credentials.Default, git.Credential);


            git.Authentication = null;
            git.Authentication = authentication;
            git.Credential = Credentials.Nothing;
            Assert.AreEqual(Credentials.Nothing, git.Credential);

            git.Authentication = null;
            git.Authentication = authentication;
            git.Credential = Credentials.Token;
            Assert.AreEqual(Credentials.Token, git.Credential);

            git.Authentication = null;
            git.Authentication = authentication;
            git.Credential = Credentials.Nothing;
            Assert.AreEqual(Credentials.Nothing, git.Credential);

            git.Authentication = null;
            git.Authentication = authentication;
            git.Credential = Credentials.UserAction;
            Assert.AreEqual(Credentials.UserAction, git.Credential);

            git.Authentication = null;
            git.Authentication = authentication;
            git.Credential = Credentials.Ssh;
            Assert.AreEqual(Credentials.Ssh, git.Credential);

            git.Authentication = null;
            git.Authentication = authentication;
            git.Credential = Credentials.SshNoPassphrase;
            Assert.AreEqual(Credentials.SshNoPassphrase, git.Credential);

            git.Authentication = null;
            git.Authentication = authentication;
            git.Credential = Credentials.Default;
            Assert.AreEqual(Credentials.Default, git.Credential);
#pragma warning restore CS8625
        }

        [TestMethod]
        public void Test_06_PrivateRepositoryCredentialDefault()
        {
            EnsureConfigData();

            var git = EnsureGitWrapper();

            RemoveMirror(_configData.PrivateRepo);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepo).Exists);

            Assert.AreEqual(Credentials.Default, git.Credential);
            git.Credential = Credentials.Default;
            Assert.AreEqual(Credentials.Default, git.Credential);

            Assert.Inconclusive($"This cannot be tested as login details may be stored on the computer.");
        }

        [TestMethod]
        public void Test_07_PrivateRepositoryCredentialNothing()
        {
            EnsureConfigData();

            var git = EnsureGitWrapper();

            RemoveMirror(_configData.PrivateRepo);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepo).Exists);

            git.Credential = Credentials.Nothing;
            var dto = git.Mirror(_configData.PrivateRepo);

            Assert.AreEqual(128, dto.ExitCode);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepo).Exists);
        }

        [TestMethod]
        public void Test_08_PrivateRepositoryCredentialTokenNotSet()
        {
            EnsureConfigData();

            var git = EnsureGitWrapper();

            RemoveMirror(_configData.PrivateRepo);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepo).Exists);

            git.Credential = Credentials.Token;
            Assert.Throws<NullReferenceException>(() => git.Mirror(_configData.PrivateRepo));

            git.Authentication.SetToken(_configData.Username, _configData.Token);
            git.Authentication.ClearToken();
            Assert.Throws<NullReferenceException>(() => git.Mirror(_configData.PrivateRepo));

            git.Authentication.SetToken("USER", "TOKEN");
            var dto = git.Mirror(_configData.PrivateRepo);
            Assert.AreEqual(128, dto.ExitCode);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepo).Exists);
        }

        [TestMethod]
        public void Test_09_PrivateRepositoryCredentialToken()
        {
            EnsureConfigData();

            var git = EnsureGitWrapper();

            RemoveMirror(_configData.PrivateRepo);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepo).Exists);

            git.Authentication.SetToken(_configData.Username, _configData.Token);
            git.Credential = Credentials.Token;
            var dto = git.Mirror(_configData.PrivateRepo);

            Assert.AreEqual(0, dto.ExitCode);
            Assert.IsTrue(MirrorDirectory(_configData.PrivateRepo).Exists);
        }

        [TestMethod]
        public void Test_10_PrivateRepositoryCredentialTokenEnvironmentVariable()
        {
            EnsureConfigData();

            var git = EnsureGitWrapper();

            RemoveMirror(_configData.PrivateRepo);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepo).Exists);

            git.BeforeProcessStart += (sender, e) =>
            {
                var envVar = new ProcessEnvironmentVariable(e.Process.StartInfo);
                envVar.Set($"GIT_MANAGER_TOKEN", $"{_configData.Token ?? string.Empty}");
            };

            git.Authentication.SetTokenFromEnvironment(_configData.Username, $"GIT_MANAGER_TOKEN");
            git.Credential = Credentials.Token;
            var dto = git.Mirror(_configData.PrivateRepo);

            Assert.AreEqual(0, dto.ExitCode);
            Assert.IsTrue(MirrorDirectory(_configData.PrivateRepo).Exists);
        }

        private bool IsUserActionCalled = false;
        private void UserActionHandler(object? sender, ProcessEventArgs e)
        {
            IsUserActionCalled = true;
        }

        [TestMethod]
        public void Test_11_UserAction()
        {
            var git = EnsureGitWrapper();

            git.Credential = Credentials.UserAction;
            git.Authentication.UserAction = UserActionHandler;

            IsUserActionCalled = false;
            _ = git.GitVersion();
            Assert.IsTrue(IsUserActionCalled);
        }

        [TestMethod]
        public void Test_12_SshWrongUrl()
        {
            var git = EnsureGitWrapper();
            RemoveMirror(_configData!.PrivateRepoSsh);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepoSsh).Exists);

            git.Credential = Credentials.Ssh;

            git.Authentication.SetSsh(_configData.SshFileInvalid, string.Empty);
            var dto = git.Mirror(_configData.PrivateRepo);
            Assert.AreEqual(128, dto.ExitCode);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepo).Exists);

            git.Authentication.SetSsh(_configData.SshFile, string.Empty);
            dto = git.Mirror(_configData.PrivateRepo);
            Assert.AreEqual(128, dto.ExitCode);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepo).Exists);
        }

        [TestMethod]
        public void Test_13_SshRightUrl()
        {
            var git = EnsureGitWrapper();
            RemoveMirror(_configData!.PrivateRepoSsh);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepoSsh).Exists);

            git.Credential = Credentials.Ssh;

            git.Authentication.SetSsh(_configData.SshFileInvalid, string.Empty);
            var dto = git.Mirror(_configData.PrivateRepoSsh);
            Assert.AreEqual(128, dto.ExitCode);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepoSsh).Exists);

            git.Authentication.SetSsh(_configData.SshFile, string.Empty);
            dto = git.Mirror(_configData.PrivateRepoSsh);
            Assert.AreEqual(0, dto.ExitCode);
            Assert.IsTrue(MirrorDirectory(_configData.PrivateRepoSsh).Exists);
        }

        [TestMethod]
        public void Test_14_SshPassword()
        {
            var git = EnsureGitWrapper();
            RemoveMirror(_configData!.PrivateRepoSsh);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepoSsh).Exists);

            git.Credential = Credentials.Ssh;

            git.Authentication.SetSsh(_configData.SshFilePwProtected, _configData.SshFilePassword);
            var dto = git.Mirror(_configData.PrivateRepoSsh);

            //File.WriteAllText("log.txt", dto.StandardError);

            Assert.AreEqual(0, dto.ExitCode);
            Assert.IsTrue(MirrorDirectory(_configData.PrivateRepoSsh).Exists);
        }

        [TestMethod]
        public void Test_15_SshNoPassword()
        {
            var git = EnsureGitWrapper();
            RemoveMirror(_configData!.PrivateRepoSsh);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepoSsh).Exists);

            git.Credential = Credentials.Ssh;

            git.Authentication.SetSsh(_configData.SshFilePwProtected, null);
            var dto = git.Mirror(_configData.PrivateRepoSsh);

            Assert.AreEqual(128, dto.ExitCode);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepoSsh).Exists);
        }

        [TestMethod]
        public void Test_16_SshWrongPassword()
        {
            var git = EnsureGitWrapper();
            RemoveMirror(_configData!.PrivateRepoSsh);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepoSsh).Exists);

            git.Credential = Credentials.Ssh;

            git.Authentication.SetSsh(_configData.SshFilePwProtected, "IAmNotTheRightPassword");
            var dto = git.Mirror(_configData.PrivateRepoSsh);

            Assert.AreEqual(128, dto.ExitCode);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepoSsh).Exists);
        }

        [TestMethod]
        public void Test_17_SshSystem()
        {
            var git = new GitWrapperSystem();
            git.Credential = Credentials.Ssh;
            git.Authentication.SetSsh(_configData!.SshFilePwProtected, _configData.SshFilePassword);
            var dto = git.Mirror(_configData.PrivateRepoSsh);

            Assert.AreEqual(128, dto.ExitCode);
            Assert.IsFalse(MirrorDirectory(_configData.PrivateRepoSsh).Exists);
        }
    }
}
