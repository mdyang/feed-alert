using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using Timer = System.Windows.Forms.Timer;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace feed_alert.UI
{
    using Entity;

    /// <summary>
    /// Interaction logic for NotifyWindow.xaml
    /// </summary>
    public partial class NotifyWindow : Window
    {
        private static readonly ImageSource appIcon = TrayIconUtility.ToImageSource(Properties.Resources.TrayIcon);
        private static readonly ImageSource closeIcon = TrayIconUtility.ToImageSource(Properties.Resources.CloseIcon);

        private static readonly int notificationWindowHeight = 116;
        private static readonly int paddingHeight = 20;
        private static readonly int timeout = 3000;

        private static Mutex mutex = new Mutex();
        private static Semaphore semaphore = new Semaphore(0, int.MaxValue);
        private static SortedSet<int> displaySlots = CalculateDisplaySlots();

        private TaskScheduler uiScheduler;
        private int slot;
        private NotificationItem item;

        private Timer fadeInTimer;
        private Timer fadeOutTimer;
        private Timer timeoutTimer;

        //SortedSet<>

        private NotifyWindow() { }

        public NotifyWindow(NotificationItem item, int slot)
        {
            InitializeComponent();
            uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            icon.Source = TrayIconUtility.notifyIconImgSource;
            closeButton.Source = closeIcon;
            this.item = item;
            this.slot = slot;

            title.Content = item.Title;
            summary.Text = item.Text;
            timeLabel.Content = GetTimeLabel();

            fadeInTimer = BuildFadeInTimer();
            fadeOutTimer = BuildFadeOutTimer();
            timeoutTimer = BuildTimeoutTimer();

            Closed += (sender, e) =>
            {
            };
        }

        public static void DisplayNotification(NotificationItem item)
        {
            int slot = PopFirstElement();
            
            Task.Factory.StartNew(() => { }).ContinueWith((t) =>
            {
                new NotifyWindow(item, slot)
                {
                    Left = 0,
                    Top = slot,
                    Opacity = 0,
                }.FadeIn();
            }, App.AppScheduler);
        }

        private void closeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            closeButton.Source = appIcon;
        }

        private void closeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            closeButton.Source = closeIcon;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (item.Url != null)
            {
                Process.Start(item.Url);
            }

            CloseWindow();
        }

        private void closeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            CloseWindow();
        }

        private Timer BuildFadeOutTimer()
        {
            Timer timer = new Timer();
            timer.Tick += (sender, e) =>
            {
                Opacity -= 0.01;
                if (Opacity <= 0.0)
                {
                    timer.Stop();
                    CloseWindow();
                }
            };
            timer.Interval = 20;
            return timer;
        }

        private Timer BuildTimeoutTimer()
        {
            Timer timer = new Timer();
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                fadeOutTimer.Start();
            };
            timer.Interval = timeout;
            return timer;
        }

        private Timer BuildFadeInTimer()
        {
            Timer timer = new Timer();
            timer.Tick += (sender, e) =>
            {
                Opacity += 0.1;
                if (Opacity >= 1.0)
                {
                    timer.Stop();
                    timeoutTimer.Start();
                }
            };
            timer.Interval = 20;
            return timer;
        }

        private static SortedSet<int> CalculateDisplaySlots()
        {
            SortedSet<int> result = new SortedSet<int>();
            int slotHeight = paddingHeight + notificationWindowHeight;
            int slots = (int)SystemParameters.VirtualScreenHeight / slotHeight;
            int current = paddingHeight;
            int i = 0;
            mutex.WaitOne();
            
            do
            {
                result.Add(current);
                semaphore.Release();
                current += slotHeight;
            }
            while (++i < slots);
            
            mutex.ReleaseMutex();

            return result;
        }

        private void CloseWindow()
        {
            ReturnSlot();
            Close();
        }

        private static int PopFirstElement()
        {
            semaphore.WaitOne();
            mutex.WaitOne();
            int ret = displaySlots.FirstOrDefault();
            displaySlots.Remove(ret);
            mutex.ReleaseMutex();

            return ret;
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            fadeOutTimer.Stop();
            timeoutTimer.Stop();
            Opacity = 1.0;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            timeoutTimer.Start();
        }

        private string GetTimeLabel()
        {
            TimeSpan ts = DateTime.UtcNow - item.PublishDate;

            if (ts.Hours > 0)
            {
                if (ts.Seconds > 0)
                {
                    return string.Format("{0}h {1}m ago", ts.Hours, ts.Seconds);
                }
                return string.Format("{0}h ago", ts.Hours);
            }

            if (ts.Minutes < 5)
            {
                return string.Format("just now");
            }

            return string.Format("{0}m ago", ts.Minutes);
        }

        private void FadeIn()
        {
            Show();
            fadeInTimer.Start();
        }

        private void ReturnSlot()
        {
            mutex.WaitOne();
            displaySlots.Add(slot);
            mutex.ReleaseMutex();
            semaphore.Release();
        }
    }
}
