using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            icon.Source = TrayIconUtility.notifyIconImgSource;
            linkButton.Click += (sender, e) =>
            {
                Process.Start(Properties.Resources.HomePage);
            };
        }
    }
}
