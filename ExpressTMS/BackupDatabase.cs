using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Packaging;
using System.IO;
using System.IO.Compression;
using Ionic.Zip;

namespace ExpressTMS
{
    public class BackupRestore
    {
        // AddFileToZip("Output.zip", @"C:\Windows\Notepad.exe");

        private const long BUFFER_SIZE = 4096;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
                (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static void CopyStream(System.IO.FileStream inputStream, System.IO.Stream outputStream)
        {
            long bufferSize = inputStream.Length < BUFFER_SIZE ? inputStream.Length : BUFFER_SIZE;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            long bytesWritten = 0;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
                bytesWritten += bufferSize;
            }
        }

        private static void AddToZipFile(string zipFilename, string fileToAdd)
        {
            using (Package zip = System.IO.Packaging.Package.Open(zipFilename, FileMode.OpenOrCreate))
            {
                string destFilename = ".\\" + Path.GetFileName(fileToAdd);
                Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
                if (zip.PartExists(uri))
                {
                    zip.DeletePart(uri);
                }
                PackagePart part = zip.CreatePart(uri, "", CompressionOption.Normal);
                using (FileStream fileStream = new FileStream(fileToAdd, FileMode.Open, FileAccess.Read))
                {
                    using (Stream dest = part.GetStream())
                    {
                        CopyStream(fileStream, dest);
                    }
                }
            }
        }

        public static List<string> GetAvailableRestorationPoints()
        {
            try
            {
                List<string> r = new List<string>();
                string[] array1 = Directory.GetFiles(Config.bakdir, "*zip");
                foreach (string s in array1)
                    r.Add(s);
                return r;
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
            return null;
        }

        public static bool RestoreDatabase(string strZipFile)
        {
            try
            {
                string TargetDirectory = System.IO.Path.GetDirectoryName(Config.sdfFile);
                using (ZipFile zip = ZipFile.Read(strZipFile))
                {
                    foreach (ZipEntry e in zip)
                    {
                        e.Extract(TargetDirectory, ExtractExistingFileAction.OverwriteSilently);  // true => overwrite existing files
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }

        public static bool BackupDatabase()
        {
            try
            {
                string szZipFile = Config.bakdir + DateTime.Now.ToString("D") + ".zip";
                string xmlfile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ExpressTMS.xml";
                string xsdfile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ExpressTMS.xsd";
                AddToZipFile(szZipFile, Config.sdfFile);
                AddToZipFile(szZipFile, xmlfile);
                AddToZipFile(szZipFile, xsdfile);
                return true;
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }
    }
}
