using Chris82111.GitManager.GitWrapper.Core.Processes;

namespace Chris82111.GitManager.GitWrapper.Core.Authenticators
{
    /// <summary>
    ///         This interface handles authentication for the GitWrapper.
    /// <br/>   It is used as dependency injection (DI) so that it can be
    /// <br/>   reused across multiple <see cref="IGitWrapper"/> instances.
    /// </summary>
    public interface IGitAuthentication
    {
        /// <summary>
        /// The name can be chosen freely; it is not used internally during authentication.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///         Returns the number of objects using this authentication method.
        /// <br/>   See the description for more details <see cref="Dispose()"/>
        /// </summary>
        int DependentCount { get; }

        /// <summary>
        ///         Indicates whether the object has been disposed of:
        /// <br/>   true:  disposed
        /// <br/>   false: usable
        /// </summary>
        public bool IsDisposed { get; }

        /// <summary>
        ///         Here, the user can register their own handler. As an alternative to deriving from the
        /// <br/>   <see cref="GitAuthenticationBase"/> class, this provides a quicker way to implement custom authentication.
        /// <br/>  
        /// <br/><code>
        /// void UserActionHandler(object? sender, ProcessEventArgs e)
        /// {
        ///     var gitWrapper = (IGitWrapper)sender!;
        ///     var envVar = new ProcessEnvironmentVariable(e.Process.StartInfo);
        /// }
        /// </code>
        /// </summary>
        Action<object?, ProcessEventArgs>? UserAction { get; set; }

        /// <summary>
        ///         Performs all object cleanup
        /// <br/>   Before calling this, the <see cref="DependentCount"/> counter should be checked.
        /// <br/>   Calling Dispose makes the object unusable and removes all handler registrations.
        /// <br/>   However, the authentication object is not removed from <see cref="IGitWrapper"/>.
        /// </summary>
        void Dispose();

        /// <summary>
        ///         This function establishes the connection for authentication.
        /// <br/>   The authentication process subscribes to events from <see cref="IGitWrapper"/>.
        /// </summary>
        /// <param name="gitWrapper">The object to which a connection is to be established</param>
        void Connect(IGitWrapper gitWrapper);

        /// <summary>
        ///         This function disconnects the authentication connection.
        /// <br/>   The authentication service unsubscribes from the events of <see cref="IGitWrapper"/>.
        /// </summary>
        /// <param name="gitWrapper">The object from which the connection is to be disconnected</param>
        void Disconnect(IGitWrapper gitWrapper);

        /// <summary>
        /// Returns whether the token information has been set correctly
        /// </summary>
        /// <returns>
        ///         true:  The information has been set correctly
        /// <br/>   false: The information has not been set correctly</returns>
        bool IsTokenValid();

        /// <summary>
        /// Sets the login details required to use the token
        /// </summary>
        /// <param name="username">The username can be null; in that case, "oauth2" is used, which works on GitHub</param>
        /// <param name="token">The token (known as the "personal access token" on GitHub)</param>
        void SetToken(string? username, string token);

        /// <summary>
        /// Deletes the token information
        /// </summary>
        void ClearToken();

        /// <summary>
        /// Returns whether the SSH information has been set correctly
        /// </summary>
        /// <returns>
        ///         true:  The information has been set correctly
        /// <br/>   false: The information has not been set correctly</returns>
        bool IsSshValid();

        /// <summary>
        ///         Sets the login details required to use SSH.
        /// </summary>
        /// <remarks>
        ///         SSH must be set up for this:
        /// <br/>   1. Generate a key pair using 
        /// <br/>      <c>ssh-keygen -t ed25519 -C "Optional comment like a name or your email@example.com"</c>
        /// <br/>   2. Store/upload the public key on the server.
        /// <br/>   3. Never let the private key leave the computer.
        /// <br/>   4. Specify the private key using <paramref name="filePath"/>.
        /// <br/>   5. Use an SSH URL, for example:
        /// <br/>      <c>git@github.com:Chris82111/Repo.git</c>
        /// <br/>      <c>ssh://git@github.com/Chris82111/Repo.git</c>
        /// </remarks>
        /// <param name="filePath">The path to the SSH file</param>
        /// <param name="password">The passphrase for the SSH file. If the file is not password-protected, <c>null</c> may be used.</param>
        void SetSsh(string filePath, string? password = null);

        /// <summary>
        /// Deletes the SSH information
        /// </summary>
        void ClearSsh();

#warning Change object? to IGitWrapper none nullable and change desciption.
        /// <summary>
        /// This handler is called when the <see cref="IGitWrapper.Credential"/> property in the <see cref="IGitWrapper"/> class is assigned a new value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CredentialChangedHandler(object? sender, CredentialChangedEventArgs e);
    }
}
