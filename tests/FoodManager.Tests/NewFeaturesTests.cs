using System;
using System.Collections.Generic;
using System.IO;
using FoodManager;
using FoodManager.Services.Database;
using FoodManager.Services.Export;
using Xunit;

namespace FoodManager.Tests
{
    public class NewFeaturesTests : IDisposable
    {
        private readonly string _tempDbPath;
        private readonly DatabaseService _db;

        public NewFeaturesTests()
        {
            _tempDbPath = Path.Combine(Path.GetTempPath(), $"test_new_features_{Guid.NewGuid():N}.db");
            _db = new DatabaseService(_tempDbPath);
            _db.EnsureDatabase();
        }

        public void Dispose()
        {
            try
            {
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                if (File.Exists(_tempDbPath)) File.Delete(_tempDbPath);
            }
            catch { }
        }

        [Fact]
        public void MicronutrientInfo_AddScaled_ComputesAccurately()
        {
            var appleMicro = new MicronutrientInfo
            {
                VitaminC_mg = 4.6,
                Potassium_mg = 107.0,
                Calcium_mg = 6.0
            };

            var total = new MicronutrientInfo();
            // 200g of apples => factor = 2.0
            total.AddScaled(appleMicro, 2.0);
            total.Round();

            Assert.Equal(9.2, total.VitaminC_mg);
            Assert.Equal(214.0, total.Potassium_mg);
            Assert.Equal(12.0, total.Calcium_mg);
        }

        [Fact]
        public void Product_Micronutrients_PersistsAndLoadsFromDatabase()
        {
            var p = new Product
            {
                Name = "Апельсин",
                InventoryUnit = "g",
                Micronutrients = new MicronutrientInfo
                {
                    VitaminC_mg = 53.2,
                    Potassium_mg = 181.0,
                    Calcium_mg = 40.0
                }
            };

            int id = _db.InsertProduct(p);
            Assert.True(id > 0);

            var loaded = _db.GetProductById(id);
            Assert.NotNull(loaded);
            Assert.NotNull(loaded.Micronutrients);
            Assert.Equal(53.2, loaded.Micronutrients.VitaminC_mg);
            Assert.Equal(181.0, loaded.Micronutrients.Potassium_mg);
            Assert.Equal(40.0, loaded.Micronutrients.Calcium_mg);
        }

        [Fact]
        public void MessengerExportService_GeneratesCleanTelegramChecklist()
        {
            var service = new MessengerExportService();

            var entries = new[]
            {
                new PlanEntry { Name = "Яблуко", Unit = "g", TotalQty = 500, Packs = 1 }
            };

            var products = new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase)
            {
                ["Яблуко"] = new Product
                {
                    Name = "Яблуко",
                    WeightPerUnitG = 500,
                    Prices = new() { ["АТБ"] = 25.0m }
                }
            };

            var selectedStores = new Dictionary<string, string?> { ["Яблуко"] = "АТБ" };

            var text = service.FormatForMessenger(entries, products, selectedStores);

            Assert.Contains("СПИСОК ЗАКУПОК", text);
            Assert.Contains("АТБ", text);
            Assert.Contains("Яблуко: 500 г", text);
            Assert.Contains("25.00 грн", text);
        }

        [Fact]
        public void DatabaseService_BackupAndRestore_Succeeds()
        {
            var backupPath = Path.Combine(Path.GetTempPath(), $"backup_{Guid.NewGuid():N}.db");
            try
            {
                _db.InsertProduct(new Product { Name = "Тестовий продукт" });
                _db.BackupDatabase(backupPath);

                Assert.True(File.Exists(backupPath));

                // Verify restore
                var restoreDbPath = Path.Combine(Path.GetTempPath(), $"restored_{Guid.NewGuid():N}.db");
                try
                {
                    var restoreDb = new DatabaseService(restoreDbPath);
                    restoreDb.RestoreDatabase(backupPath);

                    var p = restoreDb.GetProductByName("Тестовий продукт");
                    Assert.NotNull(p);
                }
                finally
                {
                    Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                    if (File.Exists(restoreDbPath)) File.Delete(restoreDbPath);
                }
            }
            finally
            {
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                if (File.Exists(backupPath)) File.Delete(backupPath);
            }
        }
    }
}
