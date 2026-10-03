using Chris82111.GitManager.GitWrapper.Core;
using Chris82111.GitManager.GitWrapper.Static.WindowsX64;
using Chris82111.GitManager.GitWrapper.Static.LinuxX64;
using System.Runtime.InteropServices;

namespace Chris82111.GitManager.GitWrapper.TestReference
{
    [TestClass]
    [DoNotParallelize]
    public sealed class Test1
    {
        public TestContext TestContext { get; set; } = null!;
        public TestContext Console { get => TestContext; set => TestContext = value; }
        const string RepositoryName = "GitManager";

        [TestMethod]
        public void Test_01_CreateTypesDirect()
        {
            var gitWrapper = new GitWrapperSystem();
            Assert.AreEqual(typeof(GitWrapperSystem), gitWrapper.GetType());

            var gitWrapperWindowsX64 = new GitWrapperWindowsX64();
            Assert.AreEqual(typeof(GitWrapperWindowsX64), gitWrapperWindowsX64.GetType());

            var gitWrapperLinuxX64 = new GitWrapperLinuxX64();
            Assert.AreEqual(typeof(GitWrapperLinuxX64), gitWrapperLinuxX64.GetType());
        }

        [TestMethod]
        public void Test_02_UsableLocal()
        {
            var gitWrapper = new GitWrapperSystem();
            Assert.AreEqual(typeof(GitWrapperSystem), gitWrapper.GetType());
            Assert.IsTrue(gitWrapper.IsGitAvailable());

            gitWrapper.ExtractArchive();
            Assert.IsTrue(gitWrapper.IsGitAvailable());
        }

        [TestMethod]
        public async Task Test_02_UsableLocalAsync()
        {
            var gitWrapper = new GitWrapperSystem();
            Assert.AreEqual(typeof(GitWrapperSystem), gitWrapper.GetType());
            Assert.IsTrue(await gitWrapper.IsGitAvailableAsync());

            await gitWrapper.ExtractArchiveAsync();
            Assert.IsTrue(await gitWrapper.IsGitAvailableAsync());
        }

