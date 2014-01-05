namespace feed_alert.Persistence
{
    using Entity;
    using System.Collections.Generic;

    interface IPersistence
    {
        List<FeedSource> LoadFeedSources();
        void SaveFeedSources(List<FeedSource> sources);

        List<FeedSourceState> LoadFeedSourceStates();
        void SaveFeedSourceStates(List<FeedSourceState> state);

        Config LoadConfig();
        void SaveConfig(Config config);
    }
}
