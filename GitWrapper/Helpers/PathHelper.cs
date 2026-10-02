using System.Diagnostics.CodeAnalysis;

namespace Chris82111.GitManager.GitWrapper.Core.Helpers
{
    /// <summary>
    /// Helps with working with paths
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Checks whether a directory is missing or empty
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <returns>
        ///         true, directory is missing or empty
        /// <br/>   false, directory exists and is not empty.</returns>
        public static bool IsDirectoryMissingOrEmpty(string path)
        {
            bool existsAndNotEmpty = Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any();
            return false == existsAndNotEmpty;
        }

        /// <summary>
        /// Changes the path separators to the system's default separator
        /// </summary>
        /// <param name="path">The input path</param>
        /// <returns>The path with the system's default separator</returns>
        [return: NotNullIfNotNull(nameof(path))]
        public static string? ReplacePathSeparatorsOnly(string? path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            return path.Replace(
                Path.AltDirectorySeparatorChar,
                Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Checks whether the path ends with the file extension
        /// </summary>
        /// <param name="path">The path to be checked</param>
        /// <param name="extension">The extension, which must match</param>
        /// <param name="stringComparison"></param>
        /// <returns>
        ///         true, the extension matches the one specified.
        /// <br/>   false, the extension does not match</returns>
        public static bool IsExtension(string path, string extension, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }

            return path.EndsWith(extension, stringComparison);
        }
    }
}
