﻿namespace feed_alert.Persistence
{
    using Entity;
    using System.Collections.Generic;

    class PersistenceFacade
    {
        private static IPersistence persistence = new XMLPersistence();
        private static List<FeedSource> feedSources;
        private static IDictionary<string, FeedSourceState> feedSourceStateStore;

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

        public static void UpdateFeedSourceState()
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