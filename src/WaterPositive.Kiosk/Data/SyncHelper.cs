using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Web.WebView2.Core;
using WaterPositive.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.AxHost;

namespace WaterPositive.Kiosk.Data
{
    public class SyncHelper
    {
        WaterPositiveDB Local;
        WaterPositiveDB Remote;
       
        public SyncHelper(WaterPositiveDB local, WaterPositiveDB remote)
        {
            //var db = new WaterPositiveDB(true);            
            //var db_remote = new WaterPositiveDB(false);
            this.Local = local;
            this.Remote = remote;
        }

        public async Task<bool> SyncData()
        {

            string Stat = string.Empty;
            int UpdateCounter = 0;
            Tools.Logs.WriteLog($"sync started\r\n");
            //users
            try
            {

                //sync if needed
                var remote_data = Remote.UserProfiles.AsNoTracking().ToList();
                var local_data = Local.UserProfiles.ToList();

                //Local.Database.ExecuteSqlRaw("DELETE FROM WaterUsages");
                foreach (var item in remote_data)
                {
                    Remote.Entry(item).State = EntityState.Detached;
                    var exist = local_data.FirstOrDefault(x => x.Username == item.Username);
                    if (exist == null)
                    {
                        item.SyncDate = DateTime.Now;
                        Local.UserProfiles.Add(item);
                        UpdateCounter++;
                    }
                    else if (exist.SyncDate < item.UpdatedDate)
                    {
                        Local.Entry(exist).State = EntityState.Modified;
                        exist.SyncDate = DateTime.Now;
                        exist.UID = item.UID;
                        exist.PIN = item.PIN;
                        exist.FullName = item.FullName;
                        exist.Email = item.Email;
                        exist.Phone = item.Phone;
                        exist.KTP = item.KTP;
                        exist.PicUrl = item.PicUrl;
                        exist.Alamat = item.Alamat;
                        exist.UpdatedDate = item.UpdatedDate;
                        UpdateCounter++;
                    }
                }
                Local.SaveChanges();
                Stat = $"sync user: {UpdateCounter} items\r\n";
                Tools.Logs.WriteLog($"{Stat}");
            }
            catch (Exception ex)
            {
                Tools.Logs.WriteLog($"fail to sync user: {ex}");
                Console.WriteLine("sync error:"+ex);
                return false;
            }
            //water depots
            try
            {
                UpdateCounter = 0;
                //sync if needed
                var remote_data = Remote.WaterDepots.AsNoTracking().ToList();
                var local_data = Local.WaterDepots.ToList();

                //Local.Database.ExecuteSqlRaw("DELETE FROM WaterDepots");
                foreach (var item in remote_data)
                {
                    var exist = local_data.FirstOrDefault(x => x.Id == item.Id);
                    if (exist == null)
                    {
                        item.SyncDate = DateTime.Now;
                        Local.WaterDepots.Add(item);
                        UpdateCounter++;
                    }
                    else if (exist.SyncDate < item.UpdatedDate)
                    {
                        Local.Entry(exist).State = EntityState.Modified;
                        exist.SyncDate = DateTime.Now;
                        exist.UpdatedDate = item.UpdatedDate;

                        exist.Latitude = item.Latitude;
                        exist.Longitude = item.Longitude;
                        exist.Nama = item.Nama;
                        exist.Lokasi = item.Lokasi;
                        exist.Keterangan = item.Keterangan;
                        UpdateCounter++;
                    }
                }
                Local.SaveChanges();
                Stat = $"sync water depot: {UpdateCounter} items\r\n";
                Tools.Logs.WriteLog($"{Stat}");
            }
            catch (Exception ex)
            {
                Tools.Logs.WriteLog($"fail to sync water depot: {ex}");
                Console.WriteLine("sync error:" + ex);
                return false;
            }
            //water price
            try
            {
                UpdateCounter = 0;
                //sync if needed
                var remote_data = Remote.WaterPrices.AsNoTracking().ToList();
                var local_data = Local.WaterPrices.ToList();
                //Local.Database.ExecuteSqlRaw("DELETE FROM WaterPrices");
                foreach (var item in remote_data)
                {
                    var exist = local_data.FirstOrDefault(x => x.Id == item.Id);
                    if (exist == null)
                    {
                        item.SyncDate = DateTime.Now;
                        Local.WaterPrices.Add(item);
                        UpdateCounter++;
                    }
                    else if (exist.SyncDate < item.UpdatedDate)
                    {
                        exist.SyncDate = DateTime.Now;
                        exist.UpdatedDate = item.UpdatedDate;

                        exist.UpdatedBy = item.UpdatedBy;
                        exist.PricePerLiter = item.PricePerLiter;
                        exist.Periode = item.Periode;
                        exist.TanggalAkhir = item.TanggalAkhir;
                        exist.TanggalAwal = item.TanggalAwal;
                        exist.Keterangan = item.Keterangan;
                        UpdateCounter++;
                    }
                }
                Local.SaveChanges();
                Stat = $"sync water price: {UpdateCounter} items\r\n";
                Tools.Logs.WriteLog($"{Stat}");
            }
            catch (Exception ex)
            {
                Tools.Logs.WriteLog($"fail to sync water price: {ex}");
                Console.WriteLine("sync error:" + ex);
                return false;
            }
            //CCTV
            try
            {
                UpdateCounter = 0;
                //sync if needed
                var remote_data = Remote.CCTVs.AsNoTracking().ToList();
                var local_data = Local.CCTVs.ToList();
                //Local.Database.ExecuteSqlRaw("DELETE FROM CCTVs");
                foreach (var item in remote_data)
                {
                    var exist = local_data.FirstOrDefault(x => x.Id == item.Id);
                    if (exist == null)
                    {
                        item.SyncDate = DateTime.Now;
                        Local.CCTVs.Add(item);
                        UpdateCounter++;
                    }
                    else if (exist.SyncDate < item.UpdatedDate)
                    {
                        exist.SyncDate = DateTime.Now;
                        exist.UpdatedDate = item.UpdatedDate;

                        exist.WaterDepot = item.WaterDepot;
                        exist.Latitude = item.Latitude;
                        exist.Longitude = item.Longitude;
                        exist.Lokasi = item.Lokasi;
                        exist.Merek = item.Merek;
                        exist.Nama = item.Nama;

                        UpdateCounter++;
                    }
                }
                Local.SaveChanges();
                Stat = $"sync cctv: {UpdateCounter} items\r\n";
                Tools.Logs.WriteLog($"{Stat}");
            }
            catch (Exception ex)
            {
                Tools.Logs.WriteLog($"fail to sync cctv: {ex}");
                Console.WriteLine("sync error:" + ex);
                return false;
            }
            //water usage => push to cloud
            try
            {
                UpdateCounter = 0;
                //sync if needed
                var local_data = Local.WaterUsages.Where(x => !x.SyncDate.HasValue).ToList();
                //Local.Database.ExecuteSqlRaw("DELETE FROM WaterUsages");
                foreach (var item in local_data)
                {
                    item.SyncDate = DateTime.Now;
                    var newItem = new Models.WaterUsage()
                    {
                        SyncDate = item.SyncDate,
                        Tanggal = item.Tanggal,
                        TotalHarga = item.TotalHarga,
                        UpdatedDate = item.UpdatedDate,

                        UserId = item.UserId,
                        Volume = item.Volume,

                        WaterDepotId = item.WaterDepotId
                    };
                    Remote.WaterUsages.Add(newItem);
                    UpdateCounter++;
                }
                Remote.SaveChanges();
                Local.SaveChanges();
                Stat = $"sync water usage: {UpdateCounter} items\r\n";
                Tools.Logs.WriteLog($"{Stat}");
            }
            catch (Exception ex)
            {
                Tools.Logs.WriteLog($"fail to sync water usage: {ex}");
                Console.WriteLine("sync error:" + ex);
                return false;
            }
            //UsageLimits
            try
            {
                UpdateCounter = 0;
                //sync if needed
                var remote_data = Remote.UsageLimits.AsNoTracking().ToList();
                var local_data = Local.UsageLimits.ToList();
                //Local.Database.ExecuteSqlRaw("DELETE FROM WaterDepots");
                foreach (var item in remote_data)
                {
                    var exist = local_data.FirstOrDefault(x => x.Id == item.Id);
                    if (exist == null)
                    {
                        item.SyncDate = DateTime.Now;
                        Local.UsageLimits.Add(item);

                        UpdateCounter++;
                    }
                    else if (exist.SyncDate < item.UpdatedDate)
                    {
                        exist.SyncDate = DateTime.Now;
                        exist.UpdatedDate = item.UpdatedDate;

                        exist.LimitLiterHarian = item.LimitLiterHarian;
                        exist.TanggalAkhir = item.TanggalAkhir;
                        exist.TanggalAwal = item.TanggalAwal;
                        exist.Keterangan = item.Keterangan;

                        UpdateCounter++;
                    }
                }
                Local.SaveChanges();
                Stat = $"sync usage limit: {UpdateCounter} items\r\n";
                Tools.Logs.WriteLog($"{Stat}");
            }
            catch (Exception ex)
            {
                Tools.Logs.WriteLog($"fail to sync usage limit: {ex}");
                Console.WriteLine("sync error:" + ex);
                return false;
            }
            //sensor data
            try
            {
                UpdateCounter = 0;
                //push only
                var local_data = Local.SensorDatas.Where(x => x.SyncDate <= DateTime.MinValue).ToList();
                //Local.Database.ExecuteSqlRaw("DELETE FROM WaterDepots");
                foreach (var item in local_data)
                {
                    var newItem = new SensorData();
                    newItem.DO = item.DO;
                    newItem.Tds = item.Tds;
                    newItem.Temperature = item.Temperature;
                    newItem.WaterLevel = item.WaterLevel;
                    newItem.Tanggal = item.Tanggal;
                    newItem.Pressure = item.Pressure;
                    newItem.Altitude = item.Altitude;
                    newItem.DeviceId = item.DeviceId;
                    newItem.WaterDepotId = item.WaterDepotId;
                    item.SyncDate = DateTime.Now;
                    newItem.SyncDate = item.SyncDate;
                    newItem.UpdatedDate = item.UpdatedDate;
                    Remote.SensorDatas.Add(newItem);
                    UpdateCounter++;
                }
                Local.SaveChanges();
                Remote.SaveChanges();
                Stat = $"sync sensor data: {UpdateCounter} items\r\n";
                Tools.Logs.WriteLog($"{Stat}");
            }
            catch (Exception ex)
            {
                Tools.Logs.WriteLog($"fail to sync sensor data: {ex}");
                Console.WriteLine("sync error:" + ex);
                return false;
            }
            Tools.Logs.WriteLog($"sync ended\r\n");


            return true;
        }
    }
}
