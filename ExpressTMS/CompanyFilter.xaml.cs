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
    /// Interaction logic for CompanyFilter.xaml
    /// </summary>
    public partial class CompanyFilter : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SqlCeConnection conn { get; set; }
        private Dictionary<string, int> dct_companies = new Dictionary<string, int>();
        private List<string> companies = new List<string>();
        public bool PeriodSelected { get; private set; }
        public DateTime startPeriod { get; private set; }
        public DateTime endPeriod { get; private set; }
        public string CmpName { get; private set; }

        public CompanyFilter()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try 
            {
                PeriodSelected = false;
                SqlCeDataReader myReader = null;
                SqlCeCommand command = new SqlCeCommand("select CMP_COD, CMP_NAME from Company", conn);
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    int Code = Convert.ToInt32(myReader["CMP_COD"].ToString());
                    string name = myReader["CMP_NAME"].ToString();
                    if (!companies.Contains(name))
                        companies.Add(name);
                    if (!dct_companies.ContainsKey(name))
                        dct_companies.Add(name, Code);
                }
                myReader.Close();
                myReader.Dispose();

                comboBox1.ItemsSource = companies;
            }
            catch (Exception ex) 
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
            if (datePicker1.SelectedDate != null && datePicker2.SelectedDate != null &&
                datePicker1.SelectedDate <= datePicker2.SelectedDate &&
                !string.IsNullOrEmpty(comboBox1.Text))
            {
                PeriodSelected = true;
                startPeriod = datePicker1.SelectedDate.Value;
                endPeriod = datePicker2.SelectedDate.Value;
                CmpName = comboBox1.Text;
                this.Close(); // generate
            }
        }
    }
}
