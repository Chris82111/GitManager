namespace Chris82111.GitManager.GitWrapper.Core.Authenticators
{
    /// <summary>
    /// Provides data for an event raised when credentials change
    /// </summary>
    public class CredentialChangedEventArgs(Credentials previousCredential, Credentials credential) : EventArgs
    {
        /// <summary>
        /// Credentials before the change
        /// </summary>
        public Credentials PreviousCredential { get; } = previousCredential;

        /// <summary>
        /// Credentials after the change
        /// </summary>
        public Credentials Credential { get; } = credential;
    }
}
