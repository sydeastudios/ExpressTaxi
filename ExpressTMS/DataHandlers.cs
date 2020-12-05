using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace ExpressTMS
{
    public partial class FinancialTransaction
    {
        public string DRV_NAME_TX { get; set; }
    }

    public class myTax : ObservableCollection<AppliedTax>
    {
        public myTax() { }
    }

    public class myTransactions :
            ObservableCollection<FinancialTransaction>
    {
        public myTransactions() { }
    }

    public class myCompany :
            ObservableCollection<Company>
    {
        public myCompany()
        {
        }
    }

    public class myDriver :
        ObservableCollection<Driver>
    {
        public myDriver()
        {
        }
    }

    public class TransactionDataHandler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SqlCeConnection conn { get; set; }
        public int _DOC_NUM { get; set; }
        public int _DRV_COD { get; set; }
        public string _Destination { get; set; }
        public string _Serv_Name { get; set; }
        public string _Doc_Type_Name { get; set; }
        public DateTime _Doc_Date { get; set; }
        public decimal _Trans_Value { get; set; }
        public decimal _Trans_Reduct { get; set; }
        public decimal _Trans_Final { get; set; }
        public string _Voucher_Num { get; set; }
        public int _No_Pax { get; set; }
        public string _Comments { get; set; }
        public int _CMP_COD { get; set; }
        public bool isValid { get; private set; }

        public TransactionDataHandler()
        {
            _DOC_NUM = 0;
            _DRV_COD = 0;
            _Trans_Value = decimal.Zero;
            _Trans_Reduct = decimal.Zero;
            _Trans_Final = decimal.Zero;
            _No_Pax = 0;
            _CMP_COD = 0;
            _Doc_Date = DateTime.MinValue;
        }

        public bool VoidTransaction(int docNum)
        {
            try
            {
                using (ExpressTaxi ctx = new ExpressTaxi(conn))
                {
                    FinancialTransaction t = ctx.FinancialTransactions.SingleOrDefault(r => r.DOC_NUM == docNum);
                    if (t != null)
                    {
                        t.IsValid = false;
                        ctx.SubmitChanges();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }

        public int? GetNewDocNum()
        {
            try
            {
                using (ExpressTaxi ctx = new ExpressTaxi(conn))
                {
                    UniqueNumber u = ctx.UniqueNumbers.SingleOrDefault(r => r.UniqueName == "Receipt");
                    if (u != null)
                        return u.UniqueValue;
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
            return null;
        }

        public bool ValidateTransaction()
        {
            if (string.IsNullOrEmpty(_Destination) ||
                string.IsNullOrEmpty(_Serv_Name) ||
                string.IsNullOrEmpty(_Doc_Type_Name) ||
                _DOC_NUM == 0 ||
                _DRV_COD == 0 ||
                _CMP_COD == 0 ||
                _Trans_Value == decimal.Zero ||
                _Doc_Date == DateTime.MinValue)
                return false;
            return true;
        }

        public bool LoadTransaction(int DocNum)
        {
            try
            {
                using (ExpressTaxi ctx = new ExpressTaxi(conn))
                {
                    FinancialTransaction t = ctx.FinancialTransactions.SingleOrDefault(r => r.DOC_NUM == DocNum);
                    if (t != null)
                    {
                        _CMP_COD = t.CMP_COD;
                        _DOC_NUM = t.DOC_NUM;
                        _Doc_Type_Name = t.DOC_TYPE_NAME;
                        _Doc_Date = t.DOCUMENT_DATE;
                        _DRV_COD = t.DRV_COD;
                        isValid = t.IsValid;
                        _No_Pax = t.NO_PAX;
                        _Serv_Name = t.SERV_NAME;
                        _Comments = t.TRANS_COMMENTS;
                        _Destination = t.TRANS_DESTINATION;
                        _Trans_Final = t.TRANS_FINAL_VALUE;
                        _Trans_Reduct = t.TRANS_REDUCTION;
                        _Trans_Value = t.TRANS_VALUE;
                        _Voucher_Num = t.VOUCHER_NUM;
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

        public bool SaveTransaction()
        {
            try
            {
                if (ValidateTransaction())
                {
                    using (ExpressTaxi ctx = new ExpressTaxi(conn))
                    {
                        FinancialTransaction t = ctx.FinancialTransactions.SingleOrDefault(r => r.DOC_NUM == _DOC_NUM);
                        if (t == null)
                        {
                            t = new FinancialTransaction();
                            t.CMP_COD = _CMP_COD;
                            t.DOC_NUM = _DOC_NUM;
                            t.DOC_TYPE_NAME = _Doc_Type_Name;
                            t.DOCUMENT_DATE = _Doc_Date;
                            t.DRV_COD = _DRV_COD;
                            t.IsValid = true;
                            t.LAST_MODIFIED = DateTime.Now;
                            t.NO_PAX = _No_Pax;
                            t.SERV_NAME = _Serv_Name;
                            t.TRANS_COMMENTS = _Comments;
                            t.TRANS_DATE = DateTime.Today;
                            t.TRANS_DESTINATION = _Destination;
                            t.TRANS_FINAL_VALUE = _Trans_Final;
                            t.TRANS_REDUCTION = _Trans_Reduct;
                            t.TRANS_VALUE = _Trans_Value;
                            t.VOUCHER_NUM = _Voucher_Num;
                            UniqueNumber num = ctx.UniqueNumbers.SingleOrDefault(r => r.UniqueName == "Receipt");
                            if (num != null)
                            {
                                if (num.UniqueValue != _DOC_NUM)
                                {
                                    _DOC_NUM = num.UniqueValue;
                                    t.DOC_NUM = _DOC_NUM;
                                }
                                num.UniqueValue = num.UniqueValue + 1;
                                ctx.FinancialTransactions.InsertOnSubmit(t);
                                ctx.SubmitChanges();
                                return true;
                            }
                        }
                        else // edit mode
                        {
                            t.CMP_COD = _CMP_COD;
                            t.DOC_NUM = _DOC_NUM;
                            t.DOC_TYPE_NAME = _Doc_Type_Name;
                            t.DOCUMENT_DATE = _Doc_Date;
                            t.DRV_COD = _DRV_COD;
                            t.IsValid = true;
                            t.LAST_MODIFIED = DateTime.Now;
                            t.NO_PAX = _No_Pax;
                            t.SERV_NAME = _Serv_Name;
                            t.TRANS_COMMENTS = _Comments;
                            t.TRANS_DATE = DateTime.Today;
                            t.TRANS_DESTINATION = _Destination;
                            t.TRANS_FINAL_VALUE = _Trans_Final;
                            t.TRANS_REDUCTION = _Trans_Reduct;
                            t.TRANS_VALUE = _Trans_Value;
                            t.VOUCHER_NUM = _Voucher_Num;
                            ctx.SubmitChanges();
                            return true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }
    }

    public class DriverDataHandler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SqlCeConnection conn { get; set; }
        public string _DriverName { get; set; }
        public string _StreetName { get; set; }
        public string _VillageName { get; set; }
        public string _ParishName { get; set; }
        public string _CountryName { get; set; }
        public string _Phone { get; set; }
        public string _Cell { get; set; }
        public string _Email { get; set; }
        public string _Plate { get; set; }
        public int? Drv_Code { get; set; }

        const string MatchPhonePattern = @"^((((\(\d{3}\))|(\d{3}-))\d{3}-\d{4})|(\+?\d{2}((-| )\d{1,8}){1,5}))(( x| ext)\d{1,5}){0,1}$";
        const string MatchEmailPattern =
                                @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                              + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                              + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
        public bool ValidateDriver()
        {
            if (string.IsNullOrEmpty(_DriverName) ||
                string.IsNullOrEmpty(_StreetName) ||
                string.IsNullOrEmpty(_VillageName) ||
                string.IsNullOrEmpty(_ParishName) ||
                string.IsNullOrEmpty(_CountryName) ||
                string.IsNullOrEmpty(_Plate))
                return false;


            if (!string.IsNullOrEmpty(_Phone))
            {
                if (!Regex.IsMatch(_Phone, MatchPhonePattern))
                    return false;
            }
            if (!string.IsNullOrEmpty(_Cell))
            {
                if (!Regex.IsMatch(_Cell, MatchPhonePattern))
                    return false;
            }
            if (!string.IsNullOrEmpty(_Email))
            {
                if (!Regex.IsMatch(_Email, MatchEmailPattern))
                    return false;
            }
            return true;
        }

        public bool LoadDriver(int DRV_COD)
        {
            try
            {
                using (ExpressTaxi ctx = new ExpressTaxi(conn))
                {
                    Driver c = ctx.Drivers.SingleOrDefault(r => r.DRV_COD == DRV_COD);
                    if (c != null)
                    {
                        Drv_Code = DRV_COD;
                        _DriverName = c.DRV_NAME;
                        _CountryName = c.COU_NAME;
                        _Email = c.DRV_EMAIL;
                        _Cell = c.DRV_CELL;
                        _ParishName = c.PAR_NAME;
                        _Phone = c.DRV_PHONE;
                        _StreetName = c.DRV_ADDRESS;
                        _VillageName = c.VIL_NAME;
                        _Plate = c.DRV_LICENSE;
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

        public bool SaveDriver()
        {
            try
            {
                if (ValidateDriver())
                {
                    using (ExpressTaxi ctx = new ExpressTaxi(conn))
                    {
                        if (Drv_Code != null)
                        {
                            Driver c = ctx.Drivers.SingleOrDefault(r => r.DRV_COD == Drv_Code.Value);
                            if(c != null)
                            {
                                c.DRV_NAME = _DriverName;
                                c.DRV_ADDRESS = _StreetName;
                                c.VIL_NAME = _VillageName;
                                c.PAR_NAME = _ParishName;
                                c.COU_NAME = _CountryName;
                                c.DRV_PHONE = _Phone;
                                c.DRV_CELL = _Cell;
                                c.DRV_EMAIL = _Email;
                                c.DRV_REG_DATE = DateTime.Now;
                                c.LAST_MODIFIED = DateTime.Now;
                                c.DRV_LICENSE = _Plate;
                                ctx.SubmitChanges();
                                return true;
                            }
                        }
                        else
                        {
                            Driver c = ctx.Drivers.SingleOrDefault(r => r.DRV_LICENSE == _Plate);
                            if (c == null) // new company
                            {
                                c = new Driver();
                                c.DRV_NAME = _DriverName;
                                c.DRV_ADDRESS = _StreetName;
                                c.VIL_NAME = _VillageName;
                                c.PAR_NAME = _ParishName;
                                c.COU_NAME = _CountryName;
                                c.DRV_PHONE = _Phone;
                                c.DRV_CELL = _Cell;
                                c.DRV_EMAIL = _Email;
                                c.DRV_REG_DATE = DateTime.Now;
                                c.LAST_MODIFIED = DateTime.Now;
                                c.DRV_LICENSE = _Plate;
                                ctx.Drivers.InsertOnSubmit(c);
                                ctx.SubmitChanges();
                                return true;
                            }
                            else // update
                            {
                                c.DRV_NAME = _DriverName;
                                c.DRV_ADDRESS = _StreetName;
                                c.VIL_NAME = _VillageName;
                                c.PAR_NAME = _ParishName;
                                c.COU_NAME = _CountryName;
                                c.DRV_PHONE = _Phone;
                                c.DRV_CELL = _Cell;
                                c.DRV_EMAIL = _Email;
                                c.DRV_REG_DATE = DateTime.Now;
                                c.LAST_MODIFIED = DateTime.Now;
                                c.DRV_LICENSE = _Plate;
                                ctx.SubmitChanges();
                                return true;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }
    }

    public class CompanyDataHandler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SqlCeConnection conn { get; set; }
        public string _CompanyName { get; set; }
        public string _StreetName { get; set; }
        public string _VillageName { get; set; }
        public string _ParishName { get; set; }
        public string _CountryName { get; set; }
        public string _Phone { get; set; }
        public string _Fax { get; set; }
        public string _Email { get; set; }

        const string MatchPhonePattern = @"^((((\(\d{3}\))|(\d{3}-))\d{3}-\d{4})|(\+?\d{2}((-| )\d{1,8}){1,5}))(( x| ext)\d{1,5}){0,1}$";
        const string MatchEmailPattern =
                                @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                              + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                              + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        public bool ValidateCompany()
        {
            if (string.IsNullOrEmpty(_CompanyName) ||
                string.IsNullOrEmpty(_StreetName) ||
                string.IsNullOrEmpty(_VillageName) ||
                string.IsNullOrEmpty(_ParishName) ||
                string.IsNullOrEmpty(_CountryName))
                return false;


            if (!string.IsNullOrEmpty(_Phone))
            {
                if (!Regex.IsMatch(_Phone, MatchPhonePattern))
                    return false;
            }
            if (!string.IsNullOrEmpty(_Fax))
            {
                if (!Regex.IsMatch(_Fax, MatchPhonePattern))
                    return false;
            }
            if (!string.IsNullOrEmpty(_Email))
            {
                if (!Regex.IsMatch(_Email, MatchEmailPattern))
                    return false;
            }
            return true;
        }

        public bool LoadCompany(int CMP_COD)
        {
            try
            {
                using (ExpressTaxi ctx = new ExpressTaxi(conn))
                {
                    Company c = ctx.Companies.SingleOrDefault(r => r.CMP_COD == CMP_COD);
                    if (c != null)
                    {
                        _CompanyName = c.CMP_NAME;
                        _CountryName = c.COU_NAME;
                        _Email = c.CMP_EMAIL;
                        _Fax = c.CMP_FAX;
                        _ParishName = c.PAR_NAME;
                        _Phone = c.CMP_PHONE;
                        _StreetName = c.CMP_ADDRESS;
                        _VillageName = c.VIL_NAME;
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

        public bool SaveCompany()
        {
            try
            {
                if (ValidateCompany())
                {
                    using (ExpressTaxi ctx = new ExpressTaxi(conn))
                    {
                        Company c = ctx.Companies.SingleOrDefault(r => r.CMP_NAME == _CompanyName);
                        if (c == null) // new company
                        {
                            c = new Company();
                            c.CMP_NAME = _CompanyName;
                            c.CMP_ADDRESS = _StreetName;
                            c.VIL_NAME = _VillageName;
                            c.PAR_NAME = _ParishName;
                            c.COU_NAME = _CountryName;
                            c.CMP_PHONE = _Phone;
                            c.CMP_FAX = _Fax;
                            c.CMP_EMAIL = _Email;
                            c.CMP_REG_DATE = DateTime.Now;
                            c.LAST_MODIFIED = DateTime.Now;
                            ctx.Companies.InsertOnSubmit(c);
                            ctx.SubmitChanges();
                            return true;
                        }
                        else // update
                        {
                            c.CMP_NAME = _CompanyName;
                            c.CMP_ADDRESS = _StreetName;
                            c.VIL_NAME = _VillageName;
                            c.PAR_NAME = _ParishName;
                            c.COU_NAME = _CountryName;
                            c.CMP_PHONE = _Phone;
                            c.CMP_FAX = _Fax;
                            c.CMP_EMAIL = _Email;
                            c.CMP_REG_DATE = DateTime.Now;
                            c.LAST_MODIFIED = DateTime.Now;
                            ctx.SubmitChanges();
                            return true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }
    }
}
