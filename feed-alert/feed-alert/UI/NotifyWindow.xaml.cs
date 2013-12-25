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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace feed_alert.UI
{
    using Entity;

    /// <summary>
    /// Interaction logic for NotifyWindow.xaml
    /// </summary>
    public partial class NotifyWindow : Window
    {
        private readonly ImageSource appIcon = ToImageSource(Properties.Resources.TrayIcon);
        private readonly ImageSource notifyIcon = ToImageSource(Properties.Resources.NotifyIcon);
        private readonly ImageSource closeIcon = ToImageSource(Properties.Resources.CloseIcon);

        private static readonly int notificationWindowHeight = 96;
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

        public NotifyWindow(NotificationItem item)
        {
            InitializeComponent();
            uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            icon.Source = notifyIcon;
            closeButton.Source = closeIcon;
            this.item = item;

            title.Content = item.Title;
            summary.Text = item.Text;

            fadeInTimer = BuildFadeInTimer();
            fadeOutTimer = BuildFadeOutTimer();
            timeoutTimer = BuildTimeoutTimer();
        }

        public static void DisplayNotification(NotificationItem item)
        {
            new NotifyWindow(item).ShowNotification();
        }

        private static ImageSource ToImageSource(Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(
               icon.Handle,
               Int32Rect.Empty,
               BitmapSizeOptions.FromEmptyOptions());
        }

        private void closeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            closeButton.Source = appIcon;
        }

        private void closeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            closeButton.Source = closeIcon;
        }

        private void text_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (item.Url != null)
            {
                Process.Start(item.Url);
            }

            CloseWindow();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void closeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            CloseWindow();
        }

        private void closeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        public void ShowNotification()
        {
            semaphore.WaitOne();
            mutex.WaitOne();
            slot = PopFirstElement();
            mutex.ReleaseMutex();
            Left = 0;
            Top = slot;
            Opacity = 0;
            Show();
            fadeInTimer.Start();
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
            } while (++i < slots);
            mutex.ReleaseMutex();

            return result;
        }

        private void CloseWindow()
        {
            mutex.WaitOne();
            displaySlots.Add(slot);
            mutex.ReleaseMutex();
            semaphore.Release();
            Close();
        }

        private int PopFirstElement()
        {
            int ret = paddingHeight;
            foreach (int slot in displaySlots)
            {
                ret = slot;
                break;
            }

            displaySlots.Remove(ret);

            return ret; ;
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
    }
}
