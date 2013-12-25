﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
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

        private static bool running = false;

        private static bool mutexCreated = false;
        private static Mutex mutex = new Mutex(true, @"26041D0E_E96A_436D_B1C0_E6C6BDA3CA80", out mutexCreated);

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // ensure single instance
            if (!mutexCreated)
            {
                Current.Shutdown();
                return;
            }

            appScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            TrayIconUtility.TrayIcon.Visible = true;
            PersistenceFacade.LoadFeedSources();
            PersistenceFacade.LoadFeedSourceStateStore();
            Updater.Start();
            Notifier.Start();
            running = true;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (running)
            {
                TrayIconUtility.TrayIcon.Visible = false;
                Updater.Stop();
                Notifier.Stop();
                PersistenceFacade.SaveFeedSources();
                PersistenceFacade.SaveFeedSourceState();
            }
        }
    }
}
