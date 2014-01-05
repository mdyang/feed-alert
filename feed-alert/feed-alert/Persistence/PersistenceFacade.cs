namespace feed_alert.Persistence
{
    using Entity;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    class PersistenceFacade
    {
        private static IPersistence persistence = new XMLPersistence();
        private static Config config;
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

        public static void LoadFeedSourceStateStore()
        {
            feedSourceStateStore = new Dictionary<string, FeedSourceState>();
            IEnumerable<FeedSourceState> sourceStates = LoadFeedSourceStates();
            foreach (FeedSourceState state in sourceStates)
            {
                feedSourceStateStore[state.Url] = state;
            }
        }

        public static void SaveFeedSources()
        {
            persistence.SaveFeedSources(feedSources);
        }

        public static FeedSourceState QueryFeedSourceState(string url)
        {
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
                FeedSourceState state;
                List<FeedSourceState> sourceStates = new List<FeedSourceState>();
                foreach (FeedSource source in FeedSources)
                {
                    if (feedSourceStateStore.TryGetValue(source.Url, out state))
                    {
                        sourceStates.Add(state);
                    }
                }

                SaveFeedSourceStates(sourceStates);
            }
        }

        public static void Load()
        {
            LoadFeedSources();
            LoadFeedSourceStateStore();
            LoadConfig();
        }

        public static void Save()
        {
            SaveFeedSources();
            SaveFeedSourceState();
            SaveConfig();
        }

        private static List<FeedSourceState> LoadFeedSourceStates()
        {
            return persistence.LoadFeedSourceStates();
        }

        private static void SaveFeedSourceStates(List<FeedSourceState> states)
        {
            persistence.SaveFeedSourceStates(states);
        }

        public static Config LoadConfig()
        {
            if (config == null)
            {
                try
                {
                    config = persistence.LoadConfig();
                }
                catch (Exception)
                {
                    // if there is still no config file, create one
                    config = new Config { UpdatePeriod = 1, RetetionPeriod = 6, HoldNotifLockScreen = true };
                }
            }
            return config;
        }

        public static void SaveConfig()
        {
            persistence.SaveConfig(config);
        }

        public static void UpdateConfig(Config conf)
        {
            config = conf;
        }
    }
}
