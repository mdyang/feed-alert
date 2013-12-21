namespace feed_alert.Persistence
{
    using Entity;
    using System.Collections.Generic;

    class Persistence
    {
        private static IPersistence persistence = new XMLPersistence();

        public static IEnumerable<FeedSource> LoadFeedSources()
        {
            return persistence.LoadFeedSources();
        }

        void SaveFeedSources(IEnumerable<FeedSource> sources)
        {
            persistence.SaveFeedSources(sources);
        }
    }
}
