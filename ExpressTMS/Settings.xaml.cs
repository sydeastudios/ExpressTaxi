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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SqlCeConnection conn { get; set; }
        public Settings()
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
                using (ExpressTaxi ctx = new ExpressTaxi(conn))
                {
                    UniqueNumber u = ctx.UniqueNumbers.SingleOrDefault(r => r.UniqueName == "Receipt");
                    if (u != null)
                    {
                        textBox7.Text = Convert.ToString(u.UniqueValue);
                    }
                }
                textBox1.Text = Config.CMP_NAME;
                textBox2.Text = Config.CMP_ADDRESSLINE1;
                textBox3.Text = Config.CMP_ADDRESSLINE2;
                textBox4.Text = Config.CMP_PHONEFAX;
                textBox5.Text = Config.Subject;
                textBox6.Text = Config.BodyText;
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ExpressTaxi ctx = new ExpressTaxi(conn))
                {
                    UniqueNumber u = ctx.UniqueNumbers.SingleOrDefault(r => r.UniqueName == "Receipt");
                    if (u != null)
                    {
                        int num;
                        if (int.TryParse(textBox7.Text, out num))
                        {
                            u.UniqueValue = num;
                            ctx.SubmitChanges();
                        }
                        else
                            Config.ShowErrorMessage("Invalid doc number, failed to save this component.");
                    }
                }
                Config.CMP_NAME = textBox1.Text;
                Config.CMP_ADDRESSLINE1 = textBox2.Text;
                Config.CMP_ADDRESSLINE2 = textBox3.Text;
                Config.CMP_PHONEFAX = textBox4.Text;
                Config.Subject = textBox5.Text;
                Config.BodyText = textBox6.Text;
                if (!Config.SaveCfg())
                    Config.ShowErrorMessage("Failed to save configuration, this error has been logged.");
                else
                    this.Close();
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.ShowErrorMessage("Failed to save configuration, this error has been logged.");
            }
        }
    }
}
