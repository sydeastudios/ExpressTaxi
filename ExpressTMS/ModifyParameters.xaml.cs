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
    /// Interaction logic for ModifyParameters.xaml
    /// </summary>
    public partial class ModifyParameters : Window
    {
        private myTax taxes = new myTax();
        public SqlCeConnection conn { get; set; }
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private int? idx;

        public ModifyParameters()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                button3.IsEnabled = false;
                idx = null;
                SqlCeDataReader myReader = null;
                SqlCeCommand command = new SqlCeCommand("select TaxName, TaxAmount, TaxDescription from AppliedTaxes", conn);
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    AppliedTax t = new AppliedTax();
                    t.TaxAmount = Convert.ToDecimal(myReader["TaxAmount"].ToString());
                    t.TaxDescription = myReader["TaxDescription"].ToString();
                    t.TaxName = myReader["TaxName"].ToString();
                    taxes.Add(t);
                }
                myReader.Close();
                myReader.Dispose();

                listView1.ItemsSource = taxes;
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
                decimal amt;
                if (decimal.TryParse(textBox2.Text, out amt))
                {
                    using (ExpressTaxi ctx = new ExpressTaxi(conn))
                    {
                        AppliedTax t = ctx.AppliedTaxes.SingleOrDefault(r => r.TaxName == textBox1.Text);
                        if (t == null)
                        {
                            t = new AppliedTax();
                            t.TaxName = textBox1.Text;
                            t.TaxAmount = amt;
                            t.TaxDescription = textBox3.Text;
                            ctx.AppliedTaxes.InsertOnSubmit(t);
                            ctx.SubmitChanges();

                            taxes.Add(t);
                            textBox1.Text = null;
                            textBox2.Text = null;
                            textBox3.Text = null;
                        }
                        else
                            Config.ShowErrorMessage(string.Format("The tax: {0} already exists.", textBox1.Text));
                    }
                }
                else
                    Config.ShowErrorMessage("Failed to convert the amount to a decimal value, please review input data.");

            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.ShowErrorMessage("Failed to save the new tax parameter to the active database.");
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView1.SelectedIndex == -1)
            {
                idx = null;
                button3.IsEnabled = false;
            }
            else
            {
                idx = taxes.IndexOf((AppliedTax)e.AddedItems[0]);
                button3.IsEnabled = true;
            }
            e.Handled = true;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (idx != null)
                {
                    string taxname = taxes[idx.Value].TaxName;
                    if (MessageBox.Show(string.Format("Are you sure that you want to delete the following tax: {0}", taxname), "ExpressTaxi", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        using (ExpressTaxi ctx = new ExpressTaxi(conn))
                        {
                            AppliedTax t1 = ctx.AppliedTaxes.SingleOrDefault(t => t.TaxName == taxname);
                            AppliedTax t2 = taxes[idx.Value];

                            ctx.AppliedTaxes.DeleteOnSubmit(t1);
                            ctx.SubmitChanges();

                            taxes.Remove(t2);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.ShowErrorMessage("Failed to delete the tax, an unspecified error occurred. The error has been logged.");
            }
        }
    }
}
