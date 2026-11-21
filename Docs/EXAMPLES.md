# Examples

Back to [README.md](./../README.md)  

This file contains a few examples.

## Example 2: Platform-Specific Implementation for Windows x64

This example demonstrates a platform-specific implementation of Git operations using the Windows x64 Git wrapper directly, without relying on the platform-independent `GitWrapperFactory`.

The `GitWrapperWindowsX64` class is instantiated explicitly, ensuring that the Windows x64 implementation is used.

The example demonstrates the following:

- **Platform-specific implementation:**  
  The Windows x64 Git wrapper is instantiated directly.
- **Git extraction:**  
  The bundled Git executable is extracted using `ExtractArchive()`.
- **Git availability check:**  
  `IsGitAvailable()` verifies that Git is available before attempting further operations.
- **Git version detection:**  
  `GitVersion()` retrieves the installed Git version.
- **Repository mirroring:**  
  `Mirror()` executes the mirroring operation against the specified remote repository.
- **Error handling:**  
  The returned `ExitCode` is checked to determine whether the mirroring operation completed successfully.

```c#
public void Test_Example2()
{
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
```

## Example 3: Platform-Independent Implementation with Token Authentication

This example demonstrates a platform-independent implementation of Git operations using token-based authentication.

An access token is provided directly in the source code and configured through the authentication object. GitManager uses this token to authenticate with the remote Git repository during the mirror operation.

The example demonstrates the following:

- **Platform independence:**  
  The same implementation can be used across supported operating systems without requiring platform-specific code.
- **Token-based authentication:**  
  An access token is used instead of SSH keys or username and password authentication.
- **Authentication configuration:**  
  The token is assigned using `SetToken()`, allowing it to be reused by the configured Git wrapper instance.
- **Repository access:**  
  The configured token is used to authenticate with the remote Git repository during the mirror operation.

```c#
public void Test_Example3()
{
    var git = GitWrapperFactory.Create();

    git.ExtractArchive();
    git.Credential = Credentials.Token;
    git.Authentication.SetToken(null, "ghp_***");

    var dto = git.Mirror($"...");    
}
```

## Example 4: Platform-Independent Implementation with Token Authentication via an Environment Variable

This example demonstrates a platform-independent implementation of Git operations using token-based authentication through an environment variable.

Instead of providing the access token directly in the code, the name of an environment variable containing the token is supplied during authentication configuration. GitManager then retrieves the token from that environment variable when authentication is required.

The example demonstrates the following:

- **Platform independence:**  
  The same implementation can be used across supported operating systems without requiring platform-specific code.
- **Environment-based authentication:**  
  The access token is retrieved from an environment variable named `GIT_MANAGER_TOKEN`.
- **Improved credential handling:**  
  The actual token does not need to be stored directly in the source code or passed explicitly to the authentication configuration.
- **Repository access:**  
  The configured token is used to authenticate with the remote Git repository during the mirror operation.

Before running the example, ensure that the `GIT_MANAGER_TOKEN` environment variable is set and contains a valid access token with the necessary repository permissions.

```c#
public void Test_Example4()
{
    var git = GitWrapperFactory.Create();

    git.ExtractArchive();
    git.Credential = Credentials.TokenFromEnvironment;
    git.Authentication.SetTokenFromEnvironment(null, "GIT_MANAGER_TOKEN");

    var dto = git.Mirror($"...");    
}
```

## Example 5: Platform-Independent Implementation with SSH Authentication

This example demonstrates a platform-independent implementation of Git operations using SSH key-based authentication.

An SSH private key file is used to authenticate with the remote Git repository. If the private key is encrypted, its passphrase is provided when configuring the authentication.

The example demonstrates the following:

- **Platform independence:**  
  The same implementation can be used across supported operating systems without requiring platform-specific code.
- **SSH authentication:**  
  Authentication is performed using an SSH private key instead of a username and password or an access token.
- **Encrypted private keys:**  
  An optional passphrase can be supplied to unlock an encrypted SSH private key.
- **Repository access:**  
  The repository is accessed using its SSH URL (for example, `git@github.com:USER/REPO.git`).

