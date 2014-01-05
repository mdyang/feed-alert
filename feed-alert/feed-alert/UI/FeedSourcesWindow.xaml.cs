using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ContextMenu = System.Windows.Forms.ContextMenu;

namespace feed_alert.UI
{
    using Entity;
    using Persistence;
    using Web;

    /// <summary>
    /// Interaction logic for FeedSources.xaml
    /// </summary>
    public partial class FeedSourcesWindow : Window
    {
        public FeedSourcesWindow()
        {
            InitializeComponent();

            Closing += (s, e) =>
            {
                List<FeedSource> _sources = PersistenceFacade.FeedSources;
                _sources.Clear();
                foreach (object o in sourceList.Items)
                {
                    string[] sourceInfo = (string[])o;
                    FeedSource source = new FeedSource { Name = sourceInfo[0], Url = sourceInfo[1] };
                    _sources.Add(source);
                }

                PersistenceFacade.FeedSources = _sources;
            };

            IList<FeedSource> sources = PersistenceFacade.FeedSources;
            foreach (FeedSource source in sources)
            {
                sourceList.Items.Add(new string[] { source.Name, source.Url });
            }

            feedUrl.Focus();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            AddFeedSource();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (sourceList.SelectedIndex != -1)
            {
                sourceList.Items.RemoveAt(sourceList.SelectedIndex);
            }
        }

        private void feedUrl_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (e.Key == Key.Enter)
            {
                AddFeedSource();
            }
        }

        private void AddFeedSource()
        {
            string url = feedUrl.Text;
            feedUrl.Text = string.Empty;

            foreach (dynamic item in sourceList.Items)
            {
                if (item[1].Equals(url))
                {
                    MessageBox.Show("Already in feed list!");
                    return;
                }
            }

            try
            {
                WebUtility.ReadFeed(WebUtility.MakeHttpRequest(url));
                sourceList.Items.Add(new string[] { CalcFeedName(url), url });
            }
            catch (Exception)
            {
                MessageBox.Show("Please provide a feed source.");
            }
        }

        private static string CalcFeedName(string url)
        {
            Uri uri = new Uri(url);
            StringBuilder sb = new StringBuilder(ProcessDomain(uri.Host));
            string path = ProcessPath(uri.LocalPath);
            Console.WriteLine(path);
            if (!string.IsNullOrEmpty(path))
            {
                sb.Append(path);
            }

            string query = ProcessQuery(uri.Query);
            if (!string.IsNullOrEmpty(query))
            {
                sb.Append(query);
            }

            return sb.ToString();
        }

        private static string ProcessDomain(string domain)
        {
            return string.Join("-", domain.Split('.'));
        }

        private static string ProcessPath(string path)
        {
            Console.WriteLine(path);
            if (!string.IsNullOrEmpty(path))
            {
                return string.Join("-", path.Split('/'));
            }

            return null;
        }

        private static string ProcessQuery(string queryStr)
        {
            if (!string.IsNullOrEmpty(queryStr))
            {
                string[] queries = queryStr.Substring(1).Split('&');
                StringBuilder sb = new StringBuilder();
                foreach (string query in queries)
                {
                    string[] kv = query.Split('=');
                    sb.Append("-");
                    sb.Append(kv[0]);
                    sb.Append("-");
                    sb.Append(HttpUtility.UrlDecode(kv[1]));
                }

                return sb.ToString();
            }

            return null;
        }
    }
}
