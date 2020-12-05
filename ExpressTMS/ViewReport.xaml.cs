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
using Microsoft.Reporting;
using Microsoft.Reporting.WinForms;

namespace ExpressTMS
{
    /// <summary>
    /// Interaction logic for ViewReport.xaml
    /// </summary>
    public partial class ViewReport : Window
    {
        private bool _isReportViewerLoaded;
        public ReportType type { get; set; }
        public SqlCeConnection conn { get; set; }
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DateTime startPeriod { get; set; }
        public DateTime endPeriod { get; set; }
        public string DrvLicense { get; set; }
        public string DrvName { get; set; }
        public int DocNum { get; set; }
        public string HotelInfo { get; set; }
        public decimal Deduction { get; set; }

        public ViewReport()
        {
            InitializeComponent();
            _reportViewer.Load += ReportViewer_Load;
        }

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            try
            {
                if (!_isReportViewerLoaded)
                {
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
                                _reportViewer.LocalReport.ReportEmbeddedResource = "ExpressTMS.CompanyList.rdlc";

                                ReportParameter[] param = new ReportParameter[4];
                                param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                                param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                                param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                                param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                                _reportViewer.LocalReport.SetParameters(param);
                                ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["Company"]);
                                _reportViewer.LocalReport.DataSources.Clear();
                                _reportViewer.LocalReport.DataSources.Add(datasource);
                                _reportViewer.RefreshReport();
                                _isReportViewerLoaded = true;
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
                                _reportViewer.LocalReport.ReportEmbeddedResource = "ExpressTMS.DriverList.rdlc";

                                ReportParameter[] param = new ReportParameter[4];
                                param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                                param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                                param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                                param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                                _reportViewer.LocalReport.SetParameters(param);
                                ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["Driver"]);
                                _reportViewer.LocalReport.DataSources.Clear();
                                _reportViewer.LocalReport.DataSources.Add(datasource);
                                _reportViewer.RefreshReport();
                                _isReportViewerLoaded = true;
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
                                            c.TRANS_DATE <= endPeriod &&
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
                                _reportViewer.LocalReport.ReportEmbeddedResource = "ExpressTMS.ServicesCurrentDay.rdlc";

                                ReportParameter[] param = new ReportParameter[6];
                                param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                                param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                                param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                                param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                                param[4] = new ReportParameter("START_DATE", startPeriod.ToString("dd MMM yyyy"));
                                param[5] = new ReportParameter("END_DATE", endPeriod.ToString("dd MMM yyyy"));
                                _reportViewer.LocalReport.SetParameters(param);
                                ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["ServicesCurrentDay"]);
                                _reportViewer.LocalReport.DataSources.Clear();
                                _reportViewer.LocalReport.DataSources.Add(datasource);
                                _reportViewer.RefreshReport();
                                _isReportViewerLoaded = true;
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
                                _reportViewer.LocalReport.ReportEmbeddedResource = "ExpressTMS.AccumulatedByDriver.rdlc";

