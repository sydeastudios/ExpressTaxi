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
    /// Interaction logic for AddNewDriver.xaml
    /// </summary>
    public partial class AddNewDriver : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Mode mode;
        public int idx;
        public SqlCeConnection conn { get; set; }

        public AddNewDriver()
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
                    this.Title = "Edit Taxi Driver";
                    DriverDataHandler handler = new DriverDataHandler();
                    handler.conn = conn;
                    if (handler.LoadDriver(idx))
                    {
                        textBox1.Text = handler._DriverName;
                        textBox2.Text = handler._StreetName;
                        comboBox1.Text = handler._VillageName;
                        comboBox2.Text = handler._ParishName;
                        comboBox3.Text = handler._CountryName;
                        textBox3.Text = handler._Phone;
                        textBox4.Text = handler._Cell;
                        textBox5.Text = handler._Email;
                        textBox6.Text = handler._Plate;
                    }
                    else
                    {
                        Config.ShowErrorMessage("Failed to load the Driver Information. Changing to Add Driver Mode.");
                        mode = Mode.MODE_CREATE;
                        this.Title = "Add New Taxi Driver";
                    }
                }
                else
                {
                    mode = Mode.MODE_CREATE;
                    this.Title = "Add New Taxi Driver";
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
            try
            {
                DriverDataHandler handler = new DriverDataHandler();
                handler.conn = conn;
                handler._DriverName = textBox1.Text;
                handler._StreetName = textBox2.Text;
                handler._VillageName = comboBox1.Text;
                handler._ParishName = comboBox2.Text;
                handler._CountryName = comboBox3.Text;
                if (!string.IsNullOrEmpty(textBox3.Text))
                    handler._Phone = textBox3.Text;
                if (!string.IsNullOrEmpty(textBox4.Text))
                    handler._Cell = textBox4.Text;
                if (!string.IsNullOrEmpty(textBox5.Text))
                    handler._Email = textBox5.Text;
                handler._Plate = textBox6.Text;
                if (mode == Mode.MODE_UPDATE)
                    handler.Drv_Code = idx;

                if (handler.SaveDriver())
                {
                    Config.ShowInfoMessage("ExpressTaxi: Successfully saved the driver.");
                    textBox1.Text = null;
                    textBox2.Text = null;
                    textBox3.Text = null;
                    textBox4.Text = null;
                    textBox5.Text = null;
                    comboBox1.Text = null;
                    comboBox2.Text = null;
                    comboBox3.Text = null;
                    textBox6.Text = null;
                    mode = Mode.MODE_CREATE;
                    this.Title = "Add New Taxi Driver";
                }
                else
                    Config.ShowErrorMessage("Driver Validation Failed, cannot save the new driver, please review your data.");
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.FatalError();
            }
        }
    }
}
