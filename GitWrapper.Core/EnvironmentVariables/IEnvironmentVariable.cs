namespace Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables
{
    /// <summary>    
    /// This interface class deals with the use of environment variables.
    /// </summary>
    public interface IEnvironmentVariable
    {
        /// <summary>
        ///         Determines whether a variable exist
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <returns>
        ///         true if the variable exists;
        /// <br/>   false if the variable does not exist</returns>
        bool IsExisting(string variable);

        /// <summary>
        ///         Sets an environment variable
        /// <br/>
        /// <br/>   An empty string can be set
        /// <br/>   If set to null, the variable is removed; <see cref="Remove(string)"/>
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <param name="value">The variable value.</param>
        void Set(string variable, string? value);

        /// <summary>
        ///         Reads an environment variable
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <returns>
        ///         Returns the value of the variable;
        /// <br/>   null if the variable does not exist</returns>
        string? Get(string variable);

        /// <summary>
        /// Removes a variable
        /// </summary>
        /// <param name="variable">The variable name</param>
        void Remove(string variable);



        /// <summary>
        ///         Gets all path values of a variable
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <returns>
        ///         All path values that are not null or empty;
        /// <br/>   null if the variable does not exist</returns>
        string[]? GetArray(string variable);

        /// <summary>
        /// Gets the number of path in the variable
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <returns>
        /// The number of values
        /// </returns>
        int Count(string variable);

        /// <summary>
        /// Adds a path to the variable
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <param name="value">The value to add</param>
        void Add(string variable, string value);



        /// <summary>
        /// Removes a path from the variable
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <param name="value">The value to remove</param>
        /// <param name="remove">The maximum number of values to remove</param>
        /// <returns>
        /// The number of values removed
        /// </returns>
        int Remove(string variable, string value, int remove = int.MaxValue);

        /// <summary>
        /// Gets the number of path in the variable
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <param name="value">The path to count</param>
        /// <returns>
        /// The number of occurrences
        /// </returns>
        int Count(string variable, string value);

        /// <summary>
        ///         Determines whether the variable contains a path
        /// </summary>
        /// <param name="variable">The variable name</param>
        /// <param name="value">The path to find</param>
        /// <returns>
        ///         true if the path exists;
        /// <br/>   false if the path does not exist</returns>
        bool Contains(string variable, string value);
    }
}
