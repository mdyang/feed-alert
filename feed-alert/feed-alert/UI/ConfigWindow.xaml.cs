using System;
using System.Collections.Generic;
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

namespace feed_alert.UI
{
    using Entity;
    using Persistence;

    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {

        public ConfigWindow()
        {
            InitializeComponent();

            Closing += (sender, e) =>
                {
                    int _updatePeriod = 0;
                    int _retentionPeriod = 0;
                    bool error = false;

                    try
                    {
                        _updatePeriod = int.Parse(updatePeriod.Text);
                        _retentionPeriod = int.Parse(retentionPeriod.Text);
                    }
                    catch (Exception)
                    {
                        error = true;
                    }
                    finally
                    {
                        if (error || _updatePeriod <= 0 || _retentionPeriod <= 0)
                        {
                            MessageBox.Show("Please provide valid, large than zero integer value.");
                            e.Cancel = true;
                        }
                    }

                    Config _config = new Config
                    {
                        UpdatePeriod = _updatePeriod,
                        RetetionPeriod = _retentionPeriod,
                        HoldNotifLockScreen = holdNotifLockScreen.IsChecked.Value
                    };

                    PersistenceFacade.UpdateConfig(_config);
                };

            Config config = PersistenceFacade.LoadConfig();
            updatePeriod.Text = config.UpdatePeriod.ToString();
            retentionPeriod.Text = config.RetetionPeriod.ToString();
            holdNotifLockScreen.IsChecked = config.HoldNotifLockScreen;
        }
    }
}
