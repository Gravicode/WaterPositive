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
            AppConstants.SQLConn = ConfigurationManager.AppSettings["SQLConn"];
            AppConstants.SQLCloud = ConfigurationManager.AppSettings["SQLCloud"];
            var db = new WaterPositiveDB(true);
            db.Database.EnsureCreated();
            var db_remote = new WaterPositiveDB(false);

            //sync if needed
            if (db.UserProfiles.Count() != db_remote.UserProfiles.Count())
            {
                db.Database.ExecuteSqlRaw("DELETE FROM WaterUsages");
                db.Database.ExecuteSqlRaw("DELETE FROM UserProfiles");
                db.Database.ExecuteSqlRaw("DELETE FROM WaterDepots");
                db.Database.ExecuteSqlRaw("DELETE FROM WaterPrices");



                var users = db_remote.UserProfiles.ToList();
                var depots = db_remote.WaterDepots.ToList();
                var prices = db_remote.WaterPrices.ToList();
                db.UserProfiles.AddRange(users);
                db.WaterDepots.AddRange(depots);
                db.WaterPrices.AddRange(prices);
                db.SaveChanges();
            }
        }
    }
}