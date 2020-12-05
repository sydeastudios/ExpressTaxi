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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fluent;
using System.Data.SqlServerCe;
using Microsoft.Win32;
using System.Xml;

namespace ExpressTMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private NotificationWindow not = new NotificationWindow();
        public SqlCeConnection conn;
        private MappingCache cache = new MappingCache();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey regKeyAppRoot = Registry.CurrentUser.CreateSubKey(@"Software\Sydea Studios Antigua\ExpressTaxi");
                string EulaAccepted = (string)regKeyAppRoot.GetValue("ExpessTaxiEula");
                cache.LoadMappingFile();
                if (string.IsNullOrEmpty(EulaAccepted) || EulaAccepted != "YES")
                {
                    EULA dlg = new EULA();
                    dlg.ShowDialog();
                    if (dlg.Accepted)
                    {
                        regKeyAppRoot.SetValue("ExpessTaxiEula", "YES");
                    }
                    else
                    {
                        Application.Current.Shutdown();
                        return;
                    }
                }
                regKeyAppRoot.Close();
                Config.LoadCfg();
                if (!Config.LoadAppRegInfo())
                {
                    Config.ShowErrorMessage("Failed to load license file.");
                    Application.Current.Shutdown();
                    return;
                }
                if (!Config.IsRegistered)
                {
                    string edt = (string)ValidateSerialKey.HexDecode((string)cache.ExpDateTime);
                    string Valid = (string)ValidateSerialKey.HexDecode((string)cache.ExpTag);
                    DateTime dt = DateTime.Parse(edt);
                    DateTime td = DateTime.Today;
                    TimeSpan ts = td - dt;
                    if (!string.IsNullOrEmpty(Valid))
                    {
                        if (ts.TotalDays > 30)
                        {
                            //Config.ShowErrorMessage("Error this trial version of ExpressTaxi has expired. Please register the application.");
                            TrialExpired texp = new TrialExpired();
                            texp.Owner = this;
                            texp.ShowDialog();
                            Valid = (string)ValidateSerialKey.HexEncode("Expired");
                            cache.ExpTag = (string)Valid;
                            cache.SaveMapping();
                            Application.Current.Shutdown();
                            return;
                        }
                        else if (!Valid.ToUpper().Equals(("Valid").ToUpper()))
                        {
                            //Config.ShowErrorMessage("Error this trial version of ExpressTaxi has expired. Please register the application.");
                            TrialExpired texp = new TrialExpired();
                            texp.Owner = this;
                            texp.ShowDialog();
                            Application.Current.Shutdown();
                            return;
                        }
                        else
                            this.Title = "Express Taxi (SP4) Trial Version";
                    }
                    else
                    {
                        Config.ShowInfoMessage("Installation completion is in progress, you will need to restart ExpressTaxi");
                        Application.Current.Shutdown();
                        return;
                    }
                }
                conn = new SqlCeConnection(string.Format("Data Source = {0};", Config.sdfFile));
                conn.Open();
            }
            catch (SqlCeException ce)
            {
                Config.ShowErrorMessage("Failed to establish a connection to the active database.");
                log.Error(ce);
                Application.Current.Shutdown();
            }
            catch (System.Exception ex)
            {
                Config.ShowErrorMessage("An unexpected Error occurred, cannot start ExpressTaxi.");
                log.Error(ex);
                Application.Current.Shutdown();
            }
        }

        private void RibbonWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                not.Close();
                conn.Close();
                conn.Dispose();
                Application.Current.Shutdown();
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }

        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Close();
                //conn.Dispose();
                if (!not.isShown)
                    not.ShowWindow("Please wait as Express Taxi Backup the active database.");
                if (!BackupRestore.BackupDatabase())
                    Config.ShowErrorMessage("Database backup has failed, please retry at another time.");
                conn.Open();
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Close();
                RestoreDB wnd = new RestoreDB();
                wnd.Owner = this;
                wnd.ShowDialog();
                conn.Open();
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
        }

        private void AddCompany_Click(object sender, RoutedEventArgs e)
        {
            AddNewCompany dlg = new AddNewCompany();
            dlg.conn = conn;
            dlg.mode = Mode.MODE_CREATE;
            dlg.ShowDialog();
        }

        private void UpdateCompany_Click(object sender, RoutedEventArgs e)
        {
            CompnaySearch dlg = new CompnaySearch();
            dlg.conn = conn;
            dlg.ShowDialog();
            if (dlg.idx != null)
            {
                AddNewCompany dlg2 = new AddNewCompany();
                dlg2.conn = conn;
                dlg2.mode = Mode.MODE_UPDATE;
                dlg2.idx = dlg.idx.Value;
                dlg2.ShowDialog();
            }
        }

        private void CompanyReport_Click(object sender, RoutedEventArgs e)
        {
            ViewReport dlg = new ViewReport();
            dlg.type = ReportType.REPORT_COMAPNY_LIST;
            dlg.conn = conn;
            dlg.ShowDialog();
        }

        private void AddNewDriver_Click(object sender, RoutedEventArgs e)
        {
            AddNewDriver dlg = new AddNewDriver();
            dlg.conn = conn;
            dlg.mode = Mode.MODE_CREATE;
            dlg.ShowDialog();
        }

        private void EditDriver_Click(object sender, RoutedEventArgs e)
        {
            DriverSearch dlg = new DriverSearch();
            dlg.conn = conn;
            dlg.ShowDialog();
            if (dlg.idx != null)
            {
                AddNewDriver dlg2 = new AddNewDriver();
                dlg2.conn = conn;
                dlg2.mode = Mode.MODE_UPDATE;
                dlg2.idx = dlg.idx.Value;
                dlg2.ShowDialog();
            }
        }

        private void DriverReport_Click(object sender, RoutedEventArgs e)
        {
            ViewReport dlg = new ViewReport();
            dlg.type = ReportType.REPORT_DRIVER_LIST;
            dlg.conn = conn;
            dlg.ShowDialog();
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            AddTransaction dlg = new AddTransaction();
            dlg.mode = Mode.MODE_CREATE;
            dlg.conn = conn;
            dlg.ShowDialog();
        }

        private void ViewTransaction_Click(object sender, RoutedEventArgs e)
        {
            TransactionSearch dlg = new TransactionSearch();
            dlg.conn = conn;
            dlg.ShowDialog();
            if (dlg.idx != null)
            {
                AddTransaction dlg2 = new AddTransaction();
                dlg2.mode = Mode.MODE_VIEWRO;
                dlg2.idx = dlg.idx;
                dlg2.conn = conn;
                dlg2.ShowDialog();
            }
        }

        private void CopyTransaction_Click(object sender, RoutedEventArgs e)
        {
            TransactionSearch dlg = new TransactionSearch();
            dlg.conn = conn;
            dlg.ShowDialog();
            if (dlg.idx != null)
            {
                AddTransaction dlg2 = new AddTransaction();
                dlg2.mode = Mode.MODE_COPYC;
                dlg2.idx = dlg.idx;
                dlg2.conn = conn;
                dlg2.ShowDialog();
            }
        }

        private void VoidTransaction_Click(object sender, RoutedEventArgs e)
        {
            TransactionSearch dlg = new TransactionSearch();
            dlg.conn = conn;
            dlg.ShowDialog();
            if (dlg.idx != null)
            {
                AddTransaction dlg2 = new AddTransaction();
                dlg2.mode = Mode.MODE_VOID;
                dlg2.idx = dlg.idx;
                dlg2.conn = conn;
                dlg2.ShowDialog();
            }
        }

        private void ModifiedParameters_Click(object sender, RoutedEventArgs e)
        {
            ModifyParameters dlg = new ModifyParameters();
            dlg.conn = conn;
            dlg.ShowDialog();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About dlg = new About();
            dlg.ShowDialog();
        }

        private void EmailReports_Click(object sender, RoutedEventArgs e)
        {
            EmailReport dlg = new EmailReport();
            dlg.conn = conn;
            dlg.ShowDialog();
        }

        private void ServicesCurrentDay_Click(object sender, RoutedEventArgs e)
        {
            DatePeriodPicker dlg2 = new DatePeriodPicker();
            dlg2.Owner = this;
            dlg2.ShowDialog();
            if (dlg2.PeriodSelected)
            {
                ViewReport dlg = new ViewReport();
                dlg.type = ReportType.REPORT_SERVICES_CURRENT_DAY;
                dlg.conn = conn;
                dlg.startPeriod = dlg2.startPeriod;
                dlg.endPeriod = dlg2.endPeriod;
                dlg.ShowDialog();
            }
        }

        private void DriverAccumulated_Click(object sender, RoutedEventArgs e)
        {
            DriverFilterWnd wnd = new DriverFilterWnd();
            wnd.Owner = this;
            wnd.conn = conn;
            wnd.ShowDialog();
            if (wnd.PeriodSelected)
            {
                ViewReport dlg = new ViewReport();
                dlg.type = ReportType.REPORT_ACCUMULATED_BY_DRIVER;
                dlg.conn = conn;
                dlg.startPeriod = wnd.startPeriod;
                dlg.endPeriod = wnd.endPeriod;
                dlg.DrvLicense = wnd.DrvLicense;
                dlg.DrvName = wnd.DrvName;
                dlg.Deduction = wnd.Deduction;
                dlg.ShowDialog();
            }
        }

        private void AccumulatedPaid_Click(object sender, RoutedEventArgs e)
        {
            DatePeriodPicker dlg2 = new DatePeriodPicker();
            dlg2.Owner = this;
            dlg2.ShowDialog();
            if (dlg2.PeriodSelected)
            {
                ViewReport dlg = new ViewReport();
                dlg.type = ReportType.REPORT_ACCUMULATED_PAID;
                dlg.conn = conn;
                dlg.startPeriod = dlg2.startPeriod;
                dlg.endPeriod = dlg2.endPeriod;
                dlg.ShowDialog();
            }
        }

        private void ServicesReport_Click(object sender, RoutedEventArgs e)
        {
            DatePeriodPicker dlg2 = new DatePeriodPicker();
            dlg2.Owner = this;
            dlg2.ShowDialog();
            if (dlg2.PeriodSelected)
            {
                ViewReport dlg = new ViewReport();
                dlg.type = ReportType.REPORT_SERVICES;
                dlg.conn = conn;
                dlg.startPeriod = dlg2.startPeriod;
                dlg.endPeriod = dlg2.endPeriod;
                dlg.ShowDialog();
            }
        }

        private void Hotel_Click(object sender, RoutedEventArgs e)
        {
            CompanyFilter dlg2 = new CompanyFilter();
            dlg2.Owner = this;
            dlg2.conn = conn;
            dlg2.ShowDialog();
            if (dlg2.PeriodSelected)
            {
                ViewReport dlg = new ViewReport();
                dlg.type = ReportType.REPORT_HOTEL_INFORMATION;
                dlg.HotelInfo = dlg2.CmpName;
                dlg.conn = conn;
                dlg.startPeriod = dlg2.startPeriod;
                dlg.endPeriod = dlg2.endPeriod;
                dlg.ShowDialog();
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings dlg = new Settings();
            dlg.Owner = this;
            dlg.conn = conn;
            dlg.ShowDialog();
        }

        private void Lookup_Click(object sender, RoutedEventArgs e)
        {
            Lookup dlg = new Lookup();
            dlg.Owner = this;
            dlg.conn = conn;
            dlg.ShowDialog();
        }

        private void Unlock_Click(object sender, RoutedEventArgs e)
        {
            EnterKey key = new EnterKey();
            key.Owner = this;
            key.ShowDialog();
        }

        private void EditTrans_Click(object sender, RoutedEventArgs e)
        {
            TransactionSearch dlg = new TransactionSearch();
            dlg.conn = conn;
            dlg.ShowDialog();
            if (dlg.idx != null)
            {
                AddTransaction dlg2 = new AddTransaction();
                dlg2.mode = Mode.MODE_UPDATE;
                dlg2.idx = dlg.idx;
                dlg2.conn = conn;
                dlg2.ShowDialog();
            }
        }
    }

    internal class MappingCache
    {
        internal string ExpDateTime { get; set; }
        internal string ExpTag { get; set; }

        internal bool LoadMappingFile()
        {
            try
            {
                if (System.IO.File.Exists(@"C:\STMSCache\CacheMapping.dat"))
                {
                    XmlReaderSettings setting = new XmlReaderSettings();
                    setting.IgnoreWhitespace = true;
                    using (XmlReader reader = XmlReader.Create(@"C:\STMSCache\CacheMapping.dat", setting))
                    {
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {
                                if ("ExpTMS".Equals(reader.Name))
                                {
                                    if (reader.Read())
                                    {
                                        ExpDateTime = reader.Value.ToString();
                                    }
                                }
                                if ("SlipLic.ET".Equals(reader.Name))
                                {
                                    if (reader.Read())
                                    {
                                        ExpTag = reader.Value.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
            }
            return false;
        }

        internal void SaveMapping()
        {
            try
            {
                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Indent = true;
                setting.IndentChars = " ";
                setting.NewLineOnAttributes = true;
                using (XmlWriter write = XmlWriter.Create(@"C:\STMSCache\CacheMapping.dat", setting))
                {
                    string edt = ExpDateTime;
                    string Valid = ExpTag;
                    write.WriteStartElement("Mapping");
                    write.WriteElementString("ExpTMS", edt);
                    write.WriteElementString("SlipLic.ET", Valid);
                    write.WriteEndElement();
                }
            }
            catch (System.Exception)
            {
            }
        }
    }
}
