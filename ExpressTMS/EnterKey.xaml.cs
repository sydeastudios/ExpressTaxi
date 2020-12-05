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
    /// Interaction logic for EnterKey.xaml
    /// </summary>
    public partial class EnterKey : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EnterKey()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) &&
                !string.IsNullOrEmpty(textBox2.Text) &&
                !string.IsNullOrEmpty(textBox3.Text))
            {

                string[] skey = textBox2.Text.Split('-');
                if (skey.Length == 4 && textBox2.Text.Length == 23)
                {
                    SerialKey key = new SerialKey();
                    key._BusinessName = textBox1.Text;
                    key._KeyPartA = skey[0];
                    key._KeyPartB = skey[1];
                    key._KeyPartC = skey[2];
                    key._KeyPartD = skey[3];
                    key._KeyHashCode = textBox3.Text;
                    if (ValidateSerialKey.ValidateSK(key))
                    {
                        try
                        {
                            string LicenseFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ExpressTaxi.lic";
                            string sKey = string.Format("{0}-{1}-{2}-{3}", key._KeyPartA, key._KeyPartB, key._KeyPartC, key._KeyPartD);
                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(LicenseFile))
                            {
                                file.WriteLine(string.Format("BusinessName:{0}:ExpressTaxi:1.1", key._BusinessName));
                                file.WriteLine(string.Format("SerialKey:{0}:ExpressTaxi:1.1", sKey));
                                file.WriteLine(string.Format("ActivationCode:{0}:ExpressTaxi:1.1", key._KeyHashCode));
                            }
                            Config.ShowInfoMessage("License successfully validated, Please restart ExpressTaxi for it to take effect.");
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            Config.ShowErrorMessage("Failed to save license file.");
                        }
                    }else
                        Config.ShowErrorMessage("Validation Failed. The information entered is not valid to activate ExpressTaxi.");
                }
                else
                    Config.ShowErrorMessage("Invalid Serial Key Entered.");
            }else
                Config.ShowErrorMessage("The information entered is not valid to activate ExpressTaxi.");
        }
    }
}
