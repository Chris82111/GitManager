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

#warning _nullDevice should always be /dev/null because Git for Windows uses MinGW
        /// <summary>
        /// Gets the platform-specific path to the null device, which discards all written data.
        /// </summary>
        protected readonly string _nullDevice = // RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            // ? "NUL" : 
            "/dev/null";

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


        #region Ssh

        protected string? _filePath = null;

#warning Not currently encrypted
        protected string? _password = null;

        public bool IsSshValid()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

#warning A distinction must be made between with and without a password, the password can be null

            return
                false == string.IsNullOrWhiteSpace(_filePath) ||
                false == string.IsNullOrEmpty(_password);
        }

        public void SetSsh(string filePath, string password)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            _filePath = filePath;
            _password = password;
        }

        public void ClearSsh()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            _filePath = null;
            _password = null;
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

                case Credentials.Ssh:
                    gitWrapper.BeforeProcessStart -= AuthenticationSsh;
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

                case Credentials.Ssh:
                    gitWrapper.BeforeProcessStart += AuthenticationSsh;
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

#warning To use an existing environmen variable could be usefull, but the else does not work
#if true
            string helper =
                $"!f() {{ printf 'username=%s\\npassword=%s\\n\\n' " +
                $"{ShellQuote(_username ?? string.Empty)} {ShellQuote(_token ?? string.Empty)}; }}; f";
#else
#error does not work
            envVar.Set("$GIT_MANAGER_TOKEN", $"{ShellQuote(_token ?? string.Empty)}");

            string helper =
                $"!f() {{ printf 'username=%s\\npassword=%s\\n\\n' " +
                $"{ShellQuote(_username ?? string.Empty)} \"$GIT_MANAGER_TOKEN\"; }}; f";
