namespace Chris82111.GitManager.GitWrapper.Core
{
    /// <summary>
    /// Defines various actions that can be performed when extracting the archive
    /// </summary>
    public enum ExtractConditions
    {
        /// <summary>
        ///         Extracts only if the directory is missing or empty,
        /// <br/>   overwriting existing files when necessary
        /// </summary>
        IfDirectoryMissingOrEmpty = 0,

        /// <summary>
        /// Always extracts, overwriting existing files when necessary
        /// </summary>
        AlwaysOverwriteFiles,

        /// <summary>
        /// Clears the extraction directory before extracting
        /// </summary>
        ClearBeforeExtract,

        /// <summary>
        /// Clears the extraction directory before extracting if the directory has been moved
        /// </summary>
        ClearBeforeExtractIfMoved,
    }
}