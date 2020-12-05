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
    /// Interaction logic for EULA.xaml
    /// </summary>
    public partial class EULA : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
                (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public bool Accepted { get; private set; }
        public EULA()
        {
            InitializeComponent();
            Accepted = false;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Accepted = false;
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Accepted = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // load EULA if can't exit the application
            try
            {
                string Eula = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Eula.txt";
                if (!string.IsNullOrEmpty(Eula))
                {
                    string content = System.IO.File.ReadAllText(Eula);
                    textBox1.Text = content;
                }
                else
                {
                    MessageBox.Show("Failed to find license file, could not read the software license agreement.", "ExpressTaxi", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                MessageBox.Show("Failed to find license file, could not read the software license agreement.", "ExpressTaxi", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }
    }
}
