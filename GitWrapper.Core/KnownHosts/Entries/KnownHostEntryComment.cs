namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    public sealed class KnownHostEntryComment : KnownHostEntryBase
    {
        // Comment including '#'
        public required string Comment { get; init; }

        public override string Serialize() => Comment;
    }
}
