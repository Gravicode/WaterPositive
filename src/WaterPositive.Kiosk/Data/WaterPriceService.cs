using WaterPositive.Models;
using Microsoft.EntityFrameworkCore;

namespace WaterPositive.Kiosk.Data
{
    public class WaterPriceService : ICrud<WaterPrice>
    {
        WaterPositiveDB db;

        public WaterPriceService()
        {
            if (db == null) db = new WaterPositiveDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.WaterPrices.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.WaterPrices.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<WaterPrice> FindByKeyword(string Keyword)
        {
            var data = from x in db.WaterPrices
                       where x.Periode.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<WaterPrice> GetAllData()
        {
            return db.WaterPrices.ToList();
        }

        public WaterPrice GetDataById(object Id)
        {
            return db.WaterPrices.Where(x => x.Id == (long)Id).FirstOrDefault();
        }
        public WaterPrice GetCurrent()
        {
            var now = DateTime.Now;
            return db.WaterPrices.Where(x => now >= x.TanggalAwal && now <= x.TanggalAkhir).FirstOrDefault();
        }

        public bool InsertData(WaterPrice data)
        {
            try
            {
                db.WaterPrices.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(WaterPrice data)
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
            return db.WaterPrices.Max(x => x.Id);
        }
    }

}