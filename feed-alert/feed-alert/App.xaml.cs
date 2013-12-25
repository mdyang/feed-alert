using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace feed_alert
{
    using Entity;
    using Persistence;
    using UI;
    using Web;
    using Worker;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static TaskScheduler appScheduler;

        public static TaskScheduler AppScheduler
        {
            get
            {
                return appScheduler;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            appScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            TrayIconUtility.TrayIcon.Visible = true;
            PersistenceFacade.LoadFeedSources();
            PersistenceFacade.LoadFeedSourceStateStore();
            Updater.Start();
            Notifier.Start();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            TrayIconUtility.TrayIcon.Visible = false;
            Updater.Stop();
            Notifier.Stop();
            PersistenceFacade.SaveFeedSources();
            PersistenceFacade.SaveFeedSourceState();
        }
    }
}
