using WaterPositive.Models;
using Microsoft.EntityFrameworkCore;

namespace WaterPositive.Web.Data
{
    public class WaterTankDataService : ICrud<WaterTankData>
    {
        WaterPositiveDB db;

        public WaterTankDataService()
        {
            if (db == null) db = new WaterPositiveDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.WaterTankDatas.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.WaterTankDatas.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<WaterTankData> FindByKeyword(string Keyword)
        {
            var data = from x in db.WaterTankDatas
                       where x.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<WaterTankData> GetAllData()
        {
            return db.WaterTankDatas.ToList();
        }

        public WaterTankData GetDataById(object Id)
        {
            return db.WaterTankDatas.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(WaterTankData data)
        {
            try
            {
                db.WaterTankDatas.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(WaterTankData data)
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
            return db.WaterTankDatas.Max(x => x.Id);
        }
    }

}