# GitManager

<div align="center">

  [![Shields](https://img.shields.io/badge/.NET-10.0-5C2D91)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0 "Download .NET 10.0")
  [![Visual Studio](https://img.shields.io/badge/IDE-Visual_Studio-5C2D91)](https://visualstudio.microsoft.com/ "Download Visual Studio")
  ![Linux x64](https://img.shields.io/badge/Linux-x64-009639)
  ![Windows x64](https://img.shields.io/badge/Windows-x64-0067C0)
  [![MIT](https://img.shields.io/badge/Licence-MIT-3ea63a)](https://spdx.org/licenses/MIT.html "Link to SPDX")
  ![Tests](https://github.com/Chris82111/GitManager/actions/workflows/test.yml/badge.svg?branch=task-github_action)

</div>

This repository offers a convenient, cross-platform solution for using Git in a portable form for Linux and Windows. As a powerful wrapper, it abstracts the complexity of Git integration and enables quick and easy integration into existing development environments.

Integration can be done either directly via [NuGet.org](https://www.nuget.org/) or by cloning the repository. This allows the package to be used flexibly either as a direct reference or to set up your own local NuGet package feed - ideal for controlled build environments and enterprise applications.

> [!WARNING]
> Please note that the greatest advantage of this project is also its greatest weakness. Since these are static versions of Git, there are no updates - not even security updates.

## Example

The following example shows a platform-independent implementation:

```c#
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
```

More examples are listed here [EXAMPLES.md](./Docs/EXAMPLES.md).

## Build

The [BUILD.md](./Docs/BUILD.md) section is only relevant to developers or people who will be build this repository.

## Security Considerations

GitManager pass sensitive credentials, such as tokens and SSH passphrases, to Git through environment variables of the started process. 
This avoids exposing the credentials as command-line arguments, but environment variables are not a secure secret store. Depending on the operating system and the privileges available to other processes, the environment of a running process may potentially be inspected by other software, debugging tools, or malicious processes.
Use GitManager only in a trusted environment and protect the machine and user account from unauthorized access. If a token or SSH passphrase is compromised, revoke or replace the affected credential immediately.

Password Storage: Passwords are currently stored as string instances in memory. Since strings are immutable, they cannot be reliably erased from memory, and sensitive data may remain accessible until garbage collection reclaims it.

## License

This repository has the [MIT](https://spdx.org/licenses/MIT.html) license ([LICENSE](LICENSE) file), but it uses many other projects, each of which has its own license that must be observed, see [THIRD_PARTY_LICENSES](THIRD_PARTY_LICENSES). The [LICENSES.md](./Docs/LICENSES.md) section provides a better overview of the various licenses.  

## Acknowledgment

This repository provides a wrapper that builds on other essential projects. To acknowledge these foundational projects and their developers, the projects used are listed below:  

- Zlib: This product includes software developed by the Zlib project. (https://www.zlib.net/) 
- OpenSSL: This product includes software developed by the OpenSSL project for use in the OpenSSL Toolkit (http://www.openssl.org/)
- libpsl: This product includes software developed by the libpsl project. (https://github.com/rockdaboot/libpsl/tree/master)
- libcurl: This product includes software developed by the curl project. (https://curl.se/)
- Git: This product includes software developed by the Git project. (https://git-scm.com/)
- SSH.NET: This product includes software developed by the SSH.NET project. (https://github.com/sshnet/SSH.NET)
