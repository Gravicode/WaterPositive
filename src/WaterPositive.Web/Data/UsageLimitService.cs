using WaterPositive.Models;
using Microsoft.EntityFrameworkCore;

namespace WaterPositive.Web.Data
{
    public class UsageLimitService : ICrud<UsageLimit>
    {
        WaterPositiveDB db;

        public UsageLimitService()
        {
            if (db == null) db = new WaterPositiveDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.UsageLimits.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.UsageLimits.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<UsageLimit> FindByKeyword(string Keyword)
        {
            var data = from x in db.UsageLimits
                       where x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<UsageLimit> GetAllData()
        {
            return db.UsageLimits.ToList();
        }

        public UsageLimit GetDataById(object Id)
        {
            return db.UsageLimits.Where(x => x.Id == (long)Id).FirstOrDefault();
        }
        public UsageLimit GetCurrentLimit()
        {
            var now = DateHelper.GetLocalTimeNow();
            var item = db.UsageLimits.Where(x => now >= x.TanggalAwal && now <= x.TanggalAkhir).FirstOrDefault();
            return item ?? new UsageLimit() { Id=-1, Keterangan = "default limit", LimitLiterHarian =50, TanggalAwal = DateTime.MinValue, TanggalAkhir = DateTime.MaxValue };
        }

        public bool InsertData(UsageLimit data)
        {
            try
            {
                db.UsageLimits.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(UsageLimit data)
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
            return db.UsageLimits.Max(x => x.Id);
        }
    }

}