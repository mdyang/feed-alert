namespace feed_alert.Persistence
{
    using Entity;
    using System.Collections.Generic;
    using System.Threading;

    class PersistenceFacade
    {
        private static IPersistence persistence = new XMLPersistence();
        private static List<FeedSource> feedSources;
        private static IDictionary<string, FeedSourceState> feedSourceStateStore;

        private static Mutex feedSourcesMutex = new Mutex();
        private static Mutex feedSourcesStatesMutex = new Mutex();

        public static List<FeedSource> FeedSources
        {
            get
            {
                return LoadFeedSources();
            }
            set
            {
                feedSources = value;
            }
        }

        public static List<FeedSource> LoadFeedSources()
        {
            if (feedSources == null)
            {
                feedSources = persistence.LoadFeedSources();
            }

            return feedSources;
        }

        public static void SaveFeedSources()
        {
            persistence.SaveFeedSources(feedSources);
        }

        public static FeedSourceState QueryFeedSourceState(string url)
        {
            if (feedSourceStateStore == null)
            {
                feedSourceStateStore = new Dictionary<string, FeedSourceState>();
                IEnumerable<FeedSourceState> sourceStates = LoadFeedSourceStates();
                foreach (FeedSourceState state in sourceStates)
                {
                    feedSourceStateStore[state.Url] = state;
                }
            }

            FeedSourceState result = null;
            if (feedSourceStateStore.TryGetValue(url, out result))
            {
                return result;
            }

            return null;
        }

        public static void UpdateFeedSourceState(string url, string lastEntry, string lastModified)
        {
            feedSourceStateStore[url] = new FeedSourceState { Url = url, LastEntryUrl = lastEntry, LastModifiedDate = lastModified };
        }

        public static void SaveFeedSourceState()
        {
            if (feedSourceStateStore != null)
            {
                List<FeedSourceState> sourceStates = new List<FeedSourceState>();
                foreach (KeyValuePair<string, FeedSourceState> pair in feedSourceStateStore)
                {
                    sourceStates.Add(pair.Value);
                }

                SaveFeedSourceStates(sourceStates);
            }
        }

        private static List<FeedSourceState> LoadFeedSourceStates()
        {
            return persistence.LoadFeedSourceStates();
        }

        private static void SaveFeedSourceStates(List<FeedSourceState> states)
        {
            persistence.SaveFeedSourceStates(states);
        }
    }
}
