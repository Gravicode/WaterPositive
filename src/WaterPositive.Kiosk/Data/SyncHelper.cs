using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Web.WebView2.Core;
using WaterPositive.Models;

namespace WaterPositive.Kiosk.Data
{
    public class SyncHelper
    {
        WaterPositiveDB Local;
        WaterPositiveDB Remote;
       
        public SyncHelper(WaterPositiveDB local, WaterPositiveDB remote)
        {
            this.Local = local;
            this.Remote = remote;
        }

        public async Task<bool> SyncData()
        {
            try
            {
                //users
                {
                    //sync if needed
                    var remote_data = Remote.UserProfiles.AsNoTracking().ToList();
                    var local_data = Local.UserProfiles.ToList();
                    //Local.Database.ExecuteSqlRaw("DELETE FROM WaterUsages");
                    foreach (var item in remote_data)
                    {
                        var exist = local_data.FirstOrDefault(x => x.Username == item.Username);
                        if (exist == null)
                        {
                            item.SyncDate = DateTime.Now;
                            Local.UserProfiles.Add(item);
                        }
                        else if (exist.SyncDate < item.UpdatedDate)
                        {
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
                        }
                    }
                    Local.SaveChanges();
                }
                //water depots
                {
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
                        }
                        else if (exist.SyncDate < item.UpdatedDate)
                        {
                            exist.SyncDate = DateTime.Now;
                            exist.UpdatedDate = item.UpdatedDate;

                            exist.Latitude = item.Latitude;
                            exist.Longitude = item.Longitude;
                            exist.Nama = item.Nama;
                            exist.Lokasi = item.Lokasi;
                            exist.Keterangan = item.Keterangan;

                        }
                    }
                    Local.SaveChanges();
                }
                //water price
                {
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

                        }
                    }
                    Local.SaveChanges();
                }
                //CCTV
                {
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


                        }
                    }
                    Local.SaveChanges();
                }
                //water usage => push to cloud
                {
                    //sync if needed
                    var local_data = Local.WaterUsages.Where(x=>!x.SyncDate.HasValue).ToList();
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
                    }
                    Remote.SaveChanges();
                    Local.SaveChanges();
                }
                //UsageLimits
                {
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
                        }
                        else if (exist.SyncDate < item.UpdatedDate)
                        {
                            exist.SyncDate = DateTime.Now;
                            exist.UpdatedDate = item.UpdatedDate;

                            exist.LimitLiterHarian = item.LimitLiterHarian;
                            exist.TanggalAkhir = item.TanggalAkhir;
                            exist.TanggalAwal = item.TanggalAwal;
                            exist.Keterangan = item.Keterangan;

                        }
                    }
                    Local.SaveChanges();
                }
                //sensor data
                {
                    //push only
                    var local_data = Local.SensorDatas.Where(x=>x.SyncDate<=DateTime.MinValue).ToList();
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
                        newItem.DeviceId = item.DeviceId;
                        newItem.WaterDepotId = item.WaterDepotId;
                        item.SyncDate = DateTime.Now;
                        newItem.SyncDate = item.SyncDate;
                        newItem.UpdatedDate = item.UpdatedDate;
                        Remote.SensorDatas.Add(newItem);
                    }
                    Local.SaveChanges();
                    Remote.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("sync error:"+ex);
                return false;
            }
           
            return true;
        }
    }
}