        private static void RemoveExtracted(DirectoryInfo directory)
        {
            Directory.CreateDirectory(directory.FullName);

            foreach (var file in Directory.EnumerateFiles(
                directory.FullName,
                "*",
                SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            Directory.Delete(directory.FullName, recursive: true);
        }

        [TestMethod]
        public void Test_02_UsableWindowsX64()
        {
            if (false == RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            var gitWrapperWindowsX64 = new GitWrapperWindowsX64();
            Assert.AreEqual(typeof(GitWrapperWindowsX64), gitWrapperWindowsX64.GetType());
            Assert.IsFalse(gitWrapperWindowsX64.IsGitAvailable());

            RemoveExtracted(
                new DirectoryInfo(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        gitWrapperWindowsX64.ExtractDirectory)));

            gitWrapperWindowsX64.ExtractArchive();
            Assert.IsTrue(gitWrapperWindowsX64.IsGitAvailable());


            var gitWrapperLinuxX64 = new GitWrapperLinuxX64();
            Assert.AreEqual(typeof(GitWrapperLinuxX64), gitWrapperLinuxX64.GetType());
            Assert.IsFalse(gitWrapperLinuxX64.IsGitAvailable());

            Assert.ThrowsExactly<PlatformNotSupportedException>(() => gitWrapperLinuxX64.ExtractArchive());
            Assert.ThrowsExactly<PlatformNotSupportedException>(gitWrapperLinuxX64.EnsureSupported);
        }

        [TestMethod]
        public async Task Test_02_UsableWindowsX64Async()
        {
            if (false == RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            var gitWrapperWindowsX64 = new GitWrapperWindowsX64();
            Assert.AreEqual(typeof(GitWrapperWindowsX64), gitWrapperWindowsX64.GetType());
            Assert.IsFalse(await gitWrapperWindowsX64.IsGitAvailableAsync());

            RemoveExtracted(
                new DirectoryInfo(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        gitWrapperWindowsX64.ExtractDirectory)));

            await gitWrapperWindowsX64.ExtractArchiveAsync();
            Assert.IsTrue(await gitWrapperWindowsX64.IsGitAvailableAsync());


            var gitWrapperLinuxX64 = new GitWrapperLinuxX64();
            Assert.AreEqual(typeof(GitWrapperLinuxX64), gitWrapperLinuxX64.GetType());
            Assert.IsFalse(await gitWrapperLinuxX64.IsGitAvailableAsync());

            await Assert.ThrowsExactlyAsync<PlatformNotSupportedException>(async () => await gitWrapperLinuxX64.ExtractArchiveAsync());
            Assert.ThrowsExactly<PlatformNotSupportedException>(gitWrapperLinuxX64.EnsureSupported);
        }

        [TestMethod]
        public void Test_02_UsableLinuxX64()
        {
            if (false == RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return;
            }

            var gitWrapperLinuxX64 = new GitWrapperLinuxX64();
            Assert.AreEqual(typeof(GitWrapperLinuxX64), gitWrapperLinuxX64.GetType());
            Assert.IsFalse(gitWrapperLinuxX64.IsGitAvailable());

            RemoveExtracted(
                new DirectoryInfo(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        gitWrapperLinuxX64.ExtractDirectory)));

            gitWrapperLinuxX64.ExtractArchive();
            Assert.IsTrue(gitWrapperLinuxX64.IsGitAvailable());


            var gitWrapperWindowsX64 = new GitWrapperWindowsX64();
            Assert.AreEqual(typeof(GitWrapperWindowsX64), gitWrapperWindowsX64.GetType());
            Assert.IsFalse(gitWrapperWindowsX64.IsGitAvailable());

            Assert.ThrowsExactly<PlatformNotSupportedException>(() => gitWrapperWindowsX64.ExtractArchive());
            Assert.ThrowsExactly<PlatformNotSupportedException>(gitWrapperWindowsX64.EnsureSupported);
        }

        [TestMethod]
        public async Task Test_02_UsableLinuxX64Async()
        {
            if (false == RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return;
            }

            var gitWrapperLinuxX64 = new GitWrapperLinuxX64();
            Assert.AreEqual(typeof(GitWrapperLinuxX64), gitWrapperLinuxX64.GetType());
            Assert.IsFalse(await gitWrapperLinuxX64.IsGitAvailableAsync());

            RemoveExtracted(
                new DirectoryInfo(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        gitWrapperLinuxX64.ExtractDirectory)));

            await gitWrapperLinuxX64.ExtractArchiveAsync();
            Assert.IsTrue(await gitWrapperLinuxX64.IsGitAvailableAsync());


            var gitWrapperWindowsX64 = new GitWrapperWindowsX64();
            Assert.AreEqual(typeof(GitWrapperWindowsX64), gitWrapperWindowsX64.GetType());
            Assert.IsFalse(await gitWrapperWindowsX64.IsGitAvailableAsync());

            await Assert.ThrowsExactlyAsync<PlatformNotSupportedException>(async () => await gitWrapperWindowsX64.ExtractArchiveAsync());
            Assert.ThrowsExactly<PlatformNotSupportedException>(gitWrapperWindowsX64.EnsureSupported);
        }

