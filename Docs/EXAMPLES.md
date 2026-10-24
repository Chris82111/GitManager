# Examples

Back to [README.md](./../README.md)  

This file contains a few examples.

## Example 2, Platform-Specific Implementation

The following example shows a platform-specific implementation, in this case Windows 64-bit:

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

## Example 3, Platform-Independent Implementation With Token

The following example shows a platform-independent implementation where a token is set in the source code:

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

## Example 4, Platform-Independent Implementation With Token Via an Environment Variable

The following example shows a platform-independent implementation that sets an environment variable containing the token. The token is then read from the environment variable at startup:

```c#
public void Test_Example4()
{
    var git = GitWrapperFactory.Create();

    git.ExtractArchive();
    git.Credential = Credentials.Token;
    git.Authentication.SetTokenFromEnvironment(null, "GIT_MANAGER_TOKEN");

    var dto = git.Mirror($"...");    
}
```

## Example 5, Platform-Independent Implementation With SSH

The following example shows a platform-independent implementation. An SSH key file is used for authentication:

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

## Example 6, Platform-Independent Implementation Authentication Reused

The following example shows a platform-independent implementation. In this example, authentication is assigned to several objects and is therefore reused:

```c#
public async Task Test_Example6()
{
    var git1 = GitWrapperFactory.Create();
    var git2 = GitWrapperFactory.Create();

    await git1.ExtractArchiveAsync();
    await git2.ExtractArchiveAsync();

    var auth = new GitAuthenticationBase();
    auth.SetSsh(_configData!.SshFilePwProtected, _configData.SshFilePassword);

    git1.Authentication = auth;
    git2.Authentication = auth;
    
    git1.Credential = Credentials.Ssh;
    git2.Credential = Credentials.Ssh;

    var dto1 = await git1.MirrorAsync($"...", dir1);
    var dto2 = await git2.MirrorAsync($"...", dir2);
```