                                ReportParameter[] param = new ReportParameter[6];
                                param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                                param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                                param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                                param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                                param[4] = new ReportParameter("START_DATE", startPeriod.ToString("dd MMM yyyy"));
                                param[5] = new ReportParameter("END_DATE", endPeriod.ToString("dd MMM yyyy"));
                                _reportViewer.LocalReport.SetParameters(param);
                                ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["AccumulatedByDriver"]);
                                ReportDataSource taxsource = new ReportDataSource("DataSet2", tax.Tables["Tax"]);
                                ReportDataSource deductionsource = new ReportDataSource("DataSet3", DeductionSet.Tables["Deduction"]);
                                _reportViewer.LocalReport.DataSources.Clear();
                                _reportViewer.LocalReport.DataSources.Add(datasource);
                                _reportViewer.LocalReport.DataSources.Add(taxsource);
                                _reportViewer.LocalReport.DataSources.Add(deductionsource);
                                _reportViewer.RefreshReport();
                                _isReportViewerLoaded = true;
                            }
                        }
                        #endregion
                    }
                    else if (ReportType.REPORT_RECEIPT.Equals(type))
                    {
                        #region RECEIPT REPORT LOGIC
                        using (ExpressTaxi ctx = new ExpressTaxi(conn))
                        {
                            var query = from c in ctx.FinancialTransactions where c.DOC_NUM == DocNum select c;
                            if (query.Count() > 0)
                            {
                                ReportDataset dataset = new ReportDataset();
                                foreach (FinancialTransaction c in query)
                                {
                                    string VoidTxt = null;
                                    if (!c.IsValid)
                                        VoidTxt = "VOIDED";
                                    dataset.Tables["Receipt"].Rows.Add(
                                           new object[]
                                           {
                                               (int)c.DOC_NUM,
                                               (string)DateTime.Now.ToString("dd MMM yyyy"),
                                               (string)c.Driver.DRV_LICENSE,
                                               (string)c.Driver.DRV_NAME,
                                               (string)c.Company.CMP_NAME,
                                               (string)c.TRANS_DESTINATION,
                                               (string)c.DOC_TYPE_NAME,
                                               (int)c.NO_PAX,
                                               (string)c.DOCUMENT_DATE.ToString("dd MMM yyyy"),
                                               (decimal)c.TRANS_VALUE,
                                               (decimal)c.TRANS_FINAL_VALUE,
                                               (string)c.SERV_NAME,
                                               (string)c.VOUCHER_NUM,
                                               (string)VoidTxt
                                           }
                                    );
                                }
                                dataset.AcceptChanges();
                                _reportViewer.LocalReport.ReportEmbeddedResource = "ExpressTMS.ReceiptVoucher.rdlc";

                                ReportParameter[] param = new ReportParameter[4];
                                param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                                param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                                param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                                param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                                _reportViewer.LocalReport.SetParameters(param);
                                ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["Receipt"]);
                                _reportViewer.LocalReport.DataSources.Clear();
                                _reportViewer.LocalReport.DataSources.Add(datasource);
                                _reportViewer.RefreshReport();
                                _isReportViewerLoaded = true;
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
                                _reportViewer.LocalReport.ReportEmbeddedResource = "ExpressTMS.AccumulatedPaid.rdlc";

                                ReportParameter[] param = new ReportParameter[6];
                                param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                                param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                                param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                                param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                                param[4] = new ReportParameter("START_DATE", startPeriod.ToString("dd MMM yyyy"));
                                param[5] = new ReportParameter("END_DATE", endPeriod.ToString("dd MMM yyyy"));
                                _reportViewer.LocalReport.SetParameters(param);
                                ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["AccumulatedPaid"]);
                                _reportViewer.LocalReport.DataSources.Clear();
                                _reportViewer.LocalReport.DataSources.Add(datasource);
                                _reportViewer.RefreshReport();
                                _isReportViewerLoaded = true;
                            }
                        }
                        #endregion
                    }
                    else if (ReportType.REPORT_SERVICES.Equals(type))
                    {
                        #region REPORT SERVICES LOGIC
                        using (ExpressTaxi ctx = new ExpressTaxi(conn))
                        {
                            var query = from c in ctx.FinancialTransactions
                                        where
                                            c.TRANS_DATE >= startPeriod &&
                                            c.TRANS_DATE <= endPeriod && c.IsValid
                                        select c;
                            if (query.Count() > 0)
                            {
                                ReportDataset dataset = new ReportDataset();
                                int n = 0;
                                foreach (FinancialTransaction c in query)
                                {
                                    dataset.Tables["Services"].Rows.Add(
                                           new object[]
                                           {
                                               (int)n,
                                               (string)c.TRANS_DATE.ToString("dd MMM yyyy"),
                                               (string)c.Driver.DRV_LICENSE + ", " + c.Driver.DRV_NAME,
                                               (int)c.NO_PAX,
                                               (string)c.TRANS_DESTINATION,
                                               (decimal)c.TRANS_FINAL_VALUE,
                                               (string)c.VOUCHER_NUM
                                           }
                                    );
                                    n = n + 1;
                                }
                                dataset.AcceptChanges();
                                _reportViewer.LocalReport.ReportEmbeddedResource = "ExpressTMS.ServicesReport.rdlc";

                                ReportParameter[] param = new ReportParameter[6];
                                param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                                param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                                param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                                param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                                param[4] = new ReportParameter("START_DATE", startPeriod.ToString("dd MMM yyyy"));
                                param[5] = new ReportParameter("END_DATE", endPeriod.ToString("dd MMM yyyy"));
                                _reportViewer.LocalReport.SetParameters(param);
                                ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["Services"]);
                                _reportViewer.LocalReport.DataSources.Clear();
                                _reportViewer.LocalReport.DataSources.Add(datasource);
                                _reportViewer.RefreshReport();
                                _isReportViewerLoaded = true;
                            }
                        }
                        #endregion
                    }
                    else if (ReportType.REPORT_HOTEL_INFORMATION.Equals(type))
                    {
                        #region HOTEL INFORMATION LOGIC
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
                                _reportViewer.LocalReport.ReportEmbeddedResource = "ExpressTMS.HotelInformation.rdlc";

                                ReportParameter[] param = new ReportParameter[7];
                                param[0] = new ReportParameter("CMP_NAME", Config.CMP_NAME);
                                param[1] = new ReportParameter("CMP_ADDRESSLINE1", Config.CMP_ADDRESSLINE1);
                                param[2] = new ReportParameter("CMP_ADDRESSLINE2", Config.CMP_ADDRESSLINE2);
                                param[3] = new ReportParameter("CMP_PHONEFAX", Config.CMP_PHONEFAX);
                                param[4] = new ReportParameter("START_DATE", startPeriod.ToString("dd MMM yyyy"));
                                param[5] = new ReportParameter("END_DATE", endPeriod.ToString("dd MMM yyyy"));
                                param[6] = new ReportParameter("AGENT", HotelInfo);
                                _reportViewer.LocalReport.SetParameters(param);
                                ReportDataSource datasource = new ReportDataSource("DataSet1", dataset.Tables["HotelInformation"]);
                                _reportViewer.LocalReport.DataSources.Clear();
                                _reportViewer.LocalReport.DataSources.Add(datasource);
                                _reportViewer.RefreshReport();
                                _isReportViewerLoaded = true;
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                Config.FatalError();
            }
        }
    }

    public enum ReportType
    {
        REPORT_COMAPNY_LIST,
        REPORT_DRIVER_LIST,
        REPORT_SERVICES_CURRENT_DAY,
        REPORT_ACCUMULATED_BY_DRIVER,
        REPORT_RECEIPT,
        REPORT_ACCUMULATED_PAID,
        REPORT_SERVICES,
        REPORT_HOTEL_INFORMATION
    }
}
