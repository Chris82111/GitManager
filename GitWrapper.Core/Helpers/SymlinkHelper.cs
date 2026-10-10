using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Chris82111.GitManager.GitWrapper.Core.Helpers
{
    /// <summary>
    /// Helps with working with symlinks (only Linux)
    /// </summary>
    public static partial class SymlinkHelper
    {
        /// <summary>
        /// Creates a symbolic link at the specified path that points to the specified target.
        /// </summary>
        /// <param name="target">The path of the file or directory to which the symbolic link points.</param>
        /// <param name="linkpath">The path at which to create the symbolic link.</param>
        /// <returns>
        ///         0 if the symbolic link was created successfully; 
        /// <br/>   -1 otherwise
        /// </returns>
        [SupportedOSPlatform("linux")]
        [LibraryImport(
            "libc",
            EntryPoint = "symlink",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf8)]
        private static partial int Symlink(string target, string linkpath);

        /// <summary>
        /// Ensures that symbolic links are supported in the specified directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void EnsureSymlinkSupported(string? directory = null)
        {
            if (false == RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new PlatformNotSupportedException(
                    "Symlink support check using libc.symlink() is only valid on Linux. " +
                    "Do not call this method on Windows.");
            }

            if (string.IsNullOrEmpty(directory))
            {
                directory = ".";
            }

            if (false == Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }


            string linkPath = Path.Combine(directory, "symlink_test_link");

            // First, check write access
            if (false == IsWritable(directory))
            {
                throw new InvalidOperationException(
                    $"Cannot write to the directory '{directory}'. Extraction requires write permissions.");
            }

            // Now attempt to create a symlink to a nonexistent target

#pragma warning disable CA1416 // Method does have an runtime check
            int result = Symlink("symlink_test_target_nonexistent", linkPath);
#pragma warning restore CA1416

            if (0 != result)
            {
                int errno = Marshal.GetLastWin32Error();
                throw new InvalidOperationException(
                    $"Cannot create symlinks in the directory '{directory}'. Filesystem may not support symlinks (errno={errno}).");
            }

            // Cleanup
            File.Delete(linkPath);
        }

        private static bool IsWritable(string directory)
        {
            try
            {
                string testFile = Path.Combine(directory, "write_test.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
