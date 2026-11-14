namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    public sealed class KnownHostEntryUnknown : KnownHostEntryBase
    {
        public required string Content { get; init; }

        public override string Serialize() => Content;
    }
}
