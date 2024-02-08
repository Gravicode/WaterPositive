using Microsoft.EntityFrameworkCore;
using System.Configuration;
using WaterPositive.Kiosk.Data;

namespace WaterPositive.Kiosk
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Setup();
            Application.Run(new Form1());
        }

        static void Setup()
        {
            AppConstants.PrinterName = ConfigurationManager.AppSettings["PrinterName"];
            AppConstants.SQLConn = ConfigurationManager.AppSettings["SQLConn"];
            AppConstants.SQLCloud = ConfigurationManager.AppSettings["SQLCloud"];
            AppConstants.XbeePort = ConfigurationManager.AppSettings["XbeePort"];
            AppConstants.ComPort = ConfigurationManager.AppSettings["ComPort"];
            AppConstants.PrinterPort = ConfigurationManager.AppSettings["PrinterPort"];
            AppConstants.SyncTimer = ConfigurationManager.AppSettings["SyncTimer"];
            AppConstants.CloseTimer = int.Parse( ConfigurationManager.AppSettings["CloseTimer"]);
            var db = new WaterPositiveDB(true);
            db.Database.EnsureCreated();
        

            
        }
    }
}