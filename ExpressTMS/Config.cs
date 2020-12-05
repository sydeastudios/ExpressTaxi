using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace ExpressTMS
{
    public static class Config
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
                (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string sdfFile { get; set; }
        public static string bakdir { get; set; }

        public static string CMP_NAME { get; set; }
        public static string CMP_ADDRESSLINE1 { get; set; }
        public static string CMP_ADDRESSLINE2 { get; set; }
        public static string CMP_PHONEFAX { get; set; }

        public static string Subject { get; set; }
        public static string BodyText { get; set; }
        public static string Logo { get; set; }

        public static bool SaveCfg()
        {
            try
            {
                string SettingsFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ExpressTMS.xml";
                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Indent = true;
                setting.IndentChars = " ";
                setting.NewLineOnAttributes = true;
                using (XmlWriter write = XmlWriter.Create(SettingsFile, setting))
                {
                    write.WriteStartElement("ExpressTaxi.Configuration");
                    write.WriteAttributeString("xmlns", "xsi", null, @"http://www.w3.org/2001/XMLSchema-instance");
                    write.WriteAttributeString("xsi", "noNamespaceSchemaLocation", null, @".\ExpressTms.xsd");
                    write.WriteStartElement("Database");
                    write.WriteElementString("DatabaseFile", sdfFile);
                    write.WriteElementString("BackupDir", bakdir);
                    write.WriteEndElement();
                    write.WriteStartElement("Company");
                    write.WriteElementString("CompanyName", CMP_NAME);
                    write.WriteElementString("AddressLine1", CMP_ADDRESSLINE1);
                    write.WriteElementString("AddressLine2", CMP_ADDRESSLINE2);
                    write.WriteElementString("PhoneFax", CMP_PHONEFAX);
                    write.WriteElementString("Logo", Logo);
                    write.WriteEndElement();
                    write.WriteStartElement("EmailInfo");
                    write.WriteElementString("Subject", Subject);
                    write.WriteElementString("Body", BodyText);
                    write.WriteEndElement();
                    write.WriteEndElement();
                }
                return true;
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }
        public static void LoadCfg()
        {
            try
            {
                string SettingsFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ExpressTMS.xml";
                XmlReaderSettings setting = new XmlReaderSettings();
                setting.IgnoreWhitespace = true;
                using (XmlReader reader = XmlReader.Create(SettingsFile, setting))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if ("DatabaseFile".Equals(reader.Name))
                            {
                                if (reader.Read())
                                {
                                    sdfFile = reader.Value.ToString();
                                }
                            }
                            else if ("BackupDir".Equals(reader.Name))
                            {
                                if (reader.Read())
                                {
                                    bakdir = reader.Value.ToString();
                                }
                            }
                            else if ("CompanyName".Equals(reader.Name))
                            {
                                if (reader.Read())
                                {
                                    CMP_NAME = reader.Value.ToString();
                                }
                            }
                            else if ("AddressLine1".Equals(reader.Name))
                            {
                                if (reader.Read())
                                {
                                    CMP_ADDRESSLINE1 = reader.Value.ToString();
                                }
                            }
                            else if ("AddressLine2".Equals(reader.Name))
                            {
                                if (reader.Read())
                                {
                                    CMP_ADDRESSLINE2 = reader.Value.ToString();
                                }
                            }
                            else if ("PhoneFax".Equals(reader.Name))
                            {
                                if (reader.Read())
                                {
                                    CMP_PHONEFAX = reader.Value.ToString();
                                }
                            }
                            else if ("Subject".Equals(reader.Name))
                            {
                                if (reader.Read())
                                {
                                    Subject = reader.Value.ToString();
                                }
                            }
                            else if ("Body".Equals(reader.Name))
                            {
                                if (reader.Read())
                                {
                                    BodyText = reader.Value.ToString();
                                }
                            }
                            else if ("Logo".Equals(reader.Name))
                            {
                                if (reader.Read())
                                {
                                    Logo = reader.Value.ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                ShowErrorMessage("Failed to load the configuration data, some elements may not function correctly.");
                log.Error(ex);
            }
        }

        public static void FatalError()
        {
            ShowErrorMessage("A fatal error has occurred, please view the log file for a detail explanation.");
        }

        public static void ShowErrorMessage(string msg)
        {
            System.Windows.MessageBox.Show(msg, "Express Taxi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }

        public static void ShowInfoMessage(string msg)
        {
            System.Windows.MessageBox.Show(msg, "Express Taxi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        internal static bool ExpTmsRSet {get; private set;}
        internal static bool IsRegistered
        {
            get
            {
                return ExpTmsRSet;
            }
        }

        internal static bool LoadAppRegInfo()
        {
            try
            {
                string LicenseFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ExpressTaxi.lic";
                string[] LicenseArray = new string[3];
                using (StreamReader reader = new StreamReader(LicenseFile))
                {
                    string line1 = reader.ReadLine();
                    string line2 = reader.ReadLine();
                    string line3 = reader.ReadLine();
                    string[] tmpArray = line1.Split(':');
                    if (tmpArray[0] == "BusinessName")
                        LicenseArray[0] = tmpArray[1];
                    else if (tmpArray[0] == "SerialKey")
                        LicenseArray[1] = tmpArray[1];
                    else if (tmpArray[0] == "ActivationCode")
                        LicenseArray[2] = tmpArray[1];

                    tmpArray = line2.Split(':');
                    if (tmpArray[0] == "BusinessName")
                        LicenseArray[0] = tmpArray[1];
                    else if (tmpArray[0] == "SerialKey")
                        LicenseArray[1] = tmpArray[1];
                    else if (tmpArray[0] == "ActivationCode")
                        LicenseArray[2] = tmpArray[1];

                    tmpArray = line3.Split(':');
                    if (tmpArray[0] == "BusinessName")
                        LicenseArray[0] = tmpArray[1];
                    else if (tmpArray[0] == "SerialKey")
                        LicenseArray[1] = tmpArray[1];
                    else if (tmpArray[0] == "ActivationCode")
                        LicenseArray[2] = tmpArray[1];

                    string[] sKey = LicenseArray[1].Split('-');
                    SerialKey key = new SerialKey();
                    key._BusinessName = LicenseArray[0];
                    key._KeyHashCode = LicenseArray[2];
                    key._KeyPartA = sKey[0];
                    key._KeyPartB = sKey[1];
                    key._KeyPartC = sKey[2];
                    key._KeyPartD = sKey[3];
                    
                    ExpTmsRSet = ValidateSerialKey.ValidateSK(key);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }
    }
}
