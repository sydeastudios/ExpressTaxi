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
using System.Data.Linq.SqlClient;

namespace ExpressTMS
{
    /// <summary>
    /// Interaction logic for TransactionSearch.xaml
    /// </summary>
    public partial class TransactionSearch : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SqlCeConnection conn { get; set; }
        public myTransactions transactions = new myTransactions();
        public int? idx;

        public TransactionSearch()
        {
            InitializeComponent();
        }

        private void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView1.SelectedIndex == -1)
                idx = null;
            else
            {
                int ID = transactions.IndexOf((FinancialTransaction)e.AddedItems[0]);
                FinancialTransaction c = transactions[ID];
                idx = c.DOC_NUM;
            }
            e.Handled = true;
        }

        private void listView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (idx != null)
            {
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                idx = null;
                SqlCeDataReader myReader = null;
                SqlCeCommand command = new SqlCeCommand("select DRV_NAME from Driver", conn);
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    comboBox1.Items.Add(myReader["DRV_NAME"].ToString());
                }
                myReader.Close();
                myReader.Dispose();

                command = new SqlCeCommand("select CMP_NAME from Company", conn);
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    comboBox2.Items.Add(myReader["CMP_NAME"].ToString());
                }
                myReader.Close();
                myReader.Dispose();

                command = new SqlCeCommand("select SERV_NAME from Service_Type", conn);
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    comboBox3.Items.Add(myReader["SERV_NAME"].ToString());
                }
                myReader.Close();
                myReader.Dispose();

                command = new SqlCeCommand("select DOC_TYPE_NAME from Document_Type", conn);
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    comboBox4.Items.Add(myReader["DOC_TYPE_NAME"].ToString());
                }
                myReader.Close();
                myReader.Dispose();
                listView1.ItemsSource = transactions;
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.FatalError();
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            idx = null;
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // search
            try
            {
                transactions.Clear();
                using (ExpressTaxi ctx = new ExpressTaxi(conn))
                {
                    bool bHasCriteria = false;
                    var query = from c in ctx.FinancialTransactions select c;
                    if (!string.IsNullOrEmpty(textBox1.Text))
                    {
                        int docNo;
                        if (int.TryParse(textBox1.Text, out docNo))
                        {
                            bHasCriteria = true;
                            query = query.Where(r => r.DOC_NUM == docNo);
                        }
                    }
                    if (datePicker1.SelectedDate != null)
                    {
                        bHasCriteria = true;
                        query = query.Where(r => r.DOCUMENT_DATE == datePicker1.SelectedDate.Value);
                    }
                    if (!string.IsNullOrEmpty(comboBox1.Text))
                    {
                        bHasCriteria = true;
                        string Name = comboBox1.Text.Replace('*', '%');
                        query = query.Where(r => SqlMethods.Like(r.Driver.DRV_NAME, Name));
                    } 
                    if (!string.IsNullOrEmpty(comboBox2.Text))
                    {
                        bHasCriteria = true;
                        string Name = comboBox2.Text.Replace('*', '%');
                        query = query.Where(r => SqlMethods.Like(r.Company.CMP_NAME, Name));
                    }
                    if (!string.IsNullOrEmpty(textBox2.Text))
                    {
                        bHasCriteria = true;
                        string Name = textBox2.Text.Replace('*', '%');
                        query = query.Where(r => SqlMethods.Like(r.TRANS_DESTINATION, Name));
                    }
                    if (!string.IsNullOrEmpty(comboBox3.Text))
                    {
                        bHasCriteria = true;
                        string Name = comboBox3.Text;
                        query = query.Where(r => r.SERV_NAME == Name);
                    }
                    if (!string.IsNullOrEmpty(comboBox4.Text))
                    {
                        bHasCriteria = true;
                        string Name = comboBox4.Text;
                        query = query.Where(r => r.DOC_TYPE_NAME == Name);
                    }
                    if (!bHasCriteria)
                        query = query.Where(r => r.TRANS_COD == -1);

                    if (query.Count() > 0)
                    {
                        foreach (FinancialTransaction c in query)
                        {
                            c.DRV_NAME_TX = ctx.Drivers.SingleOrDefault(r => r.DRV_COD == c.DRV_COD).DRV_NAME;
                            transactions.Add(c);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.FatalError();
            }
        }


    }
}
