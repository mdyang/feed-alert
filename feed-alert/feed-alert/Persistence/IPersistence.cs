namespace feed_alert.Persistence
{
    using Entity;
    using System.Collections.Generic;

    interface IPersistence
    {
        IEnumerable<FeedSource> LoadFeedSources();
        void SaveFeedSources(IEnumerable<FeedSource> sources);
    }
}
