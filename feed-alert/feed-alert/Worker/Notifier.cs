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

        private static CancellationTokenSource tokenSource = new CancellationTokenSource();

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

        public static void Start()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    tokenSource.Token.ThrowIfCancellationRequested();
                    NotificationItem item = GetOneNotification();

                    Task.Factory.StartNew(() => { }).ContinueWith((t) =>
                    {
                        t.Wait();
                        NotifyWindow.DisplayNotification(item);
                    }, App.AppScheduler);
                    Thread.Sleep(1000);
                }
            }, tokenSource.Token);
        }

        public static void Stop()
        {
            tokenSource.Cancel();
        }
    }
}
