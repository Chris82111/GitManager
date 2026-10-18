using Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables;
using Chris82111.GitManager.GitWrapper.Core.Processes;
using System.Runtime.InteropServices;

namespace Chris82111.GitManager.GitWrapper.Core.Authenticators
{
    public class GitAuthenticationBase : IGitAuthentication, IDisposable
    {
        public string Name { get; set; } = string.Empty;

        public int DependentCount => _gitWrappers.Count;

        public bool IsDisposed => _disposed;

        public Action<object?, ProcessEventArgs>? UserAction { get; set; } = null;

        protected readonly List<IGitWrapper> _gitWrappers = new List<IGitWrapper>();

        /// <summary>
        /// Gets the platform-specific path to the null device, which discards all written data.
        /// </summary>
        protected readonly string _nullDevice = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "NUL"
            : "/dev/null";

        private bool _disposed;


        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                var gitWrappers = _gitWrappers.ToList();
                foreach (var gitWrapper in gitWrappers)
                {
                    Disconnect(gitWrapper);
                }

                _gitWrappers.Clear();
            }

            _disposed = true;
        }

        #endregion


        public void Connect(IGitWrapper gitWrapper)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (false == _gitWrappers.Contains(gitWrapper))
            {
                CredentialChangedHandler(gitWrapper, new CredentialChangedEventArgs(Credentials.Default, gitWrapper.Credential));
                gitWrapper.CredentialChanged += CredentialChangedHandler;
                _gitWrappers.Add(gitWrapper);
            }
        }

        public void Disconnect(IGitWrapper gitWrapper)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            gitWrapper.CredentialChanged -= CredentialChangedHandler;
            CredentialChangedHandler(gitWrapper, new CredentialChangedEventArgs(gitWrapper.Credential, Credentials.Default));
            _gitWrappers.Remove(gitWrapper);
        }


        #region Token

        protected string? _username = null;

#warning Not currently encrypted
        protected string? _token = null;

        public bool IsTokenValid()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            return
                false == string.IsNullOrWhiteSpace(_username) ||
                false == string.IsNullOrWhiteSpace(_token);
        }

        public void SetToken(string? username, string token)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            _username = username ?? "oauth2";
            _token = token;
        }

        public void ClearToken()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            _username = null;
            _token = null;
        }

        #endregion


        public void CredentialChangedHandler(object? sender, CredentialChangedEventArgs e)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            IGitWrapper gitWrapper = (IGitWrapper)sender!;

            // Reset
            switch (e.PreviousCredential)
            {
                case Credentials.Default:
                    break;

                case Credentials.Nothing:
                    gitWrapper.BeforeProcessStart -= AuthenticationNothing;
                    break;

                case Credentials.Token:
                    gitWrapper.BeforeProcessStart -= AuthenticationToken;
                    break;

                case Credentials.UserAction:
                    gitWrapper.BeforeProcessStart -= AuthenticationUserAction;
                    break;

                default:
                    break;
            }

            // Set
            switch (e.Credential)
            {
                case Credentials.Default:
                    break;

                case Credentials.Nothing:
                    gitWrapper.BeforeProcessStart += AuthenticationNothing;
                    break;

                case Credentials.Token:
                    gitWrapper.BeforeProcessStart += AuthenticationToken;
                    break;

                case Credentials.UserAction:
                    gitWrapper.BeforeProcessStart += AuthenticationUserAction;
                    break;

                default:
                    break;
            }
        }


        #region Logic

        protected virtual void AuthenticationNothing(object? sender, ProcessEventArgs e)
        {
            var envVar = new ProcessEnvironmentVariable(e.Process.StartInfo);

            // --- Credential management / Git Credential Manager ---
            envVar.Set("GCM_INTERACTIVE", "Never");                // Never prompt interactively for credentials (Git Credential Manager)
            envVar.Set("GCM_CREDENTIAL_STORE", "none");            // Disable storing credentials in any secure store
            envVar.Set("GIT_CREDENTIAL_MANAGER_ENABLED", "false"); // Disable Git Credential Manager entirely

            // --- Config file overrides ---
            envVar.Set("GIT_CONFIG_NOSYSTEM", "1");      // Ignore system-wide Git configuration
            envVar.Set("GIT_CONFIG_GLOBAL", _nullDevice); // Ignore user global config (point to null device)
            envVar.Set("GIT_CONFIG_SYSTEM", _nullDevice); // Ignore system-wide config file (point to null device)

            // --- Non-interactive mode / disable prompts ---
            envVar.Set("GIT_TERMINAL_PROMPT", "0");    // Disable Git terminal prompts for credentials
            envVar.Set("GIT_ASKPASS", "echo");         // Disable Git GUI credential prompts (fallback to echo)
            envVar.Set("SSH_ASKPASS", "echo");         // Disable SSH GUI prompts (fallback to echo)
            envVar.Set("SSH_AUTH_SOCK", string.Empty); // Disable SSH agent (ignore any running agent)
        }

        protected virtual void AuthenticationToken(object? sender, ProcessEventArgs e)
        {
            var envVar = new ProcessEnvironmentVariable(e.Process.StartInfo);

            if (false == IsTokenValid())
            {
                throw new NullReferenceException($"Error with internal stored username and/or token call the methode '{nameof(SetToken)}' first.");
            }

            // Disable inherited credential helpers.
            envVar.Set("GIT_CONFIG_COUNT", "2");

            envVar.Set("GIT_CONFIG_KEY_0", "credential.helper");
            envVar.Set("GIT_CONFIG_VALUE_0", string.Empty);

            envVar.Set("GIT_CONFIG_KEY_1", "credential.helper");

            string helper =
                $"!f() {{ printf 'username=%s\\npassword=%s\\n\\n' " +
                $"{ShellQuote(_username ?? string.Empty)} {ShellQuote(_token ?? string.Empty)}; }}; f";

            envVar.Set("GIT_CONFIG_VALUE_1", helper);
        }

        protected virtual void AuthenticationUserAction(object? sender, ProcessEventArgs e)
        {
            UserAction?.Invoke(sender, e);
        }

        protected static string ShellQuote(string value)
        {
            return "'" + value.Replace("'", "'\\''") + "'";
        }

        #endregion
    }
}