        [TestMethod]
        public void Test_03_FactoryLocalAndWindowsX64()
        {
            if (false == RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            IGitWrapper git; 
                
            git = GitWrapperFactory.Create(GitWrapperFactory.GitType.Local);
            Assert.AreEqual(typeof(GitWrapperSystem), git.GetType());
            Assert.IsTrue(git.IsGitAvailable());

            git = GitWrapperFactory.Create(GitWrapperFactory.GitType.Auto);
            Assert.AreEqual(typeof(GitWrapperWindowsX64), git.GetType());
            Assert.IsFalse(git.IsGitAvailable());
            git.ExtractArchive();
            Assert.IsTrue(git.IsGitAvailable());

            git = GitWrapperFactory.Create(GitWrapperFactory.GitType.Static);
            Assert.AreEqual(typeof(GitWrapperWindowsX64), git.GetType());
            Assert.IsFalse(git.IsGitAvailable());
            git.ExtractArchive();
            Assert.IsTrue(git.IsGitAvailable());
        }

        [TestMethod]
        public void Test_03_FactoryLocalAndLinuxX64()
        {
            if (false == RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return;
            }

            IGitWrapper git;

            git = GitWrapperFactory.Create(GitWrapperFactory.GitType.Local);
            Assert.AreEqual(typeof(GitWrapperSystem), git.GetType());
            Assert.IsTrue(git.IsGitAvailable());

            git = GitWrapperFactory.Create(GitWrapperFactory.GitType.Auto);
            Assert.AreEqual(typeof(GitWrapperLinuxX64), git.GetType());
            Assert.IsFalse(git.IsGitAvailable());
            git.ExtractArchive();
            Assert.IsTrue(git.IsGitAvailable());

            git = GitWrapperFactory.Create(GitWrapperFactory.GitType.Static);
            Assert.AreEqual(typeof(GitWrapperLinuxX64), git.GetType());
            Assert.IsFalse(git.IsGitAvailable());
            git.ExtractArchive();
            Assert.IsTrue(git.IsGitAvailable());
        }

        [TestMethod]
        public void Test_04_GitVersion()
        {
            IGitWrapper git;
            string version1, version2;

            var registered = GitWrapperFactory.Registered();

            foreach(var identifier in registered)
            {
                git = GitWrapperFactory.Create(identifier);

                if (git.IsSupported)
                {
                    git.ExtractArchive();

                    version1 = git.GitVersion();
                    version2 = git.Git("-v").StandardOutput;

                    Assert.AreEqual(version1, version2);
                    Assert.IsFalse(string.IsNullOrEmpty(version1));
                }
            }
        }

        [TestMethod]
        public async Task Test_04_GitVersionAsync()
        {
            IGitWrapper git;
            string version1, version2;

            var registered = GitWrapperFactory.Registered();

            foreach (var identifier in registered)
            {
                git = GitWrapperFactory.Create(identifier);

                if (git.IsSupported)
                {
                    await git.ExtractArchiveAsync();

                    version1 = await git.GitVersionAsync();
                    version2 = (await git.GitAsync("-v")).StandardOutput;

                    Assert.AreEqual(version1, version2);
                    Assert.IsFalse(string.IsNullOrEmpty(version1));
                }
            }
        }

        [TestMethod]
        public void Test_05_GitCloneWindowsX64()
        {
            if (false == RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            var git = new GitWrapperWindowsX64();

            git.ExtractArchive();

            var directory = new DirectoryInfo(RepositoryName);
            Assert.IsFalse(directory.Exists);

            var result = git.Git($"clone https://github.com/Chris82111/{RepositoryName}.git");
            Assert.AreEqual(0, result.ExitCode);
            Assert.AreEqual("", result.StandardOutput);
            Assert.AreEqual($"Cloning into '{RepositoryName}'...", result.StandardError);

            var oneFile = new FileInfo(Path.Combine(directory.FullName, "README.md"));
            Assert.IsTrue(oneFile.Exists);

            foreach (var file in Directory.EnumerateFiles(
                directory.FullName,
                "*",
                SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            Directory.Delete(directory.FullName, recursive: true);
        }

        [TestMethod]
        public async Task Test_05_GitCloneWindowsX64Async()
        {
            if (false == RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            var git = new GitWrapperWindowsX64();

            await git.ExtractArchiveAsync();

            var directory = new DirectoryInfo(RepositoryName);
            Assert.IsFalse(directory.Exists);

            var result = await git.GitAsync($"clone https://github.com/Chris82111/{RepositoryName}.git");
            Assert.AreEqual(0, result.ExitCode);
            Assert.AreEqual("", result.StandardOutput);
            Assert.AreEqual($"Cloning into '{RepositoryName}'...", result.StandardError);

            var oneFile = new FileInfo(Path.Combine(directory.FullName, "README.md"));
            Assert.IsTrue(oneFile.Exists);

            foreach (var file in Directory.EnumerateFiles(
                directory.FullName,
                "*",
                SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            Directory.Delete(directory.FullName, recursive: true);
        }

#pragma warning disable IDE0059 // Unnecessary assignment of a value, but this is an example

        [TestMethod]
        public void Test_Example1()
        {
            var git = GitWrapperFactory.Create();

            if (git.IsSupported)
            {
                git.ExtractArchive();

                if (git.IsGitAvailable())
                {
                    var version = git.GitVersion();

                    var dto = git.Mirror($"...");

                    if (0 != dto.ExitCode)
                    {
                        Console.WriteLine("Could not mirror.");
                    }
                }
            }
        }

        [TestMethod]
        public void Test_Example2()
        {
            // Required only for the test project. The system is specifically configured
            // so that an exception is always triggered when a different system is used.
            if (GitWrapperWindowsX64.ClassIdentifier != GitWrapper.Core.Helpers.RuntimeHelper.Identifier)
            {
                return;
            }

            var git = new GitWrapperWindowsX64();

            git.ExtractArchive();

            if (git.IsGitAvailable())
            {
                var version = git.GitVersion();

                var dto = git.Mirror($"...");

                if (0 != dto.ExitCode)
                {
                    Console.WriteLine("Could not mirror.");
                }
            }
        }

#pragma warning restore IDE0059

    }
}
