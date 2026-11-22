# Verify Git Source Files Using GPG Signatures

Git uses GPG (GNU Privacy Guard) signatures to verify the authenticity and integrity of its source files. By checking the signature against the developer's public key, you can confirm that the downloaded archive matches the file signed by the maintainer and has not been modified. The following instructions and commands are intended for Windows using PowerShell.

## Remove Junio C Hamano's public key from your GPG keyring

1. Delete the imported public key by running the following command.  
   GPG will ask you to confirm the deletion:

   ```PowerShell
   gpg --delete-key 96E07AF25771955980DAD10020D04E5A713660A7
   ```

2. Check that the key is gone:

   ```PowerShell
   gpg --fingerprint 96E07AF25771955980DAD10020D04E5A713660A7
   ```

## Download files and verify the signature

1. Go to the [Download](https://www.kernel.org/pub/software/scm/git/) page and download the latest Git source archive and its signature, for example:

   ```PowerShell
   curl.exe -OL https://www.kernel.org/pub/software/scm/git/git-2.56.1.tar.gz
   curl.exe -OL https://www.kernel.org/pub/software/scm/git/git-2.56.1.tar.sign
   ```

2. Decompress the archive using the following single-line command:

   ```PowerShell
   $src = [IO.File]::OpenRead("$PWD\git-2.56.1.tar.gz"); try { $dst = [IO.File]::Create("$PWD\git-2.56.1.tar"); try { $gz = [IO.Compression.GzipStream]::new($src, [IO.Compression.CompressionMode]::Decompress); try { $gz.CopyTo($dst) } finally { $gz.Dispose() } } finally { $dst.Dispose() } } finally { $src.Dispose() }
   ```

3. Verify the signature:

   ```PowerShell
   gpg --verify .\git-2.56.1.tar.sign .\git-2.56.1.tar
   ```

## Retrieve and import the public key

1. Initialize a temporary Git repository:

   ```PowerShell
   git init .\git-key-temp
   ```

2. Fetch the official Git tag containing the public key:

   ```PowerShell
   git -C .\git-key-temp fetch --depth=1 https://gitlab.com/gitlab-org/git.git refs/tags/junio-gpg-pub
   ```

3. Export the actual public key:

   ```PowerShell
   git -C .\git-key-temp cat-file blob 'FETCH_HEAD^{blob}' | Set-Content -Encoding ascii .\junio-gpg-pub-complete.asc
   ```

4. Import the public key. GPG should report that the public key was imported:

   ```PowerShell
   gpg --import .\junio-gpg-pub-complete.asc
   ```

5. Check the fingerprint and verify that it matches the following value:

   ```PowerShell
   gpg --fingerprint 96E07AF25771955980DAD10020D04E5A713660A7
   ```

   Expected primary fingerprint:

   ```text
   96E0 7AF2 5771 9559 80DA D100 20D0 4E5A 7136 60A7
   ```

6. Verify the signature:

   ```PowerShell
   gpg --verify .\git-2.56.1.tar.sign .\git-2.56.1.tar
   ```