namespace feed_alert.Persistence
{
    using Entity;
    using System.Collections.Generic;
    using System.Deployment.Application;
    using System.IO;

    class XMLPersistence: IPersistence
    {
        private static readonly string dataPath = ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.DataDirectory : ".";
        private static readonly string feedSourceStore = Path.Combine(dataPath, "sources.xml");
        private static readonly string datastore = Path.Combine(dataPath, "datastore.xml");

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
