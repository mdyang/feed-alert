namespace feed_alert.Persistence
{
    using Entity;
    using System.Collections.Generic;

    class XMLPersistence: IPersistence
    {
        private static readonly string feedSourceStore = "sources.xml";
        private static readonly string datastore = "datastore.xml";

        public List<FeedSource> LoadFeedSources()
        {
            return PersistenceUtility.DeserializeFromXML<List<FeedSource>>(feedSourceStore);
        }

        public void SaveFeedSources(List<FeedSource> sources)
        {
            PersistenceUtility.SerializaToXML<List<FeedSource>>(sources, feedSourceStore);
        }

        public List<FeedSourceState> LoadFeedSourceStates()
        {
            return PersistenceUtility.DeserializeFromXML<List<FeedSourceState>>(datastore);
        }

        public void SaveFeedSourceStates(List<FeedSourceState> sources)
        {
            PersistenceUtility.SerializaToXML<List<FeedSourceState>>(sources, datastore);
        }
    }
}
