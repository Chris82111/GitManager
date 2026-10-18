namespace Chris82111.GitManager.GitWrapper.Core.Authenticators
{
    public interface IGitAuthentication
    {
        string Name { get; set; }

        int DependentCount { get; }

        public bool IsDisposed { get; }

        void Dispose();

        void Connect(IGitWrapper gitWrapper);

        void Disconnect(IGitWrapper gitWrapper);

        bool IsTokenValid();

        // Personal access token
        void SetToken(string? username, string token);

        void ClearToken();

        void CredentialChangedHandler(object? sender, CredentialChangedEventArgs e);
    }
}
