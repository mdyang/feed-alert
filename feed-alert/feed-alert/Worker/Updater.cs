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
        private static CancellationTokenSource tokenSource = new CancellationTokenSource();

        public static void Start()
        {
            Task.Factory.StartNew(() =>
            {
                List<Task> tasks = new List<Task>();
                while (true)
                {
                    tokenSource.Token.ThrowIfCancellationRequested();
                    tasks.Clear();
                    foreach (FeedSource source in PersistenceFacade.FeedSources)
                    {
                        tasks.Add(StartUpdateTask(source.Url));
                    }

                    Task.WaitAll(tasks.ToArray());
                    
                    Thread.Sleep(60000);
                }
            }, tokenSource.Token);
        }

        public static void Stop()
        {
            tokenSource.Cancel();
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
                
                if (response == null)
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
                DateTime now = DateTime.UtcNow;
                foreach (SyndicationItem item in feed.Items)
                {
                    if (first)
                    {
                        first = false;
                        lastEntry = item.Links[0].GetAbsoluteUri().ToString();
                    }

                    if (now - item.PublishDate.DateTime > new TimeSpan(0, 0, 60, 0) ||
                        (state != null && state.LastEntryUrl.Equals(item.Links[0].GetAbsoluteUri().ToString())))
                    {
                        break;
                    }
                    Notifier.AddNotification(NotificationItem.FromSyndicationItem(item));
                }

                PersistenceFacade.UpdateFeedSourceState(url, lastEntry, lastModified);
            });
        }
    }
}
