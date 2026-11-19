namespace Chris82111.GitManager.GitWrapper.Core.KnownHosts.Entries
{
    /// <summary>
    ///         Provides a base implementation of <see cref="IKnownHostEntry"/> with an abstract
    /// <br/>   serialization method for all host entries
    /// </summary>
    public abstract class KnownHostEntryBase : IKnownHostEntry
    {
        /// <inheritdoc cref="Serialize"/>
        public override string ToString() => Serialize();

        /// <inheritdoc />
        public abstract string Serialize();
    }
}
