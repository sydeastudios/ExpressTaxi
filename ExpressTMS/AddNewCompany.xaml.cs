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
    /// Interaction logic for AddNewCompany.xaml
    /// </summary>
    public partial class AddNewCompany : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Mode mode;
        public int idx;
        public SqlCeConnection conn { get; set; }

        public AddNewCompany()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                /* load lookup info */
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

                if (mode == Mode.MODE_UPDATE)
                {
                    this.Title = "Edit Tour Company";
                    CompanyDataHandler handler = new CompanyDataHandler();
                    handler.conn = conn;
                    if (handler.LoadCompany(idx))
                    {
                        textBox1.Text = handler._CompanyName;
                        textBox2.Text = handler._StreetName;
                        comboBox1.Text = handler._VillageName;
                        comboBox2.Text = handler._ParishName;
                        comboBox3.Text = handler._CountryName;
                        textBox3.Text = handler._Phone;
                        textBox4.Text = handler._Fax;
                        textBox5.Text = handler._Email;
                    }
                    else
                    {
                        Config.ShowErrorMessage("Failed to load the Company Information. Changing to Add Company Mode.");
                        mode = Mode.MODE_CREATE;
                        this.Title = "Add New Tour Company";
                    }
                }
                else
                {
                    mode = Mode.MODE_CREATE;
                    this.Title = "Add New Tour Company";
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.FatalError();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // save
            try
            {
                CompanyDataHandler handler = new CompanyDataHandler();
                handler.conn = conn;
                handler._CompanyName = textBox1.Text;
                handler._StreetName = textBox2.Text;
                handler._VillageName = comboBox1.Text;
                handler._ParishName = comboBox2.Text;
                handler._CountryName = comboBox3.Text;
                if(!string.IsNullOrEmpty(textBox3.Text))
                    handler._Phone = textBox3.Text;
                if(!string.IsNullOrEmpty(textBox4.Text))
                    handler._Fax = textBox4.Text;
                if(!string.IsNullOrEmpty(textBox5.Text))
                    handler._Email = textBox5.Text;
                if (handler.SaveCompany())
                {
                    Config.ShowInfoMessage("ExpressTaxi: Successfully saved the company.");
                    textBox1.Text = null;
                    textBox2.Text = null;
                    textBox3.Text = null;
                    textBox4.Text = null;
                    textBox5.Text = null;
                    comboBox1.Text = null;
                    comboBox2.Text = null;
                    comboBox3.Text = null;
                    mode = Mode.MODE_CREATE;
                    this.Title = "Add New Tour Company";
                }
                else
                    Config.ShowErrorMessage("Company Validation Failed, cannot save the new company, please review your data.");
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.FatalError();
            }
        }
    }

    public enum Mode
    {
        MODE_CREATE,
        MODE_UPDATE,
        MODE_VOID,
        MODE_VIEWRO,
        MODE_COPYC
    }
}
