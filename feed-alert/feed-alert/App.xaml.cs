using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace feed_alert
{
    using Entity;
    using Persistence;
    using UI;
    using Web;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            TrayIconUtility.TrayIcon.Visible = true;
            PersistenceFacade.LoadFeedSources();

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            TrayIconUtility.TrayIcon.Visible = false;
            PersistenceFacade.SaveFeedSources();
            PersistenceFacade.UpdateFeedSourceState();
        }
    }
}
