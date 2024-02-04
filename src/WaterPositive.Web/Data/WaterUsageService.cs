using WaterPositive.Models;
using Microsoft.EntityFrameworkCore;
using WaterPositive.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WaterPositive.Web.Data
{
    public class WaterUsageService : ICrud<WaterUsage>
    {
        WaterPositiveDB db;

        public WaterUsageService()
        {
            if (db == null) db = new WaterPositiveDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.WaterUsages.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.WaterUsages.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<WaterUsage> FindByKeyword(string Keyword)
        {
            var data = from x in db.WaterUsages.Include(c=>c.User)
                       where x.User.FullName.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<WaterUsage> GetAllData()
        {
            return db.WaterUsages.Include(c=>c.User).Include(c=>c.WaterDepot).ToList();
        }
        public List<WaterUsage> GetAllData(string UserName)
        {
            return db.WaterUsages.Include(c=>c.User).Include(c => c.WaterDepot).Where(x=>x.User.Username == UserName).ToList();
        }
        public List<WaterUsage> GetAllDataNoInclude()
        {
            return db.WaterUsages.ToList();
        }

        public WaterUsage GetDataById(object Id)
        {
            return db.WaterUsages.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(WaterUsage data)
        {
            try
            {
                db.WaterUsages.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }
        public bool InsertDatas(List<WaterUsage> datas)
        {
            try
            {
                db.WaterUsages.AddRange(datas);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }


        public bool UpdateData(WaterUsage data)
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
            return db.WaterUsages.Max(x => x.Id);
        }
    }

}