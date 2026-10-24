using Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables;
using Chris82111.GitManager.GitWrapper.Core.Processes;
using System.Diagnostics.CodeAnalysis;

namespace Chris82111.GitManager.GitWrapper.Core.Authenticators
{
    /// <summary>
    /// Basic implementation for authenticating the <see cref="IGitWrapper"/> class using various authentication methods
    /// </summary>
    public class GitAuthenticationBase : IGitAuthentication, IDisposable
    {
        /// <inheritdoc />
        public string Name { get; set; } = string.Empty;

        /// <inheritdoc />
        public int DependentCount => _gitWrappers.Count;

        /// <inheritdoc />
        public bool IsDisposed => _disposed;

        /// <inheritdoc />
        public Action<object?, ProcessEventArgs>? UserAction { get; set; } = null;

        /// <summary>
        /// Contains references to the objects that use this authenticator
        /// </summary>
        protected readonly List<IGitWrapper> _gitWrappers = new List<IGitWrapper>();

        /// <summary>
        ///         Path to the null device, which discards all written data.
        /// <br/>   Always specifies the Linux null device, as "Git for Windows"
        /// <br/>   also uses MinGW internally, or rather a Linux environment.
        /// </summary>
        protected const string _nullDevice = "/dev/null";

        private bool _disposed;


        #region Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///         Underlying disposal function.
        /// <br/>
        /// <br/>   <inheritdoc cref="Dispose()"/>
        /// </summary>
        /// <param name="disposing">Indicates whether the call originates from Dispose (true) or a finalizer (false).</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                ClearToken();
                ClearSsh();

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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Disconnect(IGitWrapper gitWrapper)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            gitWrapper.CredentialChanged -= CredentialChangedHandler;
            CredentialChangedHandler(gitWrapper, new CredentialChangedEventArgs(gitWrapper.Credential, Credentials.Default));
            _gitWrappers.Remove(gitWrapper);
        }


        #region Token

        /// <summary>
        /// The username can be null; in that case, "oauth2" is used, which works on GitHub
        /// </summary>
        protected string? _username = null;

#warning Not currently encrypted
        /// <summary>
        ///         The token (known as the "personal access token" on GitHub)
        /// <br/>   
        /// <br/>   Specifying a token (<see cref="_token"/>) takes precedence over an
        /// <br/>   environment variable (<see cref="_environmentVariableName"/>)
        /// </summary>
        protected string? _token = null;

        /// <summary>
        /// The name of the environment variable containing a token
        /// <br/>   
        /// <br/>   Specifying a token (<see cref="_token"/>) takes precedence over an
        /// <br/>   environment variable (<see cref="_environmentVariableName"/>)
        /// </summary>
        protected string? _environmentVariableName = null;

        /// <inheritdoc />
        public bool IsTokenValid()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var invalid = string.IsNullOrWhiteSpace(_username) ||
                ( string.IsNullOrWhiteSpace(_token) &&
                  string.IsNullOrWhiteSpace(_environmentVariableName));

            return !invalid;
        }

        /// <inheritdoc />
        public void SetToken(string? username, string token)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            ArgumentNullException.ThrowIfNull(token);

            _username = username ?? "oauth2";
            _token = token;
        }

        /// <inheritdoc />
        public void SetTokenFromEnvironment(string? username, string environmentVariableName)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            ArgumentNullException.ThrowIfNull(environmentVariableName);

            _username = username ?? "oauth2";
            _environmentVariableName = environmentVariableName;
        }

        /// <inheritdoc />
        public void ClearToken()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            _username = null;
            _token = null;
            _environmentVariableName = null;
        }

        #endregion


        #region Ssh

        /// <summary>
        /// The path to the SSH file
        /// </summary>
        protected string? _filePath = null;

