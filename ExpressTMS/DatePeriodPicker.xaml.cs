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
    /// Interaction logic for DatePeriodPicker.xaml
    /// </summary>
    public partial class DatePeriodPicker : Window
    {
        public bool PeriodSelected { get; private set; }
        public DateTime startPeriod { get; private set; }
        public DateTime endPeriod { get; private set; }

        public DatePeriodPicker()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (datePicker1.SelectedDate != null &&
                datePicker2.SelectedDate != null &&
                datePicker1.SelectedDate <= datePicker2.SelectedDate)
            {
                startPeriod = datePicker1.SelectedDate.Value;
                endPeriod = datePicker2.SelectedDate.Value;
                PeriodSelected = true;
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PeriodSelected = false;
            datePicker1.SelectedDate = null;
            datePicker2.SelectedDate = null;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            PeriodSelected = false;
            this.Close();
        }
    }
}
