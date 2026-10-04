namespace Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables
{
    public interface IEnvironmentVariable
    {
        string? Get(string variable);
        string[] GetArray(string variable);
        void Set(string variable, string value);
        void Remove(string variable);
        int Count(string variable);
        bool IsExisting(string variable);
        void Add(string variable, string value);
        void AddOrSet(string variable, string value);
        int Remove(string variable, string value, int remove = int.MaxValue);
        int Count(string variable, string value);
        bool Contains(string variable, string value);
    }
}
