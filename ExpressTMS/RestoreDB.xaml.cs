using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExpressTMS
{
    /// <summary>
    /// Interaction logic for RestoreDB.xaml
    /// </summary>
    public partial class RestoreDB : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
                (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<string> r;
        public RestoreDB()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                r = BackupRestore.GetAvailableRestorationPoints();
                listBox1.ItemsSource = r;
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex != -1)
                {
                    NotificationWindow wnd = new NotificationWindow();
                    wnd.ShowModal("Restoring database, please wait...");
                    DispatcherHelper.DoEvents();

                    if (!BackupRestore.RestoreDatabase(listBox1.SelectedValue.ToString()))
                        Config.ShowErrorMessage("Restore has failed.... Cannot restore the database.");

                    DispatcherHelper.DoEvents();
                    wnd.CloseModal();
                    this.Close();
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
