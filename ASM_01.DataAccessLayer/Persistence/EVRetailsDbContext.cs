using ASM_01.DataAccessLayer.Entities.VehicleModels;
using ASM_01.DataAccessLayer.Entities.Warehouse;
using ASM_01.DataAccessLayer.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_01.DataAccessLayer.Persistence
{
    public class EVRetailsDbContext : DbContext
    {
        public EVRetailsDbContext()
        {

        }
        public EVRetailsDbContext(DbContextOptions<EVRetailsDbContext> options) : base(options)
        {

        }

        public static string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.db.json")
                .Build();
            // Replace with your actual connection string
            return configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection String not found in DbContext");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EVRetailsDbContext).Assembly);

            // ================================
            // Seed Data
            // ================================
            modelBuilder.Entity<EvModel>().HasData(
                new EvModel { EvModelId = 1, ModelName = "VinFast VF8", Description = "Mid-size all-electric SUV", Status = EvStatus.Available },
                new EvModel { EvModelId = 2, ModelName = "Tesla Model Y", Description = "Compact SUV with long range", Status = EvStatus.Available }
            );

            modelBuilder.Entity<EvTrim>().HasData(
                new EvTrim { EvTrimId = 1, EvModelId = 1, TrimName = "VF8 Eco", ModelYear = 2024, Description = "Entry version" },
                new EvTrim { EvTrimId = 2, EvModelId = 1, TrimName = "VF8 Plus", ModelYear = 2024, Description = "Premium version" },
                new EvTrim { EvTrimId = 3, EvModelId = 2, TrimName = "Model Y Long Range", ModelYear = 2024, Description = "Dual motor" },
                new EvTrim { EvTrimId = 4, EvModelId = 2, TrimName = "Model Y Performance", ModelYear = 2024, Description = "High performance" }
            );

            modelBuilder.Entity<Spec>().HasData(
                new Spec { SpecId = 1, SpecName = "Battery Capacity", Unit = "kWh", Category = SpecCategory.Battery },
                new Spec { SpecId = 2, SpecName = "Range", Unit = "km", Category = SpecCategory.Battery },
                new Spec { SpecId = 3, SpecName = "Motor Power", Unit = "hp", Category = SpecCategory.Motor },
                new Spec { SpecId = 4, SpecName = "Charging Time (fast)", Unit = "minutes", Category = SpecCategory.Charging },
                new Spec { SpecId = 5, SpecName = "Seating Capacity", Unit = "seats", Category = SpecCategory.Interior }
            );

            modelBuilder.Entity<TrimPrice>().HasData(
                new TrimPrice { TrimPriceId = 1, EvTrimId = 1, ListedPrice = 46000, EffectiveDate = new DateTime(2024, 1, 1) },
                new TrimPrice { TrimPriceId = 2, EvTrimId = 1, ListedPrice = 47000, EffectiveDate = new DateTime(2024, 6, 1) },
                new TrimPrice { TrimPriceId = 3, EvTrimId = 2, ListedPrice = 52000, EffectiveDate = new DateTime(2024, 1, 1) },
                new TrimPrice { TrimPriceId = 4, EvTrimId = 3, ListedPrice = 55000, EffectiveDate = new DateTime(2024, 1, 1) },
                new TrimPrice { TrimPriceId = 5, EvTrimId = 3, ListedPrice = 56000, EffectiveDate = new DateTime(2024, 8, 1) },
                new TrimPrice { TrimPriceId = 6, EvTrimId = 4, ListedPrice = 61000, EffectiveDate = new DateTime(2024, 1, 1) }
            );

            modelBuilder.Entity<TrimSpec>().HasData(
                // VF8 Eco
                new TrimSpec { EvTrimId = 1, SpecId = 1, Value = "82" },
                new TrimSpec { EvTrimId = 1, SpecId = 2, Value = "420" },
                new TrimSpec { EvTrimId = 1, SpecId = 3, Value = "350" },
                new TrimSpec { EvTrimId = 1, SpecId = 4, Value = "35" },
                new TrimSpec { EvTrimId = 1, SpecId = 5, Value = "5" },

                // VF8 Plus
                new TrimSpec { EvTrimId = 2, SpecId = 1, Value = "87" },
                new TrimSpec { EvTrimId = 2, SpecId = 2, Value = "470" },
                new TrimSpec { EvTrimId = 2, SpecId = 3, Value = "402" },
                new TrimSpec { EvTrimId = 2, SpecId = 4, Value = "30" },
                new TrimSpec { EvTrimId = 2, SpecId = 5, Value = "5" },

                // Model Y Long Range
                new TrimSpec { EvTrimId = 3, SpecId = 1, Value = "82" },
                new TrimSpec { EvTrimId = 3, SpecId = 2, Value = "497" },
                new TrimSpec { EvTrimId = 3, SpecId = 3, Value = "384" },
                new TrimSpec { EvTrimId = 3, SpecId = 4, Value = "27" },
                new TrimSpec { EvTrimId = 3, SpecId = 5, Value = "5" },

                // Model Y Performance
                new TrimSpec { EvTrimId = 4, SpecId = 1, Value = "82" },
                new TrimSpec { EvTrimId = 4, SpecId = 2, Value = "450" },
                new TrimSpec { EvTrimId = 4, SpecId = 3, Value = "456" },
                new TrimSpec { EvTrimId = 4, SpecId = 4, Value = "25" },
                new TrimSpec { EvTrimId = 4, SpecId = 5, Value = "5" }
            );

            modelBuilder.Entity<Dealer>().HasData(
                new Dealer { DealerId = 1, Name = "City EV Motors", Address = "New York, NY" }
            );

            modelBuilder.Entity<VehicleStock>().HasData(
                new VehicleStock { VehicleStockId = 1, DealerId = 1, EvTrimId = 1, Quantity = 5 },
                new VehicleStock { VehicleStockId = 2, DealerId = 1, EvTrimId = 3, Quantity = 3 }
            );

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(GetConnectionString(), options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
            }
            base.OnConfiguring(optionsBuilder);
        }
        // DbSets for your entities
        public DbSet<EvModel> EvModels { get; set; }
        public DbSet<EvTrim> EvTrims { get; set; }
        public DbSet<TrimPrice> TrimPrices { get; set; }
        public DbSet<Spec> Specs { get; set; }
        public DbSet<TrimSpec> TrimSpecs { get; set; }
        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<VehicleStock> VehicleStocks { get; set; }
        public DbSet<DistributionRequest> DistributionRequests { get; set; }

    }
}
