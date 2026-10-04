using Chris82111.GitManager.GitWrapper.Core.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables
{
    public abstract class EnvironmentVariableBase : IEnvironmentVariable
    {
        protected abstract string? GetValue(string variable);

        protected abstract void SetValue(string variable, string value);

        protected abstract void RemoveValue(string variable);

        public string? Get(string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Variable '{nameof(variable)}' must not be null or empty.");
            }

            return GetValue(variable);
        }

        public string[] GetArray(string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Variable '{nameof(variable)}' must not be null or empty.");
            }

            var value1 = GetValue(variable);
            if (null == value1)
            {
                throw new Exception($"The variable '{nameof(variable)}' is not set, nothing can be added. Use the method '{nameof(Set)}'.");
            }

            return GetArrayPathValues(value1);
        }

        public void Set(string variable, string value)
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

        public void Remove(string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Variable '{nameof(variable)}' must not be null or empty.");
            }

            RemoveValue(variable);
        }

        public int Count(string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentNullException($"Variable '{nameof(variable)}' must not be null or empty.");
            }

            var value1 = GetValue(variable);
            if (null == value1)
            {
                throw new Exception($"The variable '{nameof(variable)}' is not set, nothing can be added. Use the method '{nameof(Set)}'.");
            }

            return CountPathValues(value1);
        }

        public bool IsExisting(string variable)
        {
            return null != Get(variable);
        }

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
            if (null == value1)
            {
                throw new Exception($"The variable '{nameof(variable)}' is not set, nothing can be added. Use the method '{nameof(Set)}'.");
            }

            SetValue(variable, CombinePathValues(value1, value));
        }

        public void AddOrSet(string variable, string value)
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
            SetValue(variable, CombinePathValues(value1, value));
        }

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
                throw new Exception($"The variable '{nameof(variable)}' is not set, nothing can be removed.");
            }

            int removed = TryRemovePathValues(value1, value, out string result, remove);

            SetValue(variable, result);

            return removed;
        }

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
                throw new Exception($"The variable '{nameof(variable)}' is not set, nothing can be removed.");
            }

            return CountPathValues(value1, value);
        }

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
                throw new Exception($"The variable '{nameof(variable)}' is not set, nothing can be removed.");
            }

            return ContainsPathValues(value1, value);
        }


        #region Working only with strings

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

        public static int CountPathValues(string value)
        {
            return GetArrayPathValues(value).Length;
        }

        [return: NotNullIfNotNull(nameof(value1))]
        [return: NotNullIfNotNull(nameof(value2))]
        public static string? CombinePathValues(string? value1, string? value2)
        {
            if (string.IsNullOrEmpty(value1))
            {
                if (string.IsNullOrEmpty(value2))
                {
                    return null;
                }

                return value2;
            }

            if (string.IsNullOrEmpty(value2))
            {
                return value1;
            }

            return value2 + Path.PathSeparator + value1;
        }

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

            List<string> pathsThatRemain = new List<string>(paths.Length);
            List<string> pathsRemoved = new List<string>();
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
