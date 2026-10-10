using Chris82111.GitManager.GitWrapper.Core.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables
{
    /// <summary>    
    ///         This base class deals with the use of environment variables.
    /// <br/>   
    /// <br/>   The methods are designed so that an exception is thrown only
    /// <br/>   if the input parameters are incorrect.
    /// </summary>
    public abstract class EnvironmentVariableBase : IEnvironmentVariable
    {
        /// <summary>
        ///         Reads an environment variable
        /// <br/>
        /// <br/>   Must be provided in every derived class.
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <returns>
        ///         Returns the value of the variable;
        /// <br/>   null if the variable does not exist</returns>
        protected abstract string? GetValue(string variable);

        /// <summary>
        ///         Sets an environment variable
        /// <br/>
        /// <br/>   An empty string can be set
        /// <br/>   If set to null, the variable is removed; <see cref="Remove(string)"/>
        /// <br/>
        /// <br/>   Must be provided in every derived class.
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <param name="value">The variable value.</param>
        protected abstract void SetValue(string variable, string value);

        /// <summary>
        ///         Removes a variable
        /// <br/>
        /// <br/>   Must be provided in every derived class.
        /// </summary>
        /// <param name="variable">The variable name</param>
        protected abstract void RemoveValue(string variable);



        /// <inheritdoc/>
        public bool IsExisting(string variable)
        {
            return null != Get(variable);
        }

        /// <inheritdoc/>
        public void Set(string variable, string? value)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Variable '{nameof(variable)}' must not be null or empty.");
            }

            if (null == value)
            {
                RemoveValue(variable);
            }
            else
            {
                SetValue(variable, value);
            }
        }

        /// <inheritdoc/>
        public string? Get(string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Variable '{nameof(variable)}' must not be null or empty.");
            }

            return GetValue(variable);
        }

        /// <inheritdoc/>
        public void Remove(string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Variable '{nameof(variable)}' must not be null or empty.");
            }

            RemoveValue(variable);
        }



        /// <inheritdoc/>
        public string[]? GetArray(string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Variable '{nameof(variable)}' must not be null or empty.");
            }

            var value1 = GetValue(variable);
            if (null == value1)
            {
                return null;
            }

            return GetArrayPathValues(value1);
        }

        /// <inheritdoc/>
        public int Count(string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Variable '{nameof(variable)}' must not be null or empty.");
            }

            var value1 = GetValue(variable);
            if (null == value1)
            {
                return 0;
            }

            return CountPathValues(value1);
        }

        /// <inheritdoc/>
        public void Add(string variable, string value)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Variable '{nameof(variable)}' must not be null or empty.");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException($"Variable '{nameof(value)}' must not be null or empty.");
            }

            var value1 = GetValue(variable);
            SetValue(variable, CombinePathValues(value1, value, true));
        }



        /// <inheritdoc/>
        public int Remove(string variable, string value, int remove = int.MaxValue)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Variable '{nameof(variable)}' must not be null or empty.");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException($"Variable '{nameof(value)}' must not be null or empty.");
            }

            var value1 = Get(variable);
            if (null == value1)
            {
                return 0;
            }

            int removed = TryRemovePathValues(value1, value, out string result, remove);

            SetValue(variable, result);

            return removed;
        }

        /// <inheritdoc/>
        public int Count(string variable, string value)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Variable '{nameof(variable)}' must not be null or empty.");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException($"Variable '{nameof(value)}' must not be null or empty.");
            }

            var value1 = Get(variable);
            if (null == value1)
            {
                return 0;
            }

            return CountPathValues(value1, value);
        }

        /// <inheritdoc/>
        public bool Contains(string variable, string value)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Variable '{nameof(variable)}' must not be null or empty.");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException($"Variable '{nameof(value)}' must not be null or empty.");
            }

            var value1 = Get(variable);
            if (null == value1)
            {
                return false;
            }

            return ContainsPathValues(value1, value);
        }


        #region Working only with strings

        /// <summary>
        ///         Gets all path values from a string, connected with <see cref="Path.PathSeparator"/>
        /// </summary>
        /// <param name="value">Paths as one string</param>
        /// <returns>
        ///         All path values that are not null or empty</returns>
        public static string[] GetArrayPathValues(string value)
        {
            if (null == value)
            {
                throw new ArgumentNullException($"Variable '{nameof(value)}' must not be null.");
            }

            if (string.Empty == value)
            {
                return Array.Empty<string>();
            }

            return value.Split(
                Path.PathSeparator,
                StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Gets the number of path in the variable
        /// </summary>
        /// <param name="value">Paths as one string</param>
        /// <returns>
        /// The number of values
        /// </returns>
        public static int CountPathValues(string value)
        {
            return GetArrayPathValues(value).Length;
        }

        /// <summary>
        ///         Joins paths using the <see cref="Path.PathSeparator"/> character
        /// <br/>
        /// <br/>   Depending on whether the paths are null or empty, and whether the
        /// <br/>   option is set, the paths will be joined or removed.
        /// </summary>
        /// <param name="value1">The first path</param>
        /// <param name="value2">The second path</param>
        /// <param name="removeEmptyEntries">Removes empty paths if set</param>
        /// <returns>
        ///         Connected Paths;
        /// <br/>   null depending on the situation</returns>
        [return: NotNullIfNotNull(nameof(value1))]
        [return: NotNullIfNotNull(nameof(value2))]
        public static string? CombinePathValues(string? value1, string? value2, bool removeEmptyEntries = true)
        {
            if (null == value1 && null == value2)
            {
                return null;
            }
            if (null == value1)
            {
                return value2;
            }
            if (null == value2)
            {
                return value1;
            }

            if (removeEmptyEntries)
            {
                if (string.Empty == value1)
                {
                    return value2;
                }
                if (string.Empty == value2)
                {
                    return value1;
                }
            }

            return value2 + Path.PathSeparator + value1;
        }

        /// <summary>
        /// Removes a path <paramref name="value2"/> from <paramref name="value1"/>
        /// </summary>
        /// <param name="value1">The collection of paths</param>
        /// <param name="value2">The path to find</param>
        /// <param name="result">The collection excluding the paths to be removed</param>
        /// <param name="remove">The maximum number of values to remove</param>
        /// <returns>
        /// The number of values removed
        /// </returns>
        public static int TryRemovePathValues(string value1, string value2, out string result, int remove = int.MaxValue)
        {
            if (string.IsNullOrEmpty(value1))
            {
                throw new ArgumentNullException($"Variable {nameof(value1)} must not be null or empty.");
            }

            if (string.IsNullOrEmpty(value2))
            {
                throw new ArgumentNullException($"Variable {nameof(value2)} must not be null or empty.");
            }

            value2 = PathHelper.ReplacePathSeparatorsOnly(value2);

            value2 = Path
                .GetFullPath(value2)
                .TrimEnd(Path.DirectorySeparatorChar);

            var paths = value1.Split(
                Path.PathSeparator,
                StringSplitOptions.RemoveEmptyEntries);

            var pathsThatRemain = new List<string>(paths.Length);
            var pathsRemoved = new List<string>();
            int i;
            string path;
            for (i = 0; i < paths.Length && 0 < remove; i++)
            {
                path = paths[i];

                string pathCleaned = PathHelper.ReplacePathSeparatorsOnly(Path.GetFullPath(path));
                if (string.Equals(
                        pathCleaned,
                        value2,
                        StringComparison.OrdinalIgnoreCase))
                {
                    pathsRemoved.Add(pathCleaned);
                    remove--;
                }
                else
                {
                    pathsThatRemain.Add(path);
                }
            }

            for (; i < paths.Length; i++)
            {
                path = paths[i];
                pathsThatRemain.Add(path);
            }

            result = string.Join(Path.PathSeparator, pathsThatRemain);

            return pathsRemoved.Count;
        }

        /// <summary>
        ///         Determines whether the <paramref name="value1"/> contains the path <paramref name="value2"/>
        /// </summary>
        /// <param name="value1">The collection of paths</param>
        /// <param name="value2">The path to find</param>
        /// <returns>
        ///         true if the path exists;
        /// <br/>   false if the path does not exist</returns>
        public static bool ContainsPathValues(string value1, string value2)
        {
            if (string.IsNullOrEmpty(value1))
            {
                throw new ArgumentNullException($"Variable {nameof(value1)} must not be null or empty.");
            }

            if (string.IsNullOrEmpty(value2))
            {
                throw new ArgumentNullException($"Variable {nameof(value2)} must not be null or empty.");
            }

            value2 = PathHelper.ReplacePathSeparatorsOnly(value2);

            var paths = value1.Split(
                Path.PathSeparator,
                StringSplitOptions.RemoveEmptyEntries);

            value2 = Path
                .GetFullPath(value2)
                .TrimEnd(Path.DirectorySeparatorChar);

            bool hasElement = paths
                .Any(p => string.Equals(
                    PathHelper.ReplacePathSeparatorsOnly(Path.GetFullPath(p)),
                    value2,
                    StringComparison.OrdinalIgnoreCase));

            return hasElement;
        }

        /// <summary>
        /// Gets the number of path <paramref name="value2"/> in the variable <paramref name="value1"/>
        /// </summary>
        /// <param name="value1">The collection of Paths</param>
        /// <param name="value2">The path to count</param>
        /// <returns>
        /// The number of occurrences
        /// </returns>
        public static int CountPathValues(string value1, string value2)
        {
            if (string.IsNullOrEmpty(value1))
            {
                throw new ArgumentNullException($"Variable {nameof(value1)} must not be null or empty.");
            }

            if (string.IsNullOrEmpty(value2))
            {
                throw new ArgumentNullException($"Variable {nameof(value2)} must not be null or empty.");
            }

            value2 = PathHelper.ReplacePathSeparatorsOnly(value2);

            var paths = value1.Split(
                Path.PathSeparator,
                StringSplitOptions.RemoveEmptyEntries);

            value2 = Path
                .GetFullPath(value2)
                .TrimEnd(Path.DirectorySeparatorChar);

            return paths
                .Count(p => string.Equals(
                    PathHelper.ReplacePathSeparatorsOnly(Path.GetFullPath(p)),
                    value2,
                    StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}
