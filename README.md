# GitManager

<div align="center">

  [![Shields](https://img.shields.io/badge/.NET-10.0-5C2D91)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0 "Download .NET 10.0")
  [![Visual Studio](https://img.shields.io/badge/IDE-Visual_Studio-5C2D91)](https://visualstudio.microsoft.com/ "Download Visual Studio")
  ![Linux x64](https://img.shields.io/badge/Linux-x64-009639)
  ![Windows x64](https://img.shields.io/badge/Windows-x64-0067C0)
  [![MIT](https://img.shields.io/badge/Licence-MIT-3ea63a)](https://spdx.org/licenses/MIT.html "Link to SPDX")

  [![Docker Engine](https://img.shields.io/badge/build_dependency-Docker_Engine-0d4dF2)](https://docs.docker.com/engine/install "Link to web page")
  [![WSL](https://img.shields.io/badge/build_dependency_(Windows)-WSL-00BCF2)](https://learn.microsoft.com/en-us/windows/wsl/install "Link to web page")

  [![Git for Windows](https://img.shields.io/badge/dependency_(Windows)-Git_for_Windows-F1502F)](https://github.com/git-for-windows/git "Link to repository")
  [![zlib, dependency for Git for Linux](https://img.shields.io/badge/dependency_(Linux)-zlib-2C652C)](https://zlib.net/ "Link to web page")
  [![OpenSSL, dependency for Git for Linux](https://img.shields.io/badge/dependency_(Linux)-OpenSSL-731513)](https://github.com/openssl/openssl/ "Link to repository")
  [![libpsl, dependency for Git for Linux](https://img.shields.io/badge/dependency_(Linux)-libpsl-394E79)](https://www.linuxfromscratch.org/blfs/view/svn/basicnet/libpsl.html "Link to web page")
  [![curl, dependency for Git for Linux](https://img.shields.io/badge/dependency_(Linux)-curl-093754)](https://curl.se/download.html "Link to web page")
  [![Git](https://img.shields.io/badge/dependency_(Linux)-Git_(source_code)-F1502F)](https://git-scm.com/install/source "Link to web page")

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

## Build

The [BUILD.md](./Docs/BUILD.md) section is only relevant to developers or people who will be build this repository.

## License

This repository has the [MIT](https://spdx.org/licenses/MIT.html) license ([LICENSE](LICENSE) file), but it uses many other projects, each of which has its own license that must be observed, see [THIRD_PARTY_LICENSES](THIRD_PARTY_LICENSES). The [LICENSES.md](./Docs/LICENSES.md) section provides a better overview of the various licenses.  

## Acknowledgment

This repository provides a wrapper that builds on other essential projects. To acknowledge these foundational projects and their developers, the projects used are listed below:  

- Zlib: This product includes software developed by the Zlib Project. (https://www.zlib.net/) 
- OpenSSL: This product includes software developed by the OpenSSL Project for use in the OpenSSL Toolkit (http://www.openssl.org/)
- libpsl: This product includes software developed by the libpsl Project. (https://github.com/rockdaboot/libpsl/tree/master)
- libcurl: This product includes software developed by the curl Project. (https://curl.se/)
- Git: This product includes software developed by the Git Project. (https://git-scm.com/)
