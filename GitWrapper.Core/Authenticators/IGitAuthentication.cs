using Chris82111.GitManager.GitWrapper.Core.Processes;

namespace Chris82111.GitManager.GitWrapper.Core.Authenticators
{
    public interface IGitAuthentication
    {
        string Name { get; set; }

        int DependentCount { get; }

        public bool IsDisposed { get; }

        // void UserActionHandler(object? sender, ProcessEventArgs e)
        // {
        //     var envVar = new ProcessEnvironmentVariable(e.Process.StartInfo);
        // }
        Action<object?, ProcessEventArgs>? UserAction { get; set; }

        void Dispose();

        void Connect(IGitWrapper gitWrapper);

        void Disconnect(IGitWrapper gitWrapper);

        bool IsTokenValid();

        // Personal access token
        void SetToken(string? username, string token);

        void ClearToken();

        bool IsSshValid();

        void SetSsh(string filePath, string password);

        void ClearSsh();

        void CredentialChangedHandler(object? sender, CredentialChangedEventArgs e);
    }
}
