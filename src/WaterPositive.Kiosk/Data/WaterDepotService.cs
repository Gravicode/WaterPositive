using WaterPositive.Models;
using Microsoft.EntityFrameworkCore;
using WaterPositive.Kiosk.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterPositive.Kiosk.Data
{
    public class WaterDepotService : ICrud<WaterDepot>
    {
        WaterPositiveDB db;

        public WaterDepotService()
        {
            if (db == null) db = new WaterPositiveDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.WaterDepots.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.WaterDepots.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<WaterDepot> FindByKeyword(string Keyword)
        {
            var data = from x in db.WaterDepots
                       where x.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<WaterDepot> GetAllData()
        {
            return db.WaterDepots.ToList();
        }

        public WaterDepot GetDataById(object Id)
        {
            return db.WaterDepots.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(WaterDepot data)
        {
            try
            {
                db.WaterDepots.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(WaterDepot data)
        {
            try
            {
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                /*
                if (sel != null)
                {
                    sel.Nama = data.Nama;
                    sel.Keterangan = data.Keterangan;
                    sel.Tanggal = data.Tanggal;
                    sel.DocumentUrl = data.DocumentUrl;
                    sel.StreamUrl = data.StreamUrl;
                    return true;

                }*/
                return true;
            }
            catch
            {

            }
            return false;
        }

        public long GetLastId()
        {
            return db.WaterDepots.Max(x => x.Id);
        }
    }

}