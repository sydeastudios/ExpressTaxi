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
    /// Interaction logic for AddTransaction.xaml
    /// </summary>
    public partial class AddTransaction : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Mode mode;
        public int? idx;
        public SqlCeConnection conn { get; set; }
        private Dictionary<string, string> dct_drivers = new Dictionary<string, string>();
        private Dictionary<string, int> dct_drv_licen = new Dictionary<string, int>();
        private Dictionary<string, int> dct_companies = new Dictionary<string, int>();
        private List<string> drivers = new List<string>();
        private List<string> companies = new List<string>();
        private List<string> servicetype = new List<string>();
        private List<string> documenttype = new List<string>();
        public int? DOC_NUM;

        public AddTransaction()
        {
            InitializeComponent();
        }

        private void AddCompany_Click(object sender, RoutedEventArgs e)
        {
            AddNewCompany dlg = new AddNewCompany();
            dlg.conn = conn;
            dlg.mode = Mode.MODE_CREATE;
            dlg.ShowDialog();

            try
            {
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
                comboBox2.ItemsSource = companies;
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }

        private void AddDriver_Click(object sender, RoutedEventArgs e)
        {
            AddNewDriver dlg = new AddNewDriver();
            dlg.conn = conn;
            dlg.mode = Mode.MODE_CREATE;
            dlg.ShowDialog();

            try
            {
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
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                label16.Visibility = Visibility.Hidden;
                DOC_NUM = null;
                SqlCeDataReader myReader = null;
                SqlCeCommand command = new SqlCeCommand("select UniqueValue from UniqueNumbers Where UniqueName='Receipt'", conn);
                myReader = command.ExecuteReader();
                if (myReader.Read())
                {
                    textBox1.Text = myReader["UniqueValue"].ToString();
                    textBox2.Text = DateTime.Today.ToShortDateString();
                }
                myReader.Close();
                myReader.Dispose();

                command = new SqlCeCommand("select DRV_COD, DRV_LICENSE, DRV_NAME from Driver", conn);
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

                command = new SqlCeCommand("select CMP_COD, CMP_NAME from Company", conn);
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

                command = new SqlCeCommand("select SERV_NAME from Service_Type", conn);
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    string name = myReader["SERV_NAME"].ToString();
                    if (!servicetype.Contains(name))
                        servicetype.Add(name);
                }
                myReader.Close();
                myReader.Dispose();

                command = new SqlCeCommand("select DOC_TYPE_NAME from Document_Type", conn);
                myReader = command.ExecuteReader();
                while (myReader.Read())
                {
                    string name = myReader["DOC_TYPE_NAME"].ToString();
                    if (!documenttype.Contains(name))
                        documenttype.Add(name);
                }
                myReader.Close();
                myReader.Dispose();

                comboBox1.ItemsSource = drivers;
                comboBox2.ItemsSource = companies;
                comboBox3.ItemsSource = servicetype;
                comboBox4.ItemsSource = documenttype;

                if (Mode.MODE_CREATE.Equals(mode))
                {
                    datePicker1.DisplayDate = DateTime.Today;
                    datePicker1.SelectedDate = DateTime.Today;
                    textBox5.Text = "0.00";
                    textBox6.Text = "0.00";
                    textBox7.Text = "0.00";
                    textBox9.Text = "0";

                    DeleteTransaction.IsEnabled = false;
                    DeleteTransaction.Opacity = 0.4;
                    CopyTransaction.IsEnabled = false;
                    CopyTransaction.Opacity = 0.4;
                    PrintLastTransaction.IsEnabled = false;
                    PrintLastTransaction.Opacity = 0.4;
                    this.Title = "Add Transaction";
                    label16.Visibility = Visibility.Hidden;
                }
                else if (Mode.MODE_VIEWRO.Equals(mode))
                {
                    if (idx != null)
                    {
                        MakeFormRO(true);
                        TransactionDataHandler handler = new TransactionDataHandler();
                        handler.conn = conn;
                        if (!handler.LoadTransaction(idx.Value))
                        {
                            Config.ShowErrorMessage(string.Format("Failed to load the data associated to document number: {0}", idx.Value));
                            this.Close();
                        }
                        else
                        {
                            DOC_NUM = handler._DOC_NUM;
                            textBox1.Text = Convert.ToString(handler._DOC_NUM);
                            string driver = dct_drv_licen.SingleOrDefault(r => r.Value == handler._DRV_COD).Key;
                            textBox3.Text = driver;
                            comboBox1.Text = dct_drivers[driver];

                            string company = dct_companies.SingleOrDefault(r => r.Value == handler._CMP_COD).Key;
                            comboBox2.Text = company;
                            textBox4.Text = handler._Destination;
                            comboBox3.Text = handler._Serv_Name;
                            comboBox4.Text = handler._Doc_Type_Name;
                            datePicker1.SelectedDate = handler._Doc_Date;
                            textBox5.Text = string.Format("{0:0.00}", handler._Trans_Value);
                            textBox6.Text = string.Format("{0:0.00}", handler._Trans_Reduct);
                            textBox7.Text = string.Format("{0:0.00}", handler._Trans_Final);
                            textBox8.Text = handler._Voucher_Num;
                            textBox9.Text = Convert.ToString(handler._No_Pax);
                            textBox10.Text = handler._Comments;
                            if (!handler.isValid)
                            {
                                label16.Visibility = Visibility.Visible;
                                DeleteTransaction.Opacity = 0.4;
                                DeleteTransaction.IsEnabled = false;
                                button2.IsEnabled = false;
                            }
                            this.Title = "View Transaction";
                        }
                    }
                    else
                    {
                        datePicker1.DisplayDate = DateTime.Today;
                        datePicker1.SelectedDate = DateTime.Today;
                        textBox5.Text = "0.00";
                        textBox6.Text = "0.00";
                        textBox7.Text = "0.00";
                        textBox9.Text = "0";

                        DeleteTransaction.IsEnabled = false;
                        DeleteTransaction.Opacity = 0.4;
                        CopyTransaction.IsEnabled = false;
                        CopyTransaction.Opacity = 0.4;
                        PrintLastTransaction.IsEnabled = false;
                        PrintLastTransaction.Opacity = 0.4;
                        this.Title = "Add Transaction";
                        mode = Mode.MODE_CREATE;
                        label16.Visibility = Visibility.Hidden;
                    }

                }
                else if (Mode.MODE_UPDATE.Equals(mode))
                {
                    if (idx != null)
                    {
                        //MakeFormRO(true);
                        TransactionDataHandler handler = new TransactionDataHandler();
                        handler.conn = conn;
                        if (!handler.LoadTransaction(idx.Value))
                        {
                            Config.ShowErrorMessage(string.Format("Failed to load the data associated to document number: {0}", idx.Value));
                            this.Close();
                        }
                        else
                        {
                            DOC_NUM = handler._DOC_NUM;
                            textBox1.Text = Convert.ToString(handler._DOC_NUM);
                            string driver = dct_drv_licen.SingleOrDefault(r => r.Value == handler._DRV_COD).Key;
                            textBox3.Text = driver;
                            comboBox1.Text = dct_drivers[driver];

                            string company = dct_companies.SingleOrDefault(r => r.Value == handler._CMP_COD).Key;
                            comboBox2.Text = company;
                            textBox4.Text = handler._Destination;
                            comboBox3.Text = handler._Serv_Name;
                            comboBox4.Text = handler._Doc_Type_Name;
                            datePicker1.SelectedDate = handler._Doc_Date;
                            textBox5.Text = string.Format("{0:0.00}", handler._Trans_Value);
                            textBox6.Text = string.Format("{0:0.00}", handler._Trans_Reduct);
                            textBox7.Text = string.Format("{0:0.00}", handler._Trans_Final);
                            textBox8.Text = handler._Voucher_Num;
                            textBox9.Text = Convert.ToString(handler._No_Pax);
                            textBox10.Text = handler._Comments;
                            if (!handler.isValid)
                            {
                                label16.Visibility = Visibility.Visible;
                                DeleteTransaction.Opacity = 0.4;
                                DeleteTransaction.IsEnabled = false;
                                button2.IsEnabled = false;
                                button1.IsEnabled = false;
                                PrintLastTransaction.IsEnabled = false;
                                PrintLastTransaction.Opacity = 0.4;
                                CopyTransaction.IsEnabled = true;
                                CopyTransaction.Opacity = 0.4;
                                SaveTransaction.IsEnabled = false;
                                SaveTransaction.Opacity = 0.4;
                                CancelTransaction.IsEnabled = false;
                                CancelTransaction.Opacity = 0.4;
                            }
                            this.Title = "Edit Transaction";
                        }
                    }
                }
                else if (Mode.MODE_VOID.Equals(mode))
                {
                    if (idx != null)
                    {
                        MakeFormRO(true);
                        TransactionDataHandler handler = new TransactionDataHandler();
                        handler.conn = conn;
                        if (!handler.LoadTransaction(idx.Value))
                        {
                            Config.ShowErrorMessage(string.Format("Failed to load the data associated to document number: {0}", idx.Value));
                            this.Close();
                        }
                        else
                        {
                            DOC_NUM = handler._DOC_NUM;
                            textBox1.Text = Convert.ToString(handler._DOC_NUM);
                            string driver = dct_drv_licen.SingleOrDefault(r => r.Value == handler._DRV_COD).Key;
                            textBox3.Text = driver;
                            comboBox1.Text = dct_drivers[driver];

                            string company = dct_companies.SingleOrDefault(r => r.Value == handler._CMP_COD).Key;
                            comboBox2.Text = company;
                            textBox4.Text = handler._Destination;
                            comboBox3.Text = handler._Serv_Name;
                            comboBox4.Text = handler._Doc_Type_Name;
                            datePicker1.SelectedDate = handler._Doc_Date;
                            textBox5.Text = string.Format("{0:0.00}", handler._Trans_Value);
                            textBox6.Text = string.Format("{0:0.00}", handler._Trans_Reduct);
                            textBox7.Text = string.Format("{0:0.00}", handler._Trans_Final);
                            textBox8.Text = handler._Voucher_Num;
                            textBox9.Text = Convert.ToString(handler._No_Pax);
                            textBox10.Text = handler._Comments;
                            if (!handler.isValid)
                            {
                                label16.Visibility = Visibility.Visible;
                                DeleteTransaction.Opacity = 0.4;
                                DeleteTransaction.IsEnabled = false;
                                button2.IsEnabled = false;
                            }
                            this.Title = "Void Transaction";
                        }
                        VoidTrans();
                        this.Close();
                    }
                    else
                        this.Close();
                }
                else if (Mode.MODE_COPYC.Equals(mode))
                {
                    if (idx != null)
                    {
                        TransactionDataHandler handler = new TransactionDataHandler();
                        handler.conn = conn;
                        if (!handler.LoadTransaction(idx.Value))
                        {
                            Config.ShowErrorMessage(string.Format("Failed to load the data associated to document number: {0}", idx.Value));
                            this.Close();
                        }
                        else
                        {
                            int? dn = handler.GetNewDocNum();
                            if (dn != null)
                            {
                                DOC_NUM = handler._DOC_NUM;
                                textBox1.Text = Convert.ToString(dn.Value);
                                string driver = dct_drv_licen.SingleOrDefault(r => r.Value == handler._DRV_COD).Key;
                                textBox3.Text = driver;
                                comboBox1.Text = dct_drivers[driver];

                                string company = dct_companies.SingleOrDefault(r => r.Value == handler._CMP_COD).Key;
                                comboBox2.Text = company;
                                textBox4.Text = handler._Destination;
                                comboBox3.Text = handler._Serv_Name;
                                comboBox4.Text = handler._Doc_Type_Name;
                                datePicker1.SelectedDate = DateTime.Today;
                                datePicker1.DisplayDate = DateTime.Today;
                                textBox5.Text = string.Format("{0:0.00}", handler._Trans_Value);
                                textBox6.Text = string.Format("{0:0.00}", handler._Trans_Reduct);
                                textBox7.Text = string.Format("{0:0.00}", handler._Trans_Final);
                                //textBox8.Text = handler._Voucher_Num;
                                textBox9.Text = Convert.ToString(handler._No_Pax);
                                textBox10.Text = handler._Comments;
                                this.Title = "Copy Transaction";
                                CopyTransaction.IsEnabled = false;
                                CopyTransaction.Opacity = 0.4;
                                DeleteTransaction.IsEnabled = false;
                                DeleteTransaction.Opacity = 0.4;
                            }
                            else
                            {
                                Config.ShowErrorMessage(string.Format("Failed to load the data associated to document number: {0}", idx.Value));
                                this.Close();
                            }
                        }

                    }
                    else
                    {
                        datePicker1.DisplayDate = DateTime.Today;
                        datePicker1.SelectedDate = DateTime.Today;
                        textBox5.Text = "0.00";
                        textBox6.Text = "0.00";
                        textBox7.Text = "0.00";
                        textBox9.Text = "0";

                        DeleteTransaction.IsEnabled = false;
                        DeleteTransaction.Opacity = 0.4;
                        CopyTransaction.IsEnabled = false;
                        CopyTransaction.Opacity = 0.4;
                        PrintLastTransaction.IsEnabled = false;
                        PrintLastTransaction.Opacity = 0.4;
                        this.Title = "Add Transaction";
                        mode = Mode.MODE_CREATE;
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.FatalError();
            }
        }

        private void textBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string plate = textBox3.Text;
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
                        textBox3.Text = plate;
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }

        private void textBox5_TextChanged(object sender, TextChangedEventArgs e)
        {
            // document value
            try
            {
                decimal val = decimal.Zero;
                decimal red = decimal.Zero;
                if (decimal.TryParse(textBox5.Text, out val))
                {
                    if (decimal.TryParse(textBox6.Text, out red))
                    {
                        decimal final = val - red;
                        textBox7.Text = String.Format("{0:0.00}", final);
                    }
                    else
                    {
                        textBox7.Text = String.Format("{0:0.00}", val);
                    }
                }
                else
                    Config.ShowErrorMessage("Invalid amount value entered, please enter a valid amount value.");
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }

        private void textBox6_TextChanged(object sender, TextChangedEventArgs e)
        {
            // reduction
            try
            {
                decimal val = decimal.Zero;
                decimal red = decimal.Zero;
                if (decimal.TryParse(textBox5.Text, out val))
                {
                    if (decimal.TryParse(textBox6.Text, out red))
                    {
                        decimal final = val - red;
                        textBox7.Text = String.Format("{0:0.00}", final);
                    }
                    else
                    {
                        textBox7.Text = String.Format("{0:0.00}", val);
                    }
                }
                else
                    Config.ShowErrorMessage("Invalid amount value entered, please enter a valid amount value.");
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }

        private void CancelTransaction_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            try
            {
                SqlCeDataReader myReader = null;
                SqlCeCommand command = new SqlCeCommand("select UniqueValue from UniqueNumbers Where UniqueName='Receipt'", conn);
                myReader = command.ExecuteReader();
                if (myReader.Read())
                {
                    textBox1.Text = myReader["UniqueValue"].ToString();
                    textBox2.Text = DateTime.Today.ToShortDateString();
                }
                myReader.Close();
                myReader.Dispose();

                textBox3.Text = null;
                textBox4.Text = null;
                textBox5.Text = "0.00";
                textBox6.Text = "0.00";
                textBox7.Text = "0.00";
                textBox8.Text = null;
                textBox9.Text = "0";
                textBox10.Text = null;
                comboBox1.Text = null;
                comboBox2.Text = null;
                comboBox3.Text = null;
                comboBox4.Text = null;
                datePicker1.DisplayDate = DateTime.Today;
                datePicker1.SelectedDate = DateTime.Today;

                if (DOC_NUM != null)
                {
                    PrintLastTransaction.IsEnabled = true;
                    PrintLastTransaction.Opacity = 1;
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // save button
            if (SaveTrans())
                ResetForm();
            else
                Config.ShowErrorMessage("Transaction Validation failed, please re-view your data, failed to save the transaction.");
        }

        private void SaveTransaction_Click(object sender, RoutedEventArgs e)
        {
            // save button toolbar
            if (SaveTrans())
                ResetForm();
            else
                Config.ShowErrorMessage("Transaction Validation failed, please re-view your data, failed to save the transaction.");
        }

        private bool SaveTrans()
        {
            try
            {
                string license = textBox3.Text;
                string company = comboBox2.Text;
                if (dct_drv_licen.ContainsKey(license) &&
                   dct_companies.ContainsKey(company))
                {
                    TransactionDataHandler handler = new TransactionDataHandler();
                    handler.conn = conn;
                    handler._DOC_NUM = Convert.ToInt32(textBox1.Text);
                    handler._DRV_COD = dct_drv_licen[license];
                    handler._CMP_COD = dct_companies[company];
                    handler._Destination = textBox4.Text;
                    handler._Serv_Name = comboBox3.Text;
                    handler._Doc_Type_Name = comboBox4.Text;
                    handler._Doc_Date = datePicker1.SelectedDate.Value;
                    handler._Trans_Value = Convert.ToDecimal(textBox5.Text);
                    handler._Trans_Reduct = Convert.ToDecimal(textBox6.Text);
                    handler._Trans_Final = Convert.ToDecimal(textBox7.Text);
                    handler._Voucher_Num = textBox8.Text;
                    handler._No_Pax = Convert.ToInt32(textBox9.Text);
                    if (!string.IsNullOrEmpty(textBox10.Text))
                        handler._Comments = textBox10.Text;
                    if (handler.SaveTransaction())
                    {
                        DOC_NUM = handler._DOC_NUM;
                        return true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MakeFormRO(bool RO)
        {
            if (RO)
            {
                SaveTransaction.IsEnabled = false;
                SaveTransaction.Opacity = 0.4;
                CancelTransaction.IsEnabled = false;
                CancelTransaction.Opacity = 0.4;
                PrintLastTransaction.IsEnabled = true;
                PrintLastTransaction.Opacity = 1;
                DeleteTransaction.IsEnabled = true;
                DeleteTransaction.Opacity = 1;
                CopyTransaction.IsEnabled = true;
                CopyTransaction.Opacity = 1;

                textBox3.IsReadOnly = true;
                comboBox1.IsEnabled = false;
                comboBox2.IsEnabled = false;
                textBox4.IsReadOnly = true;
                comboBox3.IsEnabled = false;
                comboBox4.IsEnabled = false;
                datePicker1.IsEnabled = false;
                textBox5.IsReadOnly = true;
                textBox6.IsReadOnly = true;
                textBox8.IsReadOnly = true;
                textBox9.IsReadOnly = true;
                textBox10.IsReadOnly = true;
                button1.IsEnabled = false;
                button2.IsEnabled = true;
            }
            else
            {
                SaveTransaction.IsEnabled = true;
                SaveTransaction.Opacity = 1;
                CancelTransaction.IsEnabled = true;
                CancelTransaction.Opacity = 1;
                PrintLastTransaction.IsEnabled = false;
                PrintLastTransaction.Opacity = 0.4;
                DeleteTransaction.IsEnabled = false;
                DeleteTransaction.Opacity = 0.4;
                CopyTransaction.IsEnabled = false;
                CopyTransaction.Opacity = 0.4;

                textBox3.IsReadOnly = false;
                comboBox1.IsEnabled = true;
                comboBox2.IsEnabled = true;
                textBox4.IsReadOnly = false;
                comboBox3.IsEnabled = true;
                comboBox4.IsEnabled = true;
                datePicker1.IsEnabled = true;
                textBox5.IsReadOnly = false;
                textBox6.IsReadOnly = false;
                textBox8.IsReadOnly = false;
                textBox9.IsReadOnly = false;
                textBox10.IsReadOnly = false;
                button1.IsEnabled = true;
                button2.IsEnabled = false;
            }
        }

        private void CopyTransaction_Click(object sender, RoutedEventArgs e)
        {
            MakeFormRO(false);
            if (idx != null)
            {
                TransactionDataHandler handler = new TransactionDataHandler();
                handler.conn = conn;
                int? dn = handler.GetNewDocNum();
                if (dn != null)
                {
                    textBox1.Text = Convert.ToString(dn.Value);
                    this.Title = "Copy Transaction";
                    textBox8.Text = null;
                    datePicker1.SelectedDate = DateTime.Today;
                    datePicker1.DisplayDate = DateTime.Today;
                    label16.Visibility = Visibility.Hidden;
                }
            }
        }

        private void VoidTrans()
        {
            try
            {
                if (MessageBox.Show(string.Format("Are you sure that you want to void the transaction with Doc No: {0}", idx.Value), "ExpressTaxi", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (idx != null)
                    {
                        TransactionDataHandler handler = new TransactionDataHandler();
                        handler.conn = conn;
                        if (!handler.VoidTransaction(idx.Value))
                            Config.ShowErrorMessage(string.Format("Failed to void transaction associated to Doc No: {0}", idx.Value));
                        else
                        {
                            label16.Visibility = Visibility.Visible;
                            DeleteTransaction.Opacity = 0.4;
                            DeleteTransaction.IsEnabled = false;
                            button2.IsEnabled = false;
                        }
                    }
                    else
                        Config.ShowErrorMessage(string.Format("Failed to void transaction associated to Doc No: {0}", idx.Value));

                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.FatalError();
            }
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            // void
            VoidTrans();
        }

        private void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            VoidTrans();
        }

        private void PrintLastTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (DOC_NUM != null)
            {
                ViewReport dlg = new ViewReport();
                dlg.conn = conn;
                dlg.DocNum = DOC_NUM.Value;
                dlg.type = ReportType.REPORT_RECEIPT;
                dlg.ShowDialog();
            }

        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            // forward
            int DocNum;
            if (int.TryParse(textBox1.Text, out DocNum))
            {
                DocNum = DocNum + 1;
                TransactionDataHandler handler = new TransactionDataHandler();
                handler.conn = conn;
                if (!handler.LoadTransaction(DocNum))
                {
                    if (!mode.Equals(Mode.MODE_VIEWRO))
                    {
                        ResetForm();

                        datePicker1.DisplayDate = DateTime.Today;
                        datePicker1.SelectedDate = DateTime.Today;
                        textBox5.Text = "0.00";
                        textBox6.Text = "0.00";
                        textBox7.Text = "0.00";
                        textBox9.Text = "0";

                        button1.IsEnabled = true;
                        button2.IsEnabled = false;

                        SaveTransaction.IsEnabled = true;
                        SaveTransaction.Opacity = 1;
                        CancelTransaction.IsEnabled = true;
                        CancelTransaction.Opacity = 1;
                        DeleteTransaction.IsEnabled = false;
                        DeleteTransaction.Opacity = 0.4;
                        CopyTransaction.IsEnabled = false;
                        CopyTransaction.Opacity = 0.4;
                        PrintLastTransaction.IsEnabled = false;
                        PrintLastTransaction.Opacity = 0.4;
                        this.Title = "Add Transaction";
                        label16.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    idx = DocNum;
                    DOC_NUM = handler._DOC_NUM;
                    textBox1.Text = Convert.ToString(handler._DOC_NUM);
                    string driver = dct_drv_licen.SingleOrDefault(r => r.Value == handler._DRV_COD).Key;
                    textBox3.Text = driver;
                    comboBox1.Text = dct_drivers[driver];

                    button1.IsEnabled = true;
                    button2.IsEnabled = true;
                    CopyTransaction.Opacity = 1;
                    CopyTransaction.IsEnabled = true;
                    string company = dct_companies.SingleOrDefault(r => r.Value == handler._CMP_COD).Key;
                    comboBox2.Text = company;
                    textBox4.Text = handler._Destination;
                    comboBox3.Text = handler._Serv_Name;
                    comboBox4.Text = handler._Doc_Type_Name;
                    datePicker1.SelectedDate = handler._Doc_Date;
                    textBox5.Text = string.Format("{0:0.00}", handler._Trans_Value);
                    textBox6.Text = string.Format("{0:0.00}", handler._Trans_Reduct);
                    textBox7.Text = string.Format("{0:0.00}", handler._Trans_Final);
                    textBox8.Text = handler._Voucher_Num;
                    textBox9.Text = Convert.ToString(handler._No_Pax);
                    textBox10.Text = handler._Comments;
                    PrintLastTransaction.Opacity = 1;
                    PrintLastTransaction.IsEnabled = true;
                    this.Title = "Edit Transaction";
                    label16.Visibility = Visibility.Hidden;
                    if (!handler.isValid)
                    {
                        label16.Visibility = Visibility.Visible;
                        DeleteTransaction.Opacity = 0.4;
                        DeleteTransaction.IsEnabled = false;
                        button2.IsEnabled = false;
                        button1.IsEnabled = false;
                        PrintLastTransaction.IsEnabled = false;
                        PrintLastTransaction.Opacity = 0.4;
                        CopyTransaction.IsEnabled = true;
                        CopyTransaction.Opacity = 0.4;
                        SaveTransaction.IsEnabled = false;
                        SaveTransaction.Opacity = 0.4;
                        CancelTransaction.IsEnabled = false;
                        CancelTransaction.Opacity = 0.4;
                    }
                    else
                    {
                        label16.Visibility = Visibility.Hidden;
                        DeleteTransaction.Opacity = 1;
                        DeleteTransaction.IsEnabled = true;
                        button2.IsEnabled = true;
                        button1.IsEnabled = true;
                        PrintLastTransaction.IsEnabled = true;
                        PrintLastTransaction.Opacity = 1;
                        CopyTransaction.IsEnabled = true;
                        CopyTransaction.Opacity = 0.4;
                        SaveTransaction.IsEnabled = true;
                        SaveTransaction.Opacity = 1;
                        CancelTransaction.IsEnabled = true;
                        CancelTransaction.Opacity = 1;
                    }

                    if (mode.Equals(Mode.MODE_VIEWRO))
                    {
                        MakeFormRO(true);
                        this.Title = "View Transaction";
                        if (!handler.isValid)
                        {
                            button2.IsEnabled = false;
                            DeleteTransaction.IsEnabled = false;
                            DeleteTransaction.Opacity = 0.4;
                        }
                    }
                }
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            // back
            int DocNum;
            if (int.TryParse(textBox1.Text, out DocNum))
            {
                DocNum = DocNum - 1;

                if (DocNum > 0)
                {
                    idx = DocNum;
                    TransactionDataHandler handler = new TransactionDataHandler();
                    handler.conn = conn;
                    if (handler.LoadTransaction(DocNum))
                    {
                        DOC_NUM = handler._DOC_NUM;
                        textBox1.Text = Convert.ToString(handler._DOC_NUM);
                        string driver = dct_drv_licen.SingleOrDefault(r => r.Value == handler._DRV_COD).Key;
                        textBox3.Text = driver;
                        comboBox1.Text = dct_drivers[driver];

                        string company = dct_companies.SingleOrDefault(r => r.Value == handler._CMP_COD).Key;
                        comboBox2.Text = company;
                        textBox4.Text = handler._Destination;
                        comboBox3.Text = handler._Serv_Name;
                        comboBox4.Text = handler._Doc_Type_Name;
                        datePicker1.SelectedDate = handler._Doc_Date;
                        textBox5.Text = string.Format("{0:0.00}", handler._Trans_Value);
                        textBox6.Text = string.Format("{0:0.00}", handler._Trans_Reduct);
                        textBox7.Text = string.Format("{0:0.00}", handler._Trans_Final);
                        textBox8.Text = handler._Voucher_Num;
                        textBox9.Text = Convert.ToString(handler._No_Pax);
                        textBox10.Text = handler._Comments;

                        DeleteTransaction.Opacity = 1;
                        DeleteTransaction.IsEnabled = true;
                        button2.IsEnabled = true;
                        CopyTransaction.Opacity = 1;
                        CopyTransaction.IsEnabled = true;
                        PrintLastTransaction.Opacity = 1;
                        PrintLastTransaction.IsEnabled = true;
                        this.Title = "Edit Transaction";
                        label16.Visibility = Visibility.Hidden;
                        if (!handler.isValid)
                        {
                            label16.Visibility = Visibility.Visible;
                            DeleteTransaction.Opacity = 0.4;
                            DeleteTransaction.IsEnabled = false;
                            button2.IsEnabled = false;
                            CopyTransaction.Opacity = 1;
                            CopyTransaction.IsEnabled = true;
                            button1.IsEnabled = false;
                            PrintLastTransaction.IsEnabled = false;
                            PrintLastTransaction.Opacity = 0.4;
                            SaveTransaction.IsEnabled = false;
                            SaveTransaction.Opacity = 0.4;
                            CancelTransaction.IsEnabled = false;
                            CancelTransaction.Opacity = 0.4;
                        }

                        if (mode.Equals(Mode.MODE_VIEWRO))
                        {
                            MakeFormRO(true);
                            this.Title = "View Transaction";
                            if (!handler.isValid)
                            {
                                button2.IsEnabled = false;
                                DeleteTransaction.IsEnabled = false;
                                DeleteTransaction.Opacity = 0.4;
                            }
                        }
                    }
                }
            }
        }
    }
}
