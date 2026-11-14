namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    public abstract class KnownHostEntryBase : IKnownHostEntry
    {
        public override string ToString() => Serialize();

        public abstract string Serialize();
    }
}
