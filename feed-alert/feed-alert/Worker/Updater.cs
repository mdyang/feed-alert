using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Windows.Forms;

namespace feed_alert.Worker
{
    using Entity;
    using Persistence;
    using UI;
    using Web;

    class Updater
    {
        public static void StartUpdateLoop()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    IList<FeedSource> sources = PersistenceFacade.FeedSources;
                    List<Task> tasks = new List<Task>();
                    foreach (FeedSource source in sources)
                    {
                        tasks.Add(StartUpdateTask(source.Url));
                    }

                    Task.WhenAll(tasks).Wait();
                    Thread.Sleep(60000);
                }
            });
        }

        public static Task StartUpdateTask(string url)
        {
            return Task.Factory.StartNew(() => 
            {
                FeedSourceState state = PersistenceFacade.QueryFeedSourceState(url);
                string lastModified = null;
                if (state != null)
                {
                    lastModified = state.LastModifiedDate;
                }

                HttpWebResponse response = WebUtility.MakeHttpRequest(url, lastModified);
                
                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    return;
                }

                lastModified = response.Headers["Date"];

                SyndicationFeed feed = WebUtility.ReadFeed(response);
                if (feed == null)
                {
                    return;
                }

                bool first = true;
                string lastEntry = null;
                foreach (SyndicationItem item in feed.Items)
                {
                    if (first)
                    {
                        first = false;
                        lastEntry = item.Links[0].Uri.ToString();
                    }

                    Notifier.AddNotification(NotificationItem.FromSyndicationItem(item));
                }

                PersistenceFacade.UpdateFeedSourceState(url, lastEntry, lastModified);
            });
        }
    }
}
