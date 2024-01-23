using Microsoft.EntityFrameworkCore;
using WaterPositive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterPositive.Web.Data;

namespace WaterPositive.Web.Data
{
    public class WaterPositiveDB : DbContext
    {

        public WaterPositiveDB()
        {
        }

        public WaterPositiveDB(DbContextOptions<WaterPositiveDB> options)
            : base(options)
        {
        }
        public DbSet<UsageLimit> UsageLimits { get; set; }
        public DbSet<WaterPrice> WaterPrices { get; set; }
        public DbSet<WaterUsage> WaterUsages { get; set; }
        public DbSet<CCTV> CCTVs { get; set; }
        public DbSet<WaterDepot> WaterDepots { get; set; }
        public DbSet<DataCounter> DataCounters { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<SensorData> SensorDatas { get; set; }
        public DbSet<WaterTankData> WaterTankDatas { get; set; }
      
        protected override void OnModelCreating(ModelBuilder builder)
        {
            /*
            builder.Entity<DataEventRecord>().HasKey(m => m.DataEventRecordId);
            builder.Entity<SourceInfo>().HasKey(m => m.SourceInfoId);

            // shadow properties
            builder.Entity<DataEventRecord>().Property<DateTime>("UpdatedTimestamp");
            builder.Entity<SourceInfo>().Property<DateTime>("UpdatedTimestamp");
            */
            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            /*
            ChangeTracker.DetectChanges();

            updateUpdatedProperty<SourceInfo>();
            updateUpdatedProperty<DataEventRecord>();
            */
            return base.SaveChanges();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(AppConstants.SQLConn, ServerVersion.AutoDetect(AppConstants.SQLConn));
            }
        }
        private void updateUpdatedProperty<T>() where T : class
        {
            /*
            var modifiedSourceInfo =
                ChangeTracker.Entries<T>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in modifiedSourceInfo)
            {
                entry.Property("UpdatedTimestamp").CurrentValue = DateTime.UtcNow;
            }
            */
        }

    }
}
