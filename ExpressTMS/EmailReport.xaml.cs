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
using Microsoft.Reporting;
using Microsoft.Reporting.WinForms;
using System.Data.SqlServerCe;
using System.IO;

namespace ExpressTMS
{
    /// <summary>
    /// Interaction logic for EmailReport.xaml
    /// </summary>
    public partial class EmailReport : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SqlCeConnection conn { get; set; }
        private DateTime startPeriod;
        private DateTime endPeriod;
        private string DrvLicense;
        private string DrvName;
        private string HotelInfo;
        private decimal Deduction;

        public EmailReport()
        {
            InitializeComponent();
        }

        private bool ExportReport(ReportType type, string outname, string fmt)
        {
            try
            {
                string deviceInfo =
                      "<DeviceInfo>" +
                      " <OutputFormat>" + fmt + "</OutputFormat>" +
                      " <PageWidth>21cm</PageWidth>" +
                      " <PageHeight>25cm</PageHeight>" +
                      " <MarginTop>2cm</MarginTop>" +
                      " <MarginLeft>2cm</MarginLeft>" +
                      " <MarginRight>2cm</MarginRight>" +
                      " <MarginBottom>2cm</MarginBottom>" +
                      "</DeviceInfo>";
                Warning[] warnings;
                string[] streams;
                byte[] bytes;
                string reportType = fmt;
                string mimeType = "";
                string encoding = "";
                string fileNameExtension = "";
                LocalReport _reportViewer = new LocalReport();
                if (ReportType.REPORT_COMAPNY_LIST.Equals(type))
                {
                    #region COMPANY LIST REPORT CREATION LOGIC
                    using (ExpressTaxi ctx = new ExpressTaxi(conn))
                    {
                        var query = from c in ctx.Companies orderby c.CMP_COD select c;
                        if (query.Count() > 0)
                        {
                            ReportDataset dataset = new ReportDataset();
                            foreach (Company c in query)
                            {
                                dataset.Tables["Company"].Rows.Add(
                                       new object[]
                                           {
                                               (int)c.CMP_COD,
                                               (string)c.CMP_NAME,
                                               (string)c.CMP_ADDRESS,
                                               (string)c.VIL_NAME,
                                               (string)c.PAR_NAME,
                                               (string)c.COU_NAME,
                                               (string)c.CMP_PHONE
                                           }
                                );
                            }
                            dataset.AcceptChanges();
                            _reportViewer.ReportEmbeddedResource = "ExpressTMS.CompanyList.rdlc";

                            ReportParameter[] param = new ReportParameter[4];
                            param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                            param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                            param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                            param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                            _reportViewer.SetParameters(param);
                            ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["Company"]);
                            _reportViewer.DataSources.Clear();
                            _reportViewer.DataSources.Add(datasource);

                            bytes = _reportViewer.Render(
                               reportType,
                               deviceInfo,
                               out mimeType,
                               out encoding,
                               out fileNameExtension,
                               out streams,
                               out warnings);

                            FileStream fs = new FileStream(outname,
                               FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                            return true;
                        }
                    }
                    #endregion
                }
                else if (ReportType.REPORT_DRIVER_LIST.Equals(type))
                {
                    #region DRIVER LIST REPORT CREATION LOGIC
                    using (ExpressTaxi ctx = new ExpressTaxi(conn))
                    {
                        var query = from c in ctx.Drivers orderby c.DRV_COD select c;
                        if (query.Count() > 0)
                        {
                            ReportDataset dataset = new ReportDataset();
                            foreach (Driver c in query)
                            {
                                dataset.Tables["Driver"].Rows.Add(
                                       new object[]
                                           {
                                               (int)c.DRV_COD,
                                               (string)c.DRV_NAME,
                                               (string)c.DRV_ADDRESS,
                                               (string)c.VIL_NAME,
                                               (string)c.PAR_NAME,
                                               (string)c.COU_NAME,
                                               (string)c.DRV_PHONE,
                                               (string)c.DRV_LICENSE
                                           }
                                );
                            }
                            dataset.AcceptChanges();
                            _reportViewer.ReportEmbeddedResource = "ExpressTMS.DriverList.rdlc";

                            ReportParameter[] param = new ReportParameter[4];
                            param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                            param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                            param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                            param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                            _reportViewer.SetParameters(param);

                            ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["Driver"]);
                            _reportViewer.DataSources.Clear();
                            _reportViewer.DataSources.Add(datasource);

                            bytes = _reportViewer.Render(
                                reportType,
                                deviceInfo,
                                out mimeType,
                                out encoding,
                                out fileNameExtension,
                                out streams,
                                out warnings);

                            FileStream fs = new FileStream(outname,
                               FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                            return true;

                        }
                    }
                    #endregion
                }
                else if (ReportType.REPORT_SERVICES_CURRENT_DAY.Equals(type))
                {
                    #region SERVICES CURRENT DAY CREATION LOGIC
                    using (ExpressTaxi ctx = new ExpressTaxi(conn))
                    {
                        var query = from c in ctx.FinancialTransactions
                                    where
                                        c.TRANS_DATE >= startPeriod &&
                                        c.TRANS_DATE <= endPeriod && c.IsValid
                                    orderby c.TRANS_COD
                                    select c;
                        if (query.Count() > 0)
                        {
                            ReportDataset dataset = new ReportDataset();
                            foreach (FinancialTransaction c in query)
                            {
                                if (c.IsValid)
                                {
                                    dataset.Tables["ServicesCurrentDay"].Rows.Add(
                                       new object[]
                                           {
                                               (int)c.DOC_NUM,
                                               (string)c.Driver.DRV_LICENSE,
                                               (string)c.Driver.DRV_NAME,
                                               (string)c.Company.CMP_NAME,
                                               (string)c.TRANS_DESTINATION,
                                               (string)c.DOC_TYPE_NAME,
                                               (string)c.DOCUMENT_DATE.ToString("dd MMM yyyy"),
                                               (decimal)c.TRANS_VALUE,
                                               (decimal)c.TRANS_FINAL_VALUE
                                           }
                                    );
                                }

                            }
                            dataset.AcceptChanges();
                            _reportViewer.ReportEmbeddedResource = "ExpressTMS.ServicesCurrentDay.rdlc";

                            ReportParameter[] param = new ReportParameter[6];
                            param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                            param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                            param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                            param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                            param[4] = new ReportParameter("START_DATE", startPeriod.ToString("dd MMM yyyy"));
                            param[5] = new ReportParameter("END_DATE", endPeriod.ToString("dd MMM yyyy"));
                            _reportViewer.SetParameters(param);
                            ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["ServicesCurrentDay"]);
                            _reportViewer.DataSources.Clear();
                            _reportViewer.DataSources.Add(datasource);

                            bytes = _reportViewer.Render(
                                reportType,
                                deviceInfo,
                                out mimeType,
                                out encoding,
                                out fileNameExtension,
                                out streams,
                                out warnings);

                            FileStream fs = new FileStream(outname,
                               FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                            return true;
                        }
                    }
                    #endregion
                }
                else if (ReportType.REPORT_ACCUMULATED_BY_DRIVER.Equals(type))
                {
                    #region ACCUMULATED BY DRIVER LOGIC
                    using (ExpressTaxi ctx = new ExpressTaxi(conn))
                    {
                        decimal FinalVal = decimal.Zero;

                        var query = from c in ctx.FinancialTransactions
                                    where
                                        c.TRANS_DATE >= startPeriod &&
                                        c.TRANS_DATE <= endPeriod &&
                                        c.Driver.DRV_LICENSE == DrvLicense &&
                                        c.Driver.DRV_NAME == DrvName &&
                                        c.IsValid
                                    orderby c.TRANS_COD
                                    select c;
                        if (query.Count() > 0)
                        {
                            ReportDataset dataset = new ReportDataset();
                            ReportDataset tax = new ReportDataset();
                            ReportDataset DeductionSet = new ReportDataset();

                            foreach (FinancialTransaction c in query)
                            {
                                if (c.IsValid)
                                {
                                    dataset.Tables["AccumulatedByDriver"].Rows.Add(
                                       new object[]
                                           {
                                               (int)c.DOC_NUM,
                                               (string)c.DOCUMENT_DATE.ToString("dd MMM yyyy"),
                                               (string)c.Driver.DRV_LICENSE,
                                               (string)c.Driver.DRV_NAME,
                                               (string)c.Company.CMP_NAME,
                                               (string)c.TRANS_DESTINATION,
                                               (decimal)c.TRANS_VALUE,
                                               (decimal)c.TRANS_REDUCTION,
                                               (decimal)c.TRANS_FINAL_VALUE,
                                               (string)c.TRANS_COMMENTS,
                                               (string)c.VOUCHER_NUM
                                           }
                                    );
                                    FinalVal = FinalVal + c.TRANS_FINAL_VALUE;
                                }

                            }

                            var query2 = from t in ctx.AppliedTaxes select t;
                            if (query2.Count() > 0)
                            {
                                int n = 0;
                                foreach (AppliedTax t in query2)
                                {
                                    tax.Tables["Tax"].Rows.Add(new object[]{
                                            (int)n,
                                            (string)t.TaxDescription,
                                            (decimal)(t.TaxAmount * FinalVal) / 100
                                        });
                                    n = n + 1;
                                }
                            }

                            DeductionSet.Tables["Deduction"].Rows.Add(new object[] { (decimal)Deduction });

                            dataset.AcceptChanges();
                            tax.AcceptChanges();
                            DeductionSet.AcceptChanges();
                            _reportViewer.ReportEmbeddedResource = "ExpressTMS.AccumulatedByDriver.rdlc";

                            ReportParameter[] param = new ReportParameter[6];
                            param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                            param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                            param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                            param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                            param[4] = new ReportParameter("START_DATE", startPeriod.ToString("dd MMM yyyy"));
                            param[5] = new ReportParameter("END_DATE", endPeriod.ToString("dd MMM yyyy"));
                            _reportViewer.SetParameters(param);
                            ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["AccumulatedByDriver"]);
                            ReportDataSource taxsource = new ReportDataSource("DataSet2", tax.Tables["Tax"]);
                            ReportDataSource deductionsource = new ReportDataSource("DataSet3", DeductionSet.Tables["Deduction"]);
                            _reportViewer.DataSources.Clear();
                            _reportViewer.DataSources.Add(datasource);
                            _reportViewer.DataSources.Add(taxsource);
                            _reportViewer.DataSources.Add(deductionsource);

                            bytes = _reportViewer.Render(
                                reportType,
                                deviceInfo,
                                out mimeType,
                                out encoding,
                                out fileNameExtension,
                                out streams,
                                out warnings);

                            FileStream fs = new FileStream(outname,
                               FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                            return true;
                        }
                    }
                    #endregion

                }
                else if (ReportType.REPORT_ACCUMULATED_PAID.Equals(type))
                {
                    #region ACCUMULATED PAID LOGIC
                    using (ExpressTaxi ctx = new ExpressTaxi(conn))
                    {
                        var query = from c in ctx.FinancialTransactions
                                    where
                                        c.TRANS_DATE >= startPeriod &&
                                        c.TRANS_DATE <= endPeriod && c.IsValid
                                    select c;
                        if (query.Count() > 0)
                        {
                            List<int> txNum = new List<int>();
                            var query2 = from d in query select new { d.DRV_COD };
                            foreach (var d in query2)
                            {
                                if (!txNum.Contains(d.DRV_COD))
                                    txNum.Add(d.DRV_COD);
                            }
                            ReportDataset dataset = new ReportDataset();
                            int n = 0;

                            foreach (var d in txNum)
                            {
                                var query3 = from tx in query where tx.DRV_COD.Equals(d) select tx;
                                FinancialTransaction c = query3.FirstOrDefault();
                                if (c != null)
                                {
                                    decimal TransValue = decimal.Zero;
                                    decimal TransReduction = decimal.Zero;
                                    decimal TransFinalValue = decimal.Zero;
                                    foreach (FinancialTransaction tx in query3)
                                    {
                                        TransValue = TransValue + tx.TRANS_VALUE;
                                        TransReduction = TransReduction + tx.TRANS_REDUCTION;
                                    }
                                    TransFinalValue = TransValue - TransReduction;

                                    dataset.Tables["AccumulatedPaid"].Rows.Add(
                                           new object[]
                                           {
                                               (int)n,
                                               (string)c.Driver.DRV_LICENSE,
                                               (string)c.Driver.DRV_NAME,
                                               (decimal)TransValue,
                                               (decimal)TransReduction,
                                               (decimal)TransFinalValue
                                           }
                                    );
                                    n = n + 1;
                                }
                            }

                            dataset.AcceptChanges();

                            _reportViewer.ReportEmbeddedResource = "ExpressTMS.AccumulatedPaid.rdlc";

                            ReportParameter[] param = new ReportParameter[6];
                            param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                            param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                            param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                            param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                            param[4] = new ReportParameter("START_DATE", startPeriod.ToString("dd MMM yyyy"));
                            param[5] = new ReportParameter("END_DATE", endPeriod.ToString("dd MMM yyyy"));
                            _reportViewer.SetParameters(param);
                            ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["AccumulatedPaid"]);
                            _reportViewer.DataSources.Clear();
                            _reportViewer.DataSources.Add(datasource);

                            bytes = _reportViewer.Render(
                                reportType,
                                deviceInfo,
                                out mimeType,
                                out encoding,
                                out fileNameExtension,
                                out streams,
                                out warnings);

                            FileStream fs = new FileStream(outname,
                               FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                            return true;
                        }
                    }
                    #endregion
                }
                else if (ReportType.REPORT_HOTEL_INFORMATION.Equals(type))
                {
                    #region SERVICES CURRENT DAY CREATION LOGIC
                    using (ExpressTaxi ctx = new ExpressTaxi(conn))
                    {
                        var query = from c in ctx.FinancialTransactions
                                    where
                                        c.TRANS_DATE >= startPeriod &&
                                        c.TRANS_DATE <= endPeriod &&
                                        c.Company.CMP_NAME == HotelInfo &&
                                        c.IsValid
                                    orderby c.TRANS_COD
                                    select c;
                        if (query.Count() > 0)
                        {
                            ReportDataset dataset = new ReportDataset();
                            foreach (FinancialTransaction c in query)
                            {
                                if (c.IsValid)
                                {
                                    dataset.Tables["HotelInformation"].Rows.Add(
                                       new object[]
                                           {
                                               (int)c.DOC_NUM,
                                               (string)c.Driver.DRV_LICENSE,
                                               (string)c.Driver.DRV_NAME,
                                               (string)c.Company.CMP_NAME,
                                               (string)c.TRANS_DESTINATION,
                                               (string)c.VOUCHER_NUM,
                                               (decimal)c.TRANS_FINAL_VALUE
                                           }
                                    );
                                }

                            }
                            dataset.AcceptChanges();
                            _reportViewer.ReportEmbeddedResource = "ExpressTMS.HotelInformation.rdlc";

                            ReportParameter[] param = new ReportParameter[7];
                            param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                            param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                            param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                            param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                            param[4] = new ReportParameter("START_DATE", startPeriod.ToString("dd MMM yyyy"));
                            param[5] = new ReportParameter("END_DATE", endPeriod.ToString("dd MMM yyyy"));
                            param[6] = new ReportParameter("AGENT", HotelInfo);
                            _reportViewer.SetParameters(param);
                            ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["HotelInformation"]);
                            _reportViewer.DataSources.Clear();
                            _reportViewer.DataSources.Add(datasource);

                            bytes = _reportViewer.Render(
                                reportType,
                                deviceInfo,
                                out mimeType,
                                out encoding,
                                out fileNameExtension,
                                out streams,
                                out warnings);

                            FileStream fs = new FileStream(outname,
                               FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                            return true;
                        }
                    }
                    #endregion
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }

        private void DriverListRpt_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            NotificationWindow dlg = new NotificationWindow();
            dlg.ShowModal("Please wait as the report is exported and your default mail client started.");
            DispatcherHelper.DoEvents();
            string ext = ".doc";
            string fmt = "WORD";
            if (("Microsoft Word (*.doc)").Equals(comboBox1.Text))
            {
                ext = ".doc";
                fmt = "WORD";
            }
            else if (("Microsoft Excel (*.xls)").Equals(comboBox1.Text))
            {
                ext = ".xls";
                fmt = "EXCEL";
            }
            else if (("Adobe PDF (*.pdf)").Equals(comboBox1.Text))
            {
                ext = ".pdf";
                fmt = "PDF";
            }
            string PathName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string FileName = PathName + "\\reports\\" + string.Format("DriverList-{0:ddMMyyyy-HHmm}", DateTime.Now) + ext;

            if (ExportReport(ReportType.REPORT_DRIVER_LIST, FileName, fmt))
            {
                MAPI mapi = new MAPI();
                mapi.AddAttachment(FileName);
                mapi.SendMailPopup(Config.Subject, Config.BodyText);
            }
            else
                MessageBox.Show("Failed to export and email the report.", "Express Taxi", MessageBoxButton.OK, MessageBoxImage.Error);
            dlg.CloseModal();
            e.Handled = true;
        }

        private void hyperlink1_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            NotificationWindow dlg = new NotificationWindow();
            dlg.ShowModal("Please wait as the report is exported and your default mail client started.");
            DispatcherHelper.DoEvents();
            string ext = ".doc";
            string fmt = "WORD";
            if (("Microsoft Word (*.doc)").Equals(comboBox1.Text))
            {
                ext = ".doc";
                fmt = "WORD";
            }
            else if (("Microsoft Excel (*.xls)").Equals(comboBox1.Text))
            {
                ext = ".xls";
                fmt = "EXCEL";
            }
            else if (("Adobe PDF (*.pdf)").Equals(comboBox1.Text))
            {
                ext = ".pdf";
                fmt = "PDF";
            }
            string PathName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string FileName = PathName + "\\reports\\" + string.Format("CompanyList-{0:ddMMyyyy-HHmm}", DateTime.Now) + ext;
            if (ExportReport(ReportType.REPORT_COMAPNY_LIST, FileName, fmt))
            {
                MAPI mapi = new MAPI();
                mapi.AddAttachment(FileName);
                mapi.SendMailPopup(Config.Subject, Config.BodyText);
            }
            else
                MessageBox.Show("Failed to export and email the report.", "Express Taxi", MessageBoxButton.OK, MessageBoxImage.Error);
            dlg.CloseModal();
            e.Handled = true;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            startPeriod = DateTime.Today;
            endPeriod = DateTime.Today;
        }

