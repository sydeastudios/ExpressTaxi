using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;

namespace RepairDB
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ExpressTaxi Database Repair Tool set.");
            Console.WriteLine("Attempting Database Repair Please Wait...");
            try
            {
                string dataSource = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ExpressTaxi.sdf";
                string tmpfile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ExpressTaxi.sdf.tmp";
                SqlCeEngine ce = new SqlCeEngine(string.Format("Data Source = {0};" /*Password ='ExpressTaxi!10'"*/, dataSource));
                ce.Repair(string.Format("Data Source={0};"/* Password ='ExpressTaxi!10'"*/, tmpfile), RepairOption.DeleteCorruptedRows);
                ce.Dispose();
                System.IO.File.Delete(dataSource);
                System.IO.File.Move(tmpfile, dataSource);
                Console.WriteLine("Successfully completed the operation.");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}
