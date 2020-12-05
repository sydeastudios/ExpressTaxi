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
    /// Interaction logic for Lookup.xaml
    /// </summary>
    public partial class Lookup : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SqlCeConnection conn { get; set; }

        public Lookup()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    using (ExpressTaxi ctx = new ExpressTaxi(conn))
                    {
                        Village v = ctx.Villages.SingleOrDefault(r => r.VIL_NAME.ToUpper() == textBox1.Text.ToUpper());
                        if (v == null)
                        {
                            v = new Village();
                            v.VIL_NAME = textBox1.Text;
                            ctx.Villages.InsertOnSubmit(v);
                            ctx.SubmitChanges();
                            Config.ShowInfoMessage("Completed adding the Village to the database.");
                            textBox1.Text = null;
                        }
                        else
                            Config.ShowErrorMessage("The Village already exists in the database, cannot duplicate the entry");
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.ShowErrorMessage("Failed to add the New Village, an internal error has occurred.");
            }
            
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(textBox2.Text))
                {
                    using (ExpressTaxi ctx = new ExpressTaxi(conn))
                    {
                        Parish v = ctx.Parishes.SingleOrDefault(r => r.PAR_NAME.ToUpper() == textBox2.Text.ToUpper());
                        if (v == null)
                        {
                            v = new Parish();
                            v.PAR_NAME = textBox2.Text;
                            ctx.Parishes.InsertOnSubmit(v);
                            ctx.SubmitChanges();
                            Config.ShowInfoMessage("Completed adding the Parish to the database.");
                            textBox2.Text = null;
                        }
                        else
                            Config.ShowErrorMessage("The Parish already exists in the database, cannot duplicate the entry");
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.ShowErrorMessage("Failed to add the New Parish, an internal error has occurred.");
            }
            
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(textBox3.Text))
                {
                    using (ExpressTaxi ctx = new ExpressTaxi(conn))
                    {
                        Country v = ctx.Countries.SingleOrDefault(r => r.COU_NAME.ToUpper() == textBox3.Text.ToUpper());
                        if (v == null)
                        {
                            v = new Country();
                            v.COU_NAME = textBox3.Text;
                            ctx.Countries.InsertOnSubmit(v);
                            ctx.SubmitChanges();
                            Config.ShowInfoMessage("Completed adding the Country to the database.");
                            textBox3.Text = null;
                        }
                        else
                            Config.ShowErrorMessage("The Country already exists in the database, cannot duplicate the entry");
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.ShowErrorMessage("Failed to add the New Country, an internal error has occurred.");
            }
           
        }
    }
}
