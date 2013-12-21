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
    /// <summary>
    /// Interaction logic for FeedSources.xaml
    /// </summary>
    public partial class FeedSourcesWindow : Window
    {
        public FeedSourcesWindow()
        {
            InitializeComponent();
            feedUrl.Focus();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            AddFeedSource();
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void feedUrl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                AddFeedSource();
            }
        }

        private void AddFeedSource()
        {
            string url = feedUrl.Text;
            feedUrl.Text = string.Empty;

            sourceList.Items.Add(new string[] { url, CalcFeedName(url) });
        }

        private string CalcFeedName(string url)
        {
            return url;
        }
    }
}
