using Chris82111.GitManager.GitWrapper.Core.Helpers;
using System.Reflection;

namespace Chris82111.GitManager.GitWrapper.Core
{
    /// <summary>
    /// Factory Pattern, which provides the correct GitWrapper class depending on the system.
    /// </summary>
    public static class GitWrapperFactory
    {
        /// <summary>
        /// These types can be used to influence the behavior of the <see cref="Create(GitType)"/> function.
        /// </summary>
        public enum GitType
        {
            /// <summary>
            /// Try to use the static Git version. If that fails, the version installed on the system will be used.
            /// </summary>
            Auto = 0,

            /// <summary>
            /// The version installed on the system will be used.
            /// </summary>
            Local,

            /// <summary>
            /// Uses the static Git version. If this fails, an exception is thrown.
            /// </summary>
            Static,
        }

        private static readonly Dictionary<string, Func<IGitWrapper>> _creators = [];

        static GitWrapperFactory()
        {
            LoadPlugins();
        }

        /// <summary>
        ///         This method allows plugins to register themselves.
        /// <br/>   The respective <see cref="IGitPlugin.Register"/> functions call this static function.
        /// </summary>
        /// <param name="identifier"><inheritdoc cref="IGitWrapper.Identifier" path="/summary/node()"/></param>
        /// <param name="creator">This function parameter creates a new instance of the GitWrapper class.</param>
        public static void Register(string identifier, Func<IGitWrapper> creator)
        {
            _creators.TryAdd(identifier, creator);
        }

        /// <summary>
        /// Searches all DLLs in the folder and loads the system-specific plugins
        /// </summary>
        public static void LoadPlugins()
        {
            Assembly assembly;

            var dlls = Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll");

            foreach (string file in dlls)
            {
                try
                {
                    assembly = Assembly.LoadFrom(file);
                }
                catch
                {
                    continue;
                }

                foreach (Type type in assembly.GetTypes())
                {
                    if (!typeof(IGitPlugin).IsAssignableFrom(type)
                        || type.IsAbstract
                        || type.IsInterface)
                    {
                        continue;
                    }

                    if (Activator.CreateInstance(type) is IGitPlugin plugin)
                    {
                        plugin.Register();
                    }
                }
            }
        }

        /// <summary>
        ///         Returns a list of all registered system-specific plugins
        /// <br/>   
        /// <br/>   Each string identifier is:
        /// <br/>   <inheritdoc cref="IGitWrapper.Identifier" path="/summary/node()"/>
        /// </summary>
        /// <returns>List of identifier string</returns>
        public static List<string> Registerd()
        {
            return _creators.Keys.ToList();
        }

        /// <summary>
        ///         Creates a new class based on the specified platform.
        /// <br/>   
        /// <br/>   If the platform does not exist, a <see cref="KeyNotFoundException"/> is thrown.
        /// </summary>
        /// <param name="identifier"><inheritdoc cref="IGitWrapper.Identifier" path="/summary/node()"/></param>
        /// <returns>Returns a class that implements the specified interface</returns>
        public static IGitWrapper Create(string identifier)
        {
            return _creators[identifier]();
        }

        /// <summary>
        ///         Tries to create a new class based on the specified platform.
        /// </summary>
        /// <param name="identifier"><inheritdoc cref="IGitWrapper.Identifier" path="/summary/node()"/></param>
        /// <returns>
        ///         Returns a class that implements the specified interface;
        /// <br/>   if an error occurs, null is returned.
        /// </returns>
        public static IGitWrapper? TryCreate(string identifier)
        {
            return _creators.TryGetValue(identifier, out Func<IGitWrapper>? creator) ? creator() : null;
        }

        /// <summary>
        ///         Creates a new class based on the specified platform.
        /// </summary>
        /// <param name="gitType">Defines the behavior of the function; <see cref="GitType"/></param>
        /// <returns>Returns an iterface to use all wrapper classes</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IGitWrapper Create(GitType gitType = GitType.Auto)
        {
            if (gitType == GitType.Local)
            {
                return _creators["default"]();
            }

            var identifier = RuntimeHelper.Identifier;

            if (_creators.TryGetValue(identifier, out Func<IGitWrapper>? creator))
            {
                return creator();
            }

            if (gitType == GitType.Static)
            {
                throw new InvalidOperationException(
                    $"No Git implementation registered for '{identifier}', you are using '{RuntimeHelper.Identifier}'.{Environment.NewLine}" +
                    $"For example, use the following NuGet packages:{Environment.NewLine}" +
                    $"  'Chris82111.GitManager.GitWrapper.StaticGitWindowsX64'{Environment.NewLine}" +
                    $"  'Chris82111.GitManager.GitWrapper.StaticGitLinuxX64'{Environment.NewLine}");
            }

            return _creators["default"]();
        }

    }
}
