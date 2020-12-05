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
using System.Data.SqlServerCe;

namespace ExpressTMS
{
    /// <summary>
    /// Interaction logic for DriverFilterWnd.xaml
    /// </summary>
    public partial class DriverFilterWnd : Window
    {
        public bool PeriodSelected { get; private set; }
        public DateTime startPeriod { get; private set; }
        public DateTime endPeriod { get; private set; }
        public string DrvName { get; private set; }
        public string DrvLicense { get; private set; }
        public decimal Deduction { get; private set; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SqlCeConnection conn { get; set; }
        private Dictionary<string, string> dct_drivers = new Dictionary<string, string>();
        private Dictionary<string, int> dct_drv_licen = new Dictionary<string, int>();
        private List<string> drivers = new List<string>();

        public DriverFilterWnd()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                PeriodSelected = false;
                SqlCeDataReader myReader = null;
                SqlCeCommand command = new SqlCeCommand("select DRV_COD, DRV_LICENSE, DRV_NAME from Driver", conn);
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    int Code = Convert.ToInt32(myReader["DRV_COD"].ToString());
                    string license = myReader["DRV_LICENSE"].ToString();
                    string name = myReader["DRV_NAME"].ToString();
                    if (!dct_drivers.ContainsKey(license))
                        dct_drivers.Add(license, name);
                    if (!drivers.Contains(name))
                        drivers.Add(name);
                    if (!dct_drv_licen.ContainsKey(license))
                        dct_drv_licen.Add(license, Code);
                }
                myReader.Close();
                myReader.Dispose();
                comboBox1.ItemsSource = drivers;
                textBox2.Text = "25.00";
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            PeriodSelected = false;
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            decimal deduct;
            if(datePicker1.SelectedDate != null && datePicker2.SelectedDate != null &&
                datePicker1.SelectedDate <= datePicker2.SelectedDate &&
                !string.IsNullOrEmpty(comboBox1.Text) &&
                !string.IsNullOrEmpty(textBox1.Text) &&
                !string.IsNullOrEmpty(textBox2.Text) &&
                decimal.TryParse(textBox2.Text, out deduct))
            {
                Deduction = deduct;
                PeriodSelected = true;
                startPeriod = datePicker1.SelectedDate.Value;
                endPeriod = datePicker2.SelectedDate.Value;
                DrvLicense = textBox1.Text;
                DrvName = comboBox1.Text;
                this.Close(); // generate
            }
            
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string name = e.AddedItems[0].ToString();
                if (!string.IsNullOrEmpty(name))
                {
                    if (dct_drivers.ContainsValue(name))
                    {
                        string plate = dct_drivers.SingleOrDefault(r => r.Value == name).Key;
                        textBox1.Text = plate;
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string plate = textBox1.Text;
                if (!string.IsNullOrEmpty(plate))
                {
                    if (dct_drivers.ContainsKey(plate))
                    {
                        string name = dct_drivers[plate];
                        comboBox1.Text = name;
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
