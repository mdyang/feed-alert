using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;

namespace feed_alert.Entity
{
    public class NotificationItem
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }

        private static readonly int maxLength = 140;

        public static NotificationItem FromSyndicationItem(SyndicationItem item)
        {
            return new NotificationItem
            {
                Title = item.Title.Text,
                Text = ExtractSummaryText(item),
                Url = item.Links[0].Uri.ToString()
            };
        }

        private static string ExtractSummaryText(SyndicationItem item)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(item.Summary.Text);
            string summary = doc.DocumentNode.InnerText.Replace("\n\n", "\n").Trim();
            if (summary.Length > maxLength)
            {
                summary = summary.Substring(0, maxLength) + "...";
            }

            return summary;
        }
    }
}
