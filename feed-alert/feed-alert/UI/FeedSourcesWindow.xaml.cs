using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        private string CalcFeedName(string url)
        {
            return url;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            List<FeedSource> sources = PersistenceFacade.FeedSources;
            sources.Clear();
            foreach (object o in sourceList.Items)
            {
                string[] sourceInfo = (string[])o;
                FeedSource source = new FeedSource { Name = sourceInfo[0], Url = sourceInfo[1] };
                sources.Add(source);
            }

            PersistenceFacade.FeedSources = sources;
        }
    }
}
