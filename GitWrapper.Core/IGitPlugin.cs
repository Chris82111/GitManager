namespace Chris82111.GitManager.GitWrapper.Core
{
    /// <summary>
    /// An interface that every plugin class must implement in order to register itself.
    /// </summary>
    public interface IGitPlugin
    {
        /// <summary>
        ///         The method must be implemented for a specific operating system platform and architecture.
        /// <br/>   The public static method <see cref="GitWrapperFactory.Register(string, Func{IGitWrapper})"/> must be
        /// <br/>   called.
        /// <br/>   
        /// <br/>   The plugin implementation of the Register function must provide the following parameters:
        /// <br/>   
        /// <br/>   identifier: <inheritdoc cref="GitWrapperFactory.Register(string, Func{IGitWrapper})" path="//param[@name='identifier']/node()"/>
        /// <br/>   
        /// <br/>   creator: <inheritdoc cref="GitWrapperFactory.Register(string, Func{IGitWrapper})" path="//param[@name='creator']/node()"/>
        /// <code>
        /// GitFactory.Register(
        ///     $"{OSPlatform.Windows}-{Architecture.X64}",
        ///     static () => new StaticWindowsX64());
        /// </code>
        /// <code>
        /// GitFactory.Register(
        ///     GitWrapperWindowsX64.ClassIdentifier,,
        ///     static () => new StaticWindowsX64());
        /// </code>
        /// </summary>
        void Register();
    }
}
