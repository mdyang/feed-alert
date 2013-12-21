namespace feed_alert.Persistence
{
    using Entity;
    using System.Collections.Generic;

    class XMLPersistence: IPersistence
    {
        private static readonly string feedSourceStore = "sources.xml";
        private static readonly string datastore = "datastore.xml";

        public IEnumerable<FeedSource> LoadFeedSources()
        {
            return PersistenceUtility.DeserializeFromXML<IEnumerable<FeedSource>>(feedSourceStore);
        }

        public void SaveFeedSources(IEnumerable<FeedSource> sources)
        {
            PersistenceUtility.SerializaToXML<IEnumerable<FeedSource>>(sources, feedSourceStore);
        }
    }
}
