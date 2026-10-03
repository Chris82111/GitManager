# Build 

Except for the [#License](./LICENSES.md#license) and [#Acknowledgment](./../README.md#acknowledgment) sections, the following sections are only relevant to developers. This applies to those who want to clone the repository and compile it themselves. They can be skipped when using NuGet.

Docker is required for the build process; WSL is also required on Windows.  
The build process for the Linux binaries takes approximately 10 minutes.

- `dotnet build /p:Configuration=Debug`
- `dotnet build /p:Configuration=Release`

## Build Tests

The project can be tested using the test project in the repository. The standard tests use the other projects as references. However, it is also possible to use the NuGet packages directly. To do so, they must be available—either by using a published version or by publishing a package locally [see here](#nuget).

- `dotnet test`
- `dotnet test -p:UseNuGetPackages=true`

## Build Settings

There are various ways to disable individual functions during the build:

1. The file `LibraryConfigDefaults.props` can be modified, but it is tracked by Git.
2. A new file `LibraryConfigOverrides.props` can be created next to the file `LibraryConfigDefaults.props` with the following content:
   
   ```c#
   <Project>
     <PropertyGroup>
       <EnableStaticGitUseLinux>false</EnableStaticGitUseLinux>
       <EnableStaticGitUseWindows>false</EnableStaticGitUseWindows>
     </PropertyGroup>
   </Project>
   ```
   
3. An environment variable can override the properties
4. A custom property can override the properties:  
   `dotnet build -c Debug -p:EnableStaticGitUseLinux=false -p:EnableStaticGitUseWindows=false`

## Clean

When the project is cleaned up, a `.docker-no-cache` file is created in the project folder `GitWrapper.Static.LinuxX64`. This file is deleted when the project is recompiled, but it ensures that the next Docker build process is performed without using the cache. However, the build is only performed if there is no packaged file in the `runtimes` folder. It is allowed to create or delete the file manually.

You can clean up the project using the command:

```c#
dotnet clean
```

## Updating the Runtime Files 

The version of Git for Windows (MinGit) can be specified in the file [GitWindows.props](./../GitWrapper.Static.WindowsX64/Lib/GitWindows.props).  
The individual versions of Git for Linux can be specified in the [Dockerfile](./../GitWrapper.Static.LinuxX64/Lib/Dockerfile) file.

## Docker

The entire build process requires many prerequisites. To always provide the same build environment and avoid complications with the host operating system, the build is performed in a container. For this reason, Docker is required to create a portable Git for Linux.

### Docker on Windows

1. See the Microsoft description: [How to install Linux on Windows with WSL](https://learn.microsoft.com/en-us/windows/wsl/install)
2. Start PowerShell in administrator mode.
3. Install WSL: `wsl --install`
4. Restart the computer
5. The installation program will start automatically, follow the descriptions
6. Open the Run dialog box with Windows key + R and enter `wsl`
7. See the Docker description: [Install Docker Engine](https://docs.docker.com/engine/install)
8. Follow the description.
9. `wsl sudo sudo usermod -aG docker $USER`
10. `wsl --shutdown`
11. Verify the installation,
    1. from inside WSL: `docker run hello-world`
    2. from cmd/PowerShell: `wsl docker run hello-world`

### Docker on Linux

Docker must be installed on Linux.

### Docker Errors

Here is a list of common mistakes:

1. Mounted NTFS drive:

   ```shell
   EXEC : error : failed to solve: failed to read dockerfile: open Dockerfile: no such file or directory
       GitManager/GitWrapper.Static.LinuxX64/Lib/GitLinux.targets(27,5): error MSB3073: The command "docker build --progress=plain -f GitManager/GitWrapper.Static.LinuxX64/Lib/Dockerfile -t staticgitbuildtempimage GitManager/GitWrapper.Static.LinuxX64/Lib" exited with code 1.
   ```
   
   Do not use an NTFS-mounted drive. This can cause Docker to be unable to find the `Dockerfile` file.

2. Permission denied:
   
   ```shell
   GitManager failed with 2 error(s) (0.1s)
       EXEC : error : permission denied while trying to connect to the Docker daemon socket at unix:///var/run/docker.sock: Head "http://%2Fvar%2Frun%2Fdocker.sock/_ping": dial unix /var/run/docker.sock: connect: permission denied
       GitManager/GitWrapper.Static.LinuxX64/Lib/GitLinux.targets(21,5): error MSB3073: The command "docker build --progress=plain -t staticgitbuildtempimage ." exited with code 1.
   ```
   
   To provoke this error, you can run `docker ps` in the terminal. An error should then appear due to insufficient permissions. This error can be resolved as follows. The current user must be added to the Docker group. Enter the following commands:
   
   1. Checks whether the group exists:  
      `getent group docker`  
      If it prints a line like `docker:x:AnyNumer:` then the group exists.  
      If it prints nothing, the group does not exist.  
   
   2. Only create a group if it does not exist; sudo is required:  
      `if ! getent group docker > /dev/null ; then sudo groupadd docker ; fi`  
   
   3. Add your user to the docker group:  
      `sudo usermod -aG docker $USER`
   
   Then log out and back in (or reboot) so the group takes effect.

## NuGet

### Create a Local NuGet Package Feed

The following command lists all packet sources:

```shell
dotnet nuget list source
```

The following commands create a local NuGet package feed named `local`:

#### Linux:

```shell
mkdir -p ~/.NuGetPackages
dotnet nuget add source ~/.NuGetPackages -n local
```

#### Windows:

```powershell
mkdir C:\NuGetPackages
dotnet nuget add source C:\NuGetPackages\ -n local
```

### Publishing Your Own Version

The following command creates a NuGet package and transfers it to a local package feed named `local`:  

1. Navigate to the repository directory.
2. Change the Version in the files `*.csproj` in the tag `Project/PropertyGroup/Version`, #Major.#Minor.#Patch.
3. Creating all NuGet packages:
   
   ```shell
   dotnet pack -c Release -o .
   ```
   
4. Once you create a NuGet package it can be published to the local package feed:  
   
   ```shell
   dotnet nuget push Chris82111.GitManager.GitWrapper.Core.1.0.0.nupkg -s local
   dotnet nuget push Chris82111.GitManager.GitWrapper.Static.WindowsX64.1.0.0.nupkg -s local
   dotnet nuget push Chris82111.GitManager.GitWrapper.Static.LinuxX64.1.0.0.nupkg -s local
   ```
   
5. Clear the NuGet caches, this is only necessary if the version number is reused:  
   
   ```shell
   dotnet nuget locals all --clear
   ```

5. Restart Visual Studio

6. Run tests:
   
   ```shell
   dotnet test
   dotnet test -p:UseNuGetPackages=true
   ```

### Additional Commands

Lists all versions of a NuGet package that are available in your configured package sources:

```shell
dotnet package search Chris82111.GitManager.GitWrapper.Core.1.0.0.nupkg --exact-match
dotnet package search Chris82111.GitManager.GitWrapper.Static.WindowsX64.1.0.0.nupkg --exact-match
dotnet package search Chris82111.GitManager.GitWrapper.Static.LinuxX64.1.0.0.nupkg --exact-match
```