#warning Not currently encrypted
        /// <summary>
        /// The passphrase for the SSH file. If the file is not password-protected, <c>null</c> may be used
        /// </summary>
        protected string? _password = null;

        /// <inheritdoc />
        public bool IsSshValid()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            return false == string.IsNullOrWhiteSpace(_filePath);
        }

        /// <inheritdoc />
        public void SetSsh(string filePath, string? password = null)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            ArgumentNullException.ThrowIfNull(filePath);

            _filePath = filePath;
            _password = password;
        }

        /// <inheritdoc />
        public void ClearSsh()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            _filePath = null;
            _password = null;
        }

        #endregion

        /// <inheritdoc />
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

                case Credentials.SshNoPassphrase:
                    gitWrapper.BeforeProcessStart -= AuthenticationSshNoPassphrase;
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

                case Credentials.SshNoPassphrase:
                    gitWrapper.BeforeProcessStart += AuthenticationSshNoPassphrase;
                    break;

                default:
                    break;
            }
        }


        #region Logic

        /// <summary>
        ///         This is an authentication handler that is called via the <see cref="IGitWrapper.BeforeProcessStart"/>
        /// <br/>   event before the process starts. It modifies and sets environment variables to influence Git’s
        /// <br/>   behaviour accordingly.
        /// <br/>   
        /// <br/>   This authentication handler implements the behaviour of <see cref="Credentials.Nothing"/>:
        /// <br/>   <inheritdoc cref="Credentials.Nothing"/>
        /// </summary>
        /// <param name="sender">sender is of type <see cref="IGitWrapper"/> and is never null</param>
        /// <param name="e">Contains information about the process</param>
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

        /// <summary>
        ///         This is an authentication handler that is called via the <see cref="IGitWrapper.BeforeProcessStart"/>
        /// <br/>   event before the process starts. It modifies and sets environment variables to influence Git’s
        /// <br/>   behaviour accordingly.
        /// <br/>   
        /// <br/>   This authentication handler implements the behaviour of <see cref="Credentials.Token"/>:
        /// <br/>   <inheritdoc cref="Credentials.Token"/>
        /// </summary>
        /// <param name="sender">sender is of type <see cref="IGitWrapper"/> and is never null</param>
        /// <param name="e">Contains information about the process</param>
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

            string helper;
            if (null != _token)
            {
                helper =
                    $"!f() {{ printf 'username=%s\\npassword=%s\\n\\n' " +
                    $"{ShellQuote(_username ?? string.Empty)} {ShellQuote(_token ?? string.Empty)}; }}; f";
            }
            else
            {
                helper =
                    $"!f() {{ printf 'username=%s\\npassword=%s\\n\\n' " +
                    $"{ShellQuote(_username) ?? string.Empty} \"${{{_environmentVariableName}}}\"; }}; f";
            }
            envVar.Set("GIT_CONFIG_VALUE_1", helper);
        }

        /// <summary>
        ///         This is an authentication handler that is called via the <see cref="IGitWrapper.BeforeProcessStart"/>
        /// <br/>   event before the process starts. It modifies and sets environment variables to influence Git’s
        /// <br/>   behaviour accordingly.
        /// <br/>   
        /// <br/>   This authentication handler implements the behaviour of <see cref="Credentials.UserAction"/>:
        /// <br/>   <inheritdoc cref="Credentials.UserAction"/>
        /// </summary>
        /// <param name="sender">sender is of type <see cref="IGitWrapper"/> and is never null</param>
        /// <param name="e">Contains information about the process</param>
        protected virtual void AuthenticationUserAction(object? sender, ProcessEventArgs e)
        {
            UserAction?.Invoke(sender, e);
        }

        /// <summary>
        ///         This is an authentication handler that is called via the <see cref="IGitWrapper.BeforeProcessStart"/>
        /// <br/>   event before the process starts. It modifies and sets environment variables to influence Git’s
        /// <br/>   behaviour accordingly.
        /// <br/>   
        /// <br/>   This authentication handler implements the behaviour of <see cref="Credentials.SshNoPassphrase"/>:
        /// <br/>   <inheritdoc cref="Credentials.SshNoPassphrase"/>
        /// </summary>
        /// <param name="sender">sender is of type <see cref="IGitWrapper"/> and is never null</param>
        /// <param name="e">Contains information about the process</param>
        protected virtual void AuthenticationSshNoPassphrase(object? sender, ProcessEventArgs e)
        {
            var envVar = new ProcessEnvironmentVariable(e.Process.StartInfo);

            if (false == IsSshValid())
            {
                throw new NullReferenceException($"Error with internal stored ssh file call the methode '{nameof(SetSsh)}' first.");
            }

            var psi = e.Process.StartInfo;

            // The debug options "-vvv" can be added after the SSH command

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

        /// <summary>
        ///         This is an authentication handler that is called via the <see cref="IGitWrapper.BeforeProcessStart"/>
        /// <br/>   event before the process starts. It modifies and sets environment variables to influence Git’s
        /// <br/>   behaviour accordingly.
        /// <br/>   
        /// <br/>   This authentication handler implements the behaviour of <see cref="Credentials.Ssh"/>:
        /// <br/>   <inheritdoc cref="Credentials.Ssh"/>
        /// </summary>
        /// <param name="sender">sender is of type <see cref="IGitWrapper"/> and is never null</param>
        /// <param name="e">Contains information about the process</param>
        protected virtual void AuthenticationSsh(object? sender, ProcessEventArgs e)
        {
            var gitWrapper = (IGitWrapper)sender!;

            if (false == IsSshValid())
            {
                throw new NullReferenceException($"Error with internal stored ssh file call the methode '{nameof(SetSsh)}' first.");
            }

            var envVar = new ProcessEnvironmentVariable(e.Process.StartInfo);

            // The debug options "-vvv" can be added after the SSH command

            // --- Configure GIT_SSH_COMMAND for non-interactive Git operations ---
            envVar.Set("GIT_SSH_COMMAND",
                $"ssh -i \"{_filePath}\" " + // Use this specific private key file for authentication
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

            envVar.Set("SSH_ASKPASS", gitWrapper.SshPassphrase ?? _nullDevice);
            envVar.Set("SSH_ASKPASS_REQUIRE", "force");

            if (null != _password)
            {
                envVar.Set("GIT_MANAGER_SSH_PASSPHRASE", _password);
            }

            // --- Config file overrides ---
            envVar.Set("GIT_CONFIG_NOSYSTEM", "1");      // Ignore system-wide Git configuration
            envVar.Set("GIT_CONFIG_GLOBAL", _nullDevice); // Ignore user global config (point to null device)
            envVar.Set("GIT_CONFIG_SYSTEM", _nullDevice); // Ignore system-wide config file (point to null device)
        }

        /// <summary>
        /// Quotes a value for use as a shell argument
        /// </summary>
        /// <param name="value">The value to quote</param>
        [return: NotNullIfNotNull(nameof(value))]
        protected static string? ShellQuote(string? value)
        {
            if (null == value) { return null; }

            return "'" + value.Replace("'", "'\\''") + "'";
        }

        #endregion
    }
}
