using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;

namespace feed_alert.Entity
{
    class NotificationItem
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }

        public static NotificationItem FromSyndicationItem(SyndicationItem item)
        {
            return new NotificationItem
            {
                Title = item.Title.Text,
                Text = item.Summary.Text,
                Url = item.Links[0].Uri.ToString()
            };
        }
    }
}
