using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace feed_alert.UI
{
    using System.Windows.Forms;
    using Properties;

    class TrayIconUtility
    {
        public static readonly ImageSource notifyIconImgSource = TrayIconUtility.ToImageSource(Properties.Resources.NotifyIcon);

        private static readonly NotifyIcon trayIcon = InitializeTrayIcon();
        private static Window window = null;

        public static NotifyIcon TrayIcon
        {
            get
            {
                return trayIcon;
            }
        }

        private static NotifyIcon InitializeTrayIcon()
        {
            // Create the NotifyIcon.
            NotifyIcon icon = new NotifyIcon 
            { 
                Icon = Resources.TrayIcon, 
                Text = "Right click for more options.",
                ContextMenu = InitializeContextMenu()
            };

            return icon;
        }

        private static ContextMenu InitializeContextMenu()
        {

            MenuItem menuItem_ManageFeedSources;
            MenuItem menuItem_Settings;
            MenuItem menuItem_About;
            MenuItem menuItem_Exit;

            ContextMenu menu = new ContextMenu();

            // Initialize menuItem
            menuItem_ManageFeedSources = new MenuItem("Manage Feed Source&s");
            menuItem_ManageFeedSources.Click += (s, e) =>
            {
                HandleMenuOpenWindow(WindowType.FeedSourceManagement);
            };

            menuItem_Settings = new MenuItem("S&ettings");
            menuItem_Settings.Click += (s, e) =>
            {
                HandleMenuOpenWindow(WindowType.Settings);
            };

            menuItem_About = new MenuItem("A&bout");
            menuItem_About.Click += (s, e) =>
            {
                HandleMenuOpenWindow(WindowType.About);
            };

            menuItem_Exit = new MenuItem("E&xit");
            menuItem_Exit.Click += (sender, e) =>
            {
                App.Current.Shutdown();
            };

            // Initialize contextMenu1
            menu.MenuItems.Add(menuItem_ManageFeedSources);
            menu.MenuItems.Add("-");
            menu.MenuItems.Add(menuItem_Settings);
            menu.MenuItems.Add(menuItem_About);
            menu.MenuItems.Add(menuItem_Exit);

            return menu;
        }

        private static void HandleMenuOpenWindow(WindowType type)
        {
            if (window == null)
            {
                window = BuildWindow(type);
                window.Closing += (_s, _e) =>
                {
                    window = null;
                };
                window.Show();
                return;
            }

            window.Activate();
        }

        private static Window BuildWindow(WindowType type)
        {
            switch (type)
            {
                case WindowType.FeedSourceManagement:
                    return new FeedSourcesWindow();
                case WindowType.Settings:
                    return new ConfigWindow();
                case WindowType.About:
                    return new AboutWindow();
            }

            return null;
        }

        private enum WindowType
        {
            FeedSourceManagement,
            Settings,
            About
        }

        public static ImageSource ToImageSource(Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(
               icon.Handle,
               Int32Rect.Empty,
               BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