        private void hyperlink2_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            DatePeriodPicker pck = new DatePeriodPicker();
            pck.Owner = this;
            pck.ShowDialog();
            if (pck.PeriodSelected)
            {
                startPeriod = pck.startPeriod;
                endPeriod = pck.endPeriod;

                NotificationWindow dlg = new NotificationWindow();
                dlg.ShowModal("Please wait as the report is exported and your default mail client started.");
                DispatcherHelper.DoEvents();
                string ext = ".doc";
                string fmt = "WORD";
                if (("Microsoft Word (*.doc)").Equals(comboBox1.Text))
                {
                    ext = ".doc";
                    fmt = "WORD";
                }
                else if (("Microsoft Excel (*.xls)").Equals(comboBox1.Text))
                {
                    ext = ".xls";
                    fmt = "EXCEL";
                }
                else if (("Adobe PDF (*.pdf)").Equals(comboBox1.Text))
                {
                    ext = ".pdf";
                    fmt = "PDF";
                }
                string PathName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string FileName = PathName + "\\reports\\" + string.Format("Services-{0:ddMMyyyy-HHmm}", DateTime.Now) + ext;

                if (ExportReport(ReportType.REPORT_SERVICES_CURRENT_DAY, FileName, fmt))
                {
                    MAPI mapi = new MAPI();
                    mapi.AddAttachment(FileName);
                    mapi.SendMailPopup(Config.Subject, Config.BodyText);
                }
                else
                    MessageBox.Show("Failed to export and email the report.", "Express Taxi", MessageBoxButton.OK, MessageBoxImage.Error);
                dlg.CloseModal();

            }
            e.Handled = true;
        }

        private void hyperlink3_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            DriverFilterWnd pck = new DriverFilterWnd();
            pck.Owner = this;
            pck.conn = conn;
            pck.ShowDialog();
            if (pck.PeriodSelected)
            {
                startPeriod = pck.startPeriod;
                endPeriod = pck.endPeriod;
                DrvLicense = pck.DrvLicense;
                DrvName = pck.DrvName;
                Deduction = pck.Deduction;

                NotificationWindow dlg = new NotificationWindow();
                dlg.ShowModal("Please wait as the report is exported and your default mail client started.");
                DispatcherHelper.DoEvents();
                string ext = ".doc";
                string fmt = "WORD";
                if (("Microsoft Word (*.doc)").Equals(comboBox1.Text))
                {
                    ext = ".doc";
                    fmt = "WORD";
                }
                else if (("Microsoft Excel (*.xls)").Equals(comboBox1.Text))
                {
                    ext = ".xls";
                    fmt = "EXCEL";
                }
                else if (("Adobe PDF (*.pdf)").Equals(comboBox1.Text))
                {
                    ext = ".pdf";
                    fmt = "PDF";
                }
                string PathName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string FileName = PathName + "\\reports\\" + string.Format("AccByDriver-{0:ddMMyyyy-HHmm}", DateTime.Now) + ext;

                if (ExportReport(ReportType.REPORT_ACCUMULATED_BY_DRIVER, FileName, fmt))
                {
                    MAPI mapi = new MAPI();
                    mapi.AddAttachment(FileName);
                    mapi.SendMailPopup(Config.Subject, Config.BodyText);
                }
                else
                    MessageBox.Show("Failed to export and email the report.", "Express Taxi", MessageBoxButton.OK, MessageBoxImage.Error);
                dlg.CloseModal();

            }
            e.Handled = true;
        }

        private void hyperlink4_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            DatePeriodPicker pck = new DatePeriodPicker();
            pck.Owner = this;
            pck.ShowDialog();
            if (pck.PeriodSelected)
            {
                startPeriod = pck.startPeriod;
                endPeriod = pck.endPeriod;

                NotificationWindow dlg = new NotificationWindow();
                dlg.ShowModal("Please wait as the report is exported and your default mail client started.");
                DispatcherHelper.DoEvents();
                string ext = ".doc";
                string fmt = "WORD";
                if (("Microsoft Word (*.doc)").Equals(comboBox1.Text))
                {
                    ext = ".doc";
                    fmt = "WORD";
                }
                else if (("Microsoft Excel (*.xls)").Equals(comboBox1.Text))
                {
                    ext = ".xls";
                    fmt = "EXCEL";
                }
                else if (("Adobe PDF (*.pdf)").Equals(comboBox1.Text))
                {
                    ext = ".pdf";
                    fmt = "PDF";
                }
                string PathName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string FileName = PathName + "\\reports\\" + string.Format("AccumulatedPaid-{0:ddMMyyyy-HHmm}", DateTime.Now) + ext;

                if (ExportReport(ReportType.REPORT_ACCUMULATED_PAID, FileName, fmt))
                {
                    MAPI mapi = new MAPI();
                    mapi.AddAttachment(FileName);
                    mapi.SendMailPopup(Config.Subject, Config.BodyText);
                }
                else
                    MessageBox.Show("Failed to export and email the report.", "Express Taxi", MessageBoxButton.OK, MessageBoxImage.Error);
                dlg.CloseModal();

            }
            e.Handled = true;
        }

        private void hyperlink5_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            CompanyFilter pck = new CompanyFilter();
            pck.Owner = this;
            pck.conn = conn;
            pck.ShowDialog();
            if (pck.PeriodSelected)
            {
                startPeriod = pck.startPeriod;
                endPeriod = pck.endPeriod;
                HotelInfo = pck.CmpName;
                NotificationWindow dlg = new NotificationWindow();
                dlg.ShowModal("Please wait as the report is exported and your default mail client started.");
                DispatcherHelper.DoEvents();
                string ext = ".doc";
                string fmt = "WORD";
                if (("Microsoft Word (*.doc)").Equals(comboBox1.Text))
                {
                    ext = ".doc";
                    fmt = "WORD";
                }
                else if (("Microsoft Excel (*.xls)").Equals(comboBox1.Text))
                {
                    ext = ".xls";
                    fmt = "EXCEL";
                }
                else if (("Adobe PDF (*.pdf)").Equals(comboBox1.Text))
                {
                    ext = ".pdf";
                    fmt = "PDF";
                }
                string PathName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string FileName = PathName + "\\reports\\" + string.Format("HotelInfo-{0:ddMMyyyy-HHmm}", DateTime.Now) + ext;

                if (ExportReport(ReportType.REPORT_HOTEL_INFORMATION, FileName, fmt))
                {
                    MAPI mapi = new MAPI();
                    mapi.AddAttachment(FileName);
                    mapi.SendMailPopup(Config.Subject, Config.BodyText);
                }
                else
                    MessageBox.Show("Failed to export and email the report.", "Express Taxi", MessageBoxButton.OK, MessageBoxImage.Error);
                dlg.CloseModal();

            }
            e.Handled = true;
        }

    }
}
