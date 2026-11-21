using Chris82111.GitManager.GitWrapper.Core.Utilities;

namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts
{
    /// <summary>
    ///         Provides a thread-safe multiton for <see cref="KnownHostsFile"/> instances.
    /// <br/>   A separate shared instance is maintained for each unique file path.
    /// </summary>
    public class KnownHostMultiton : Multiton<KnownHostsFile>
    {
        /// <summary>
        ///         Gets the shared <see cref="KnownHostsFile"/> instance for the specified file path,
        /// <br/>   creating it asynchronously if it does not already exist.
        /// </summary>
        /// <param name="filePath">The path to the known_hosts file.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result is the shared
        /// <see cref="KnownHostsFile"/> instance associated with the specified file path.
        /// </returns>
        public async static Task<KnownHostsFile> InstanceAsync(string filePath)
        {
            return await Multiton<KnownHostsFile>.InstanceAsync(KnownHostsFile.CreateAsync, filePath);
        }

        /// <summary>
        ///         Gets the shared <see cref="KnownHostsFile"/> instance for the specified file path,
        /// <br/>   creating it synchronously if it does not already exist.
        /// </summary>
        /// <param name="filePath">The path to the known_hosts file.</param>
        /// <returns>
        /// The shared <see cref="KnownHostsFile"/> instance associated with the specified file path.
        /// </returns>
        public static KnownHostsFile Instance(string filePath)
        {
            return Multiton<KnownHostsFile>.Instance(KnownHostsFile.Create, filePath);
        }
    }
}