```c#
public void Test_Example5()
{
    var git = GitWrapperFactory.Create();

    git.ExtractArchive();
    git.Credential = Credentials.Ssh;
    git.Authentication.SetSsh("path/to/key/file", "theSecretPassphrase");

    // git@github.com:USER/REPO.git
    var dto = git.Mirror($"...");    
}
```

## Example 6: Platform-Independent Implementation with Reusable Authentication and Parallel Execution

This example demonstrates a platform-independent implementation of Git operations using token-based authentication.

A single authentication object is configured and assigned to multiple Git wrapper instances. This allows the same authentication credentials to be reused across multiple Git operations without having to configure them individually.

The example demonstrates the following:

- **Platform independence:**  
  The same implementation can be used across supported operating systems without requiring platform-specific code.
- **Reusable authentication:**  
  A single `GitAuthenticationBase` instance is shared between multiple Git wrapper instances.
- **Token-based authentication:**  
  An access token is configured once and reused for both Git operations.
- **Asynchronous execution:**  
  Asynchronous methods (`async`/`await`) are used to execute Git operations without blocking the calling thread.
- **Parallel execution:**  
  `Task.WhenAll()` starts both mirroring operations concurrently and waits until both have completed.
- **Result handling:**  
  The results of both operations are collected and accessed individually after completion.

```c#
public async Task Test_Example6()
{
    var git1 = GitWrapperFactory.Create();
    var git2 = GitWrapperFactory.Create();

    await Task.WhenAll(
        git1.ExtractArchiveAsync(),
        git2.ExtractArchiveAsync());

    var auth = new GitAuthenticationBase();
    auth.SetToken(null, "ghp_***");

    git1.Authentication = auth;
    git2.Authentication = auth;

    git1.Credential = Credentials.Token;
    git2.Credential = Credentials.Token;

    var results = await Task.WhenAll(
        git1.MirrorAsync($"...", dir1),
        git2.MirrorAsync($"...", dir2));

    var dto1 = results[0];
    var dto2 = results[1];
}
```

## Example 7: Platform-Independent SSH Authentication with Reusable Credentials, Asynchronous Execution, and a Custom `known_hosts` File

This example demonstrates a platform-independent implementation of SSH authentication using GitManager.

The authentication configuration can be reused across multiple Git operations. Asynchronous methods (`async`/`await`) allow other tasks to continue executing while an operation is in progress, without blocking the calling thread.

The example also demonstrates the following:

- **SSH key file permissions:**  
  On supported UNIX-based systems, the necessary file permissions are applied to the SSH private key using `SecureSshFile()`.
- **SSH key type detection:**  
  The first detected key type is retrieved using `SshKeyTypes().First()` and used to validate the expected host key.
- **Custom `known_hosts` file:**  
  Instead of modifying the user's or system-wide `known_hosts` files, a separate file named `known_hosts` is created in the current working directory.
- **Host verification:**  
  GitHub's expected ED25519 host key fingerprint is verified and added to the custom `known_hosts` file before the Git operation begins.
- **Explicit host verification:**  
  By setting `KnownHostsFileSelection.ListedFiles`, only the explicitly configured `known_hosts` files are used for host verification.
- **Reusable authentication:**  
  The configured authentication object can be assigned to multiple Git operations without having to configure the SSH credentials again.

This approach allows SSH authentication and host verification to be configured independently of the operating system while avoiding modifications to existing user or system SSH configuration files.

```c#
public async Task Test_Example7()
{
    var git = GitWrapperFactory.Create();

    await git.ExtractArchiveAsync();

    var auth = new GitAuthenticationBase();
    auth.SetSsh("path/to/key/file", "theSecretPassphrase");
    auth.SecureSshFile();

    var keyType = auth.SshKeyTypes().First();

    var knownHostFile = await KnownHostMultiton.InstanceAsync("known_hosts");
    await knownHostFile.VerifyAndAddHostAsync("github.com", keyType, 
        "+DiY3wvvV6TuJJhbpZisF/zLDA0zPMSvHdkr4UvCOqU");
    await knownHostFile.WriteAsync();

    auth.KnownHostsFiles.Add(knownHostFile);
    auth.KnownHostsFileSelection = KnownHostsFileSelection.ListedFiles;

    git.Authentication = auth;

    git.Credential = Credentials.Ssh;

    var dto1 = await git.MirrorAsync(_configData.PrivateRepoSsh, dir);
}
```
