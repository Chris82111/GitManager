namespace Chris82111.GitManager.GitWrapper.Core.Authenticators
{
    /// <summary>
    /// This enum lists the various authentication options that can be used
    /// </summary>
    public enum Credentials
    {
        /// <summary>
        ///         No environment variables are set or overwritten. All authentication details
        /// <br/>   and agents for the current system are used.
        /// </summary>
        Default = 0,

        /// <summary>
        ///         All forms of authentication will be removed. This setting means you can only
        /// <br/>   work with local or publicly available repositories
        /// </summary>
        Nothing,

        /// <summary>
        ///         A username and a token are used for authentication.
        /// <br/>   The function <see cref="IGitAuthentication.SetToken"/> must be used to set the data.
        /// </summary>
        Token,

        /// <summary>
        ///         A custom action is used to configure authentication.
        /// <br/>   To do this, the action <see cref="IGitAuthentication.UserAction"/> must be assigned.
        /// </summary>
        UserAction,


        /// <summary>
        ///         An SSH key file is used for authentication. This setting handles unencrypted or encrypted SSH key files.
        /// <br/>   The function <see cref="IGitAuthentication.SetSsh"/> must be used to set the data.
        /// </summary>
        Ssh,

        /// <summary>
        ///         An SSH key file is used for authentication. This setting can only handle unencrypted
        /// <br/>   SSH key files; if the key is protected by a passphrase, <see cref="Credentials.Ssh"/> must be used.
        /// <br/>   The function <see cref="IGitAuthentication.SetSsh"/> must be used to set the data.
        /// </summary>
        SshNoPassphrase
    }
}
