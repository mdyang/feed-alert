using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feed_alert.Worker
{
    using Entity;
    using System.Threading;
    using System.Windows.Forms;
    using UI;

    class Notifier
    {
        private static ConcurrentQueue<NotificationItem> notificationQueue = new ConcurrentQueue<NotificationItem>();
        private static Semaphore empty = new Semaphore(0, int.MaxValue);
        private static string currentUrl = null;

        public static void AddNotification(NotificationItem item)
        {
            notificationQueue.Enqueue(item);
            empty.Release();
        }

        private static NotificationItem GetOneNotification()
        {
            NotificationItem not;
            empty.WaitOne();
            if (notificationQueue.TryDequeue(out not))
            {
                return not;
            }

            return null;
        }

        public static void StartNotifier()
        {
            AutoResetEvent are = new AutoResetEvent(true);

            TrayIconUtility.TrayIcon.BalloonTipClosed += (s, e) =>
                {
                    currentUrl = null;
                    are.Set();
                };

            TrayIconUtility.TrayIcon.BalloonTipClicked += (s, e) =>
            {
                if (currentUrl != null)
                {
                    Process.Start(currentUrl);
                }
            };

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    are.WaitOne();
                    NotificationItem item = GetOneNotification();
                    currentUrl = item.Url;
                    TrayIconUtility.TrayIcon.ShowBalloonTip(1000, item.Title, item.Url, ToolTipIcon.Info);
                }
            });
        }


    }
}
