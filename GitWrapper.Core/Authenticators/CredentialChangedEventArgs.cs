namespace Chris82111.GitManager.GitWrapper.Core.Authenticators
{
    public class CredentialChangedEventArgs(Credentials previousCredential, Credentials credential) : EventArgs
    {
        public Credentials PreviousCredential { get; } = previousCredential;
        public Credentials Credential { get; } = credential;
    }
}