#endif      

            envVar.Set("GIT_CONFIG_VALUE_1", helper);
        }

        protected virtual void AuthenticationUserAction(object? sender, ProcessEventArgs e)
        {
            UserAction?.Invoke(sender, e);
        }

        // GitHub needs the public key, created with:
        // ssh-keygen -t ed25519 -C "Optional comment like a name or your email@example.com"
        

        // Use the right URL, if using ssh
        // git@github.com:Chris82111/Repo.git
        // ssh://git@github.com/Chris82111/Repo.git
        protected virtual void AuthenticationSshUnencrypted(object? sender, ProcessEventArgs e)
        {
            var envVar = new ProcessEnvironmentVariable(e.Process.StartInfo);

            if (false == IsSshValid())
            {
                throw new NullReferenceException($"Error with internal stored ssh file call the methode '{nameof(SetSsh)}' first.");
            }

            var psi = e.Process.StartInfo;

            // --- Configure GIT_SSH_COMMAND for non-interactive Git operations ---
            envVar.Set("GIT_SSH_COMMAND", 
                $"ssh -i \"{_filePath}\" " + // Use this specific private key file for authentication
                $"-F \"{_nullDevice}\" " +   // SSH will not read ~/.ssh/config (or the system SSH config)
                "-o BatchMode=yes " +        // yes : Disable interactive prompts (fail instead of asking for password)
                "-o IdentitiesOnly=yes " +   // Use only the identity files specified with -i, ignore SSH agent keys
                "-o StrictHostKeyChecking=yes " +          // no : Automatically accept host keys (useful in scripts/CI, but bypasses security check)
                "-o PreferredAuthentications=publickey " + // Only attempt public key authentication, skip password or other methods
                "-o IdentityAgent=none " +                 // This disables the SSH authentication agent for this connection.
                "-o PasswordAuthentication=no " +          // This prevents SSH from using the traditional SSH password authentication method.
                "-o KbdInteractiveAuthentication=no");     // This disables keyboard-interactive authentication.

            // --- Credential management / Git Credential Manager ---
            envVar.Set("GCM_INTERACTIVE", "Never");                // Never prompt interactively for credentials (Git Credential Manager)
            envVar.Set("GCM_CREDENTIAL_STORE", "none");            // Disable storing credentials in any secure store
            envVar.Set("GIT_CREDENTIAL_MANAGER_ENABLED", "false"); // Disable Git Credential Manager entirely

            // --- Non-interactive mode / disable prompts ---
            envVar.Set("GIT_TERMINAL_PROMPT", "0");    // Disable Git terminal prompts for credentials
            envVar.Set("GIT_ASKPASS", "echo");         // Disable Git GUI credential prompts (fallback to echo)
            envVar.Set("SSH_ASKPASS", "echo");         // Disable SSH GUI prompts (fallback to echo)
            envVar.Set("SSH_AUTH_SOCK", string.Empty); // Disable SSH agent (ignore any running agent)
            
            envVar.Set("SSH_ASKPASS_REQUIRE", "never");

            // --- Config file overrides ---
            envVar.Set("GIT_CONFIG_NOSYSTEM", "1");      // Ignore system-wide Git configuration
            envVar.Set("GIT_CONFIG_GLOBAL", _nullDevice); // Ignore user global config (point to null device)
            envVar.Set("GIT_CONFIG_SYSTEM", _nullDevice); // Ignore system-wide config file (point to null device)
        }

        protected virtual void AuthenticationSsh(object? sender, ProcessEventArgs e)
        {
            IGitWrapper gitWrapper = (IGitWrapper)sender!;

            if (null == gitWrapper.SshPassphrase)
            {
                if (gitWrapper is GitWrapperSystem)
                {
                    throw new NotSupportedException($"The '{nameof(GitWrapperSystem)}' class does not support passphrases. Only the system-specific plugins support this feature.");
                }
                throw new NullReferenceException($"A program to echo the passphrase is required; the plugin should have set the variable '{nameof(gitWrapper.SshPassphrase)}'.");
            }

            if (false == File.Exists(gitWrapper.SshPassphrase))
            {
                throw new Exception($"File does not exists, '{gitWrapper.SshPassphrase}'");
            }

            var envVar = new ProcessEnvironmentVariable(e.Process.StartInfo);

            if (false == IsSshValid())
            {
                throw new NullReferenceException($"Error with internal stored ssh file call the methode '{nameof(SetSsh)}' first.");
            }

            // --- Configure GIT_SSH_COMMAND for non-interactive Git operations ---
            envVar.Set("GIT_SSH_COMMAND",
                $"ssh -vvv -i \"{_filePath}\" " + // Use this specific private key file for authentication
                $"-F \"{_nullDevice}\" " +        // SSH will not read ~/.ssh/config (or the system SSH config)
                "-o BatchMode=no " +              // yes : Disable interactive prompts (fail instead of asking for password)
                "-o IdentitiesOnly=yes " +        // Use only the identity files specified with -i, ignore SSH agent keys
                "-o StrictHostKeyChecking=yes " +          // no : Automatically accept host keys (useful in scripts/CI, but bypasses security check)
                "-o PreferredAuthentications=publickey " + // Only attempt public key authentication, skip password or other methods
                "-o IdentityAgent=none " +                 // This disables the SSH authentication agent for this connection.
                "-o PasswordAuthentication=no " +          // This prevents SSH from using the traditional SSH password authentication method.
                "-o KbdInteractiveAuthentication=no");     // This disables keyboard-interactive authentication.

            // --- Credential management / Git Credential Manager ---
            envVar.Set("GCM_INTERACTIVE", "Never");                // Never prompt interactively for credentials (Git Credential Manager)
            envVar.Set("GCM_CREDENTIAL_STORE", "none");            // Disable storing credentials in any secure store
            envVar.Set("GIT_CREDENTIAL_MANAGER_ENABLED", "false"); // Disable Git Credential Manager entirely

            // --- Non-interactive mode / disable prompts ---
            envVar.Set("GIT_TERMINAL_PROMPT", "0");    // Disable Git terminal prompts for credentials
            envVar.Set("GIT_ASKPASS", "echo");         // Disable Git GUI credential prompts (fallback to echo)
            
            envVar.Set("SSH_AUTH_SOCK", string.Empty); // Disable SSH agent (ignore any running agent)


            envVar.Set("SSH_ASKPASS", gitWrapper.SshPassphrase);
            envVar.Set("SSH_ASKPASS_REQUIRE", "force");

            if (null == _password)
            {
                throw new NullReferenceException($"A passphrase is required; set the '{nameof(_password)}' variable");
            }
            envVar.Set("GIT_MANAGER_SSH_PASSPHRASE", _password);

            // --- Config file overrides ---
            envVar.Set("GIT_CONFIG_NOSYSTEM", "1");      // Ignore system-wide Git configuration
            envVar.Set("GIT_CONFIG_GLOBAL", _nullDevice); // Ignore user global config (point to null device)
            envVar.Set("GIT_CONFIG_SYSTEM", _nullDevice); // Ignore system-wide config file (point to null device)
        }

        protected static string ShellQuote(string value)
        {
            return "'" + value.Replace("'", "'\\''") + "'";
        }

#endregion
    }
}
