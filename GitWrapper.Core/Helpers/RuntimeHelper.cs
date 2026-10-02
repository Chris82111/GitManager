using System.Runtime.InteropServices;

namespace Chris82111.GitManager.GitWrapper.Core.Helpers
{
    /// <summary>
    /// Helps provide platform-specific data
    /// </summary>
    public class RuntimeHelper
    {
        /// <summary>
        ///         Creates the current platform identifier
        /// <br/>   
        /// <br/>   The string identifier is:
        /// <br/>   <inheritdoc cref="IGitWrapper.Identifier" path="/summary/node()"/>
        /// </summary>
        /// <returns>The current platform identifier</returns>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public static string Identifier { get; } = GetIdentifier();

        private static string GetIdentifier()
        {
            OSPlatform osPlatform;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                osPlatform = OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                osPlatform = OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                osPlatform = OSPlatform.OSX;
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            return $"{osPlatform}-{RuntimeInformation.OSArchitecture}";
        }
    }
}
