using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace feed_alert.UI
{
    using System.Windows.Forms;
    using Properties;

    class TrayIconUtility
    {
        private static readonly NotifyIcon trayIcon = InitializeTrayIcon();

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
            // _notifyIcon.MouseMove += new MouseEventHandler(_notifyIcon_MouseMove);

            icon.Click += (sender, e) => 
            {
                //new NotifyWindow(null).ShowNotification();
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
            // menuItem_ManageFeedSources.Index = 0;
            menuItem_ManageFeedSources.Click += (sender, e) =>
            {
                if (FeedSourcesWindow.SingletonInstance.IsVisible)
                {
                    FeedSourcesWindow.SingletonInstance.Activate();
                    return;
                }
                FeedSourcesWindow.SingletonInstance.Show();
            };

            menuItem_Settings = new MenuItem("S&ettings");

            menuItem_About = new MenuItem("A&bout");
            // menuItem_About.Index = 1;
            // _menuItem_About.Click += new EventHandler(_menuItem_About_Click);

            menuItem_Exit = new MenuItem("E&xit");
            menuItem_Exit.Index = 2;
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
    }
}
