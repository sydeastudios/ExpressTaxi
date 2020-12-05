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
    /// Interaction logic for DriverSearch.xaml
    /// </summary>
    public partial class DriverSearch : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SqlCeConnection conn { get; set; }
        public myDriver drivers = new myDriver();
        public int? idx;

        public DriverSearch()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                /* load lookup info */
                idx = null;
                SqlCeDataReader myReader = null;
                SqlCeCommand command = new SqlCeCommand("select VIL_NAME from Village", conn);
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    comboBox1.Items.Add(myReader["VIL_NAME"].ToString());
                }
                myReader.Close();
                myReader.Dispose();

                command = new SqlCeCommand("select PAR_NAME from Parish", conn);
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    comboBox2.Items.Add(myReader["PAR_NAME"].ToString());
                }
                myReader.Close();
                myReader.Dispose();

                command = new SqlCeCommand("select COU_NAME from Country", conn);
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    comboBox3.Items.Add(myReader["COU_NAME"].ToString());
                }
                myReader.Close();
                myReader.Dispose();

                listView1.ItemsSource = drivers;
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.FatalError();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            idx = null;
            this.Close();
        }

        private void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView1.SelectedIndex == -1)
                idx = null;
            else
            {
                int ID = drivers.IndexOf((Driver)e.AddedItems[0]);
                Driver c = drivers[ID];
                idx = c.DRV_COD;
            }
            e.Handled = true;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                drivers.Clear();
                using (ExpressTaxi ctx = new ExpressTaxi(conn))
                {
                    bool bHasCriteria = false;
                    var query = from c in ctx.Drivers select c;
                    if (!string.IsNullOrEmpty(textBox1.Text))
                    {
                        bHasCriteria = true;
                        string Name = textBox1.Text.Replace('*', '%');
                        query = query.Where(r => SqlMethods.Like(r.DRV_NAME, Name));
                    }
                    if (!string.IsNullOrEmpty(textBox2.Text))
                    {
                        bHasCriteria = true;
                        string Name = textBox2.Text.Replace('*', '%');
                        query = query.Where(r => SqlMethods.Like(r.DRV_ADDRESS, Name));
                    }
                    if (!string.IsNullOrEmpty(comboBox1.Text))
                    {
                        bHasCriteria = true;
                        query = query.Where(r => r.VIL_NAME == comboBox1.Text);
                    }
                    if (!string.IsNullOrEmpty(comboBox2.Text))
                    {
                        bHasCriteria = true;
                        query = query.Where(r => r.PAR_NAME == comboBox2.Text);
                    }
                    if (!string.IsNullOrEmpty(comboBox3.Text))
                    {
                        bHasCriteria = true;
                        query = query.Where(r => r.COU_NAME == comboBox3.Text);
                    }
                    if(!string.IsNullOrEmpty(textBox3.Text))
                    {
                        bHasCriteria = true;
                        string Name = textBox3.Text.Replace('*', '%');
                        query = query.Where(r => SqlMethods.Like(r.DRV_LICENSE, Name));
                    }

                    if (!bHasCriteria)
                        query = query.Where(r => r.DRV_COD == -1);

                    if (query.Count() > 0)
                    {
                        foreach (Driver c in query)
                            drivers.Add(c);
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.FatalError();
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (idx != null)
            {
                this.Close();
            }
        }
    }
}
