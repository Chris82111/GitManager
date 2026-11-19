namespace Chris82111.GitManager.GitWrapper.Core.Authenticators
{
    /// <summary>
    /// Specifies which known_hosts files are used for SSH host verification.
    /// </summary>
    public enum KnownHostsFileSelection
    {
        /// <summary>
        /// Disables the use of all known_hosts files.
        /// </summary>
        None = 0,

        /// <summary>
        /// Uses only the explicitly listed known_hosts files.
        /// </summary>
        ListedFiles,

        /// <summary>
        /// Uses the standard OpenSSH user and system known_hosts files.
        /// </summary>
        SystemStandard,
    }
}
