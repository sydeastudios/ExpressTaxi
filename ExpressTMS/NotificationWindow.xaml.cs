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
using System.Diagnostics;
using System.Threading;

namespace ExpressTMS
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        public bool isShown { get; private set; }

        public NotificationWindow()
        {
            InitializeComponent();
            //this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - 410;
            //this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - 130;
            this.Topmost = true;
            isShown = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isShown = false;
        }

        public void ShowModal(string msg)
        {
            msgLabel.Text = msg;
            Show();
            isShown = true;
        }

        public void CloseModal()
        {
            Hide();
            isShown = false;
        }

        public void ShowWindow(string msg)
        {
            isShown = true;
            Show();
            msgLabel.Text = msg;
            DispatcherHelper.DoEvents();

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (true)
            {
                if (stopwatch.ElapsedMilliseconds >= 10000)
                    break;

                DispatcherHelper.DoEvents();
                Thread.Sleep(1);
            }
            stopwatch.Stop();
            Hide();
            isShown = false;
        }
    }
}
