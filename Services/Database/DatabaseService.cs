using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace FoodManager.Services.Database
{
    /// <summary>
    /// SQLite-backed database service implementation.
    /// Provides thread-safe, parameterized queries and transaction management.
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private static readonly Lazy<DatabaseService> _instance = new(() => new DatabaseService());
        public static DatabaseService Default => _instance.Value;

        private readonly string _dbPath;
        private readonly string _connectionString;

        public DatabaseService(string? customDbPath = null)
        {
            _dbPath = customDbPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "food.db");
            _connectionString = $"Data Source={_dbPath}";
        }

        public string DbPath => _dbPath;
        public string ConnectionString => _connectionString;

        private SqliteConnection CreateOpenConnection()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            return conn;
        }

        public void EnsureDatabase()
        {
            var needCreate = !File.Exists(_dbPath);
            if (needCreate)
            {
                using var conn = CreateOpenConnection();
                using var cmd = conn.CreateCommand();

                cmd.CommandText = @"
CREATE TABLE products (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    weight_per_unit_g INTEGER,
    nutrition_json TEXT,
    price_json TEXT,
    barcodes_json TEXT,
    links_json TEXT,
    manual_sources_json TEXT,
    pack_json TEXT,
    inventory_unit TEXT DEFAULT 'g',
    micronutrients_json TEXT
);

CREATE TABLE recipes (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    status TEXT,
    ingredients_json TEXT
);

CREATE TABLE inventory (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    product_id INTEGER NOT NULL,
    quantity INTEGER NOT NULL,
    unit TEXT DEFAULT 'g',
    FOREIGN KEY(product_id) REFERENCES products(id)
);

CREATE TABLE recounts (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    created_at TEXT NOT NULL,
    note TEXT
);

CREATE TABLE recount_items (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    recount_id INTEGER NOT NULL,
    product_id INTEGER NOT NULL,
    old_quantity INTEGER NOT NULL,
    new_quantity INTEGER NOT NULL,
    delta INTEGER NOT NULL,
    unit TEXT NOT NULL,
    FOREIGN KEY(recount_id) REFERENCES recounts(id),
    FOREIGN KEY(product_id) REFERENCES products(id)
);

CREATE INDEX IF NOT EXISTS idx_inventory_product_id ON inventory(product_id);
CREATE INDEX IF NOT EXISTS idx_recount_items_recount_id ON recount_items(recount_id);
CREATE INDEX IF NOT EXISTS idx_products_name ON products(name);
";
                cmd.ExecuteNonQuery();

                // Initial sample product
                using var insert = conn.CreateCommand();
                insert.CommandText = @"
INSERT INTO products (name, weight_per_unit_g, nutrition_json, price_json, barcodes_json, links_json, manual_sources_json, pack_json, inventory_unit)
VALUES (@n, @w, @nut, @pr, @bc, @lnk, @ms, @pack, @unit);";
                insert.Parameters.AddWithValue("@n", "Яблуко");
                insert.Parameters.AddWithValue("@w", 150);
                insert.Parameters.AddWithValue("@nut", JsonSerializer.Serialize(new
                {
                    carbs_100g = 13.8,
                    sugars_100g = 10.4,
                    fats_100g = 0.2,
                    saturated_100g = 0.0,
                    protein_100g = 0.3,
                    kcal_100g = 52.0,
                    fiber_100g = 2.4
                }));
                insert.Parameters.AddWithValue("@pr", JsonSerializer.Serialize(new Dictionary<string, decimal> { { "MarketA", 12.5M }, { "MarketB", 11.0M } }));
                insert.Parameters.AddWithValue("@bc", JsonSerializer.Serialize(new[] { "1234567890123" }));
                insert.Parameters.AddWithValue("@lnk", JsonSerializer.Serialize(new Dictionary<string, string>()));
                insert.Parameters.AddWithValue("@ms", JsonSerializer.Serialize(new List<string>()));
                insert.Parameters.AddWithValue("@pack", JsonSerializer.Serialize(new Dictionary<string, int>()));
                insert.Parameters.AddWithValue("@unit", "g");
                insert.ExecuteNonQuery();

                // Initial inventory
                using var insert2 = conn.CreateCommand();
                insert2.CommandText = "INSERT INTO inventory (product_id, quantity, unit) VALUES (1, 1000, 'g');";
                insert2.ExecuteNonQuery();

                // Initial recipe
                using var r = conn.CreateCommand();
                r.CommandText = "INSERT INTO recipes (name, status, ingredients_json) VALUES (@n, @s, @ing);";
                r.Parameters.AddWithValue("@n", "Яблучний салат");
                r.Parameters.AddWithValue("@s", "перекус");
                var ing = new Dictionary<string, IngredientAmount> { { "Яблуко", new IngredientAmount { Quantity = 150, Unit = "g" } } };
                r.Parameters.AddWithValue("@ing", JsonSerializer.Serialize(ing));
                r.ExecuteNonQuery();
            }
            else
            {
                // Run schema migrations and create indexes if missing
                try
                {
                    using var conn = CreateOpenConnection();

                    void TryExecute(string sql)
                    {
                        try
                        {
                            using var c = conn.CreateCommand();
                            c.CommandText = sql;
                            c.ExecuteNonQuery();
                        }
                        catch { /* column or table already exists */ }
                    }

                    TryExecute("ALTER TABLE products ADD COLUMN inventory_unit TEXT DEFAULT 'g'");
                    TryExecute("ALTER TABLE products ADD COLUMN links_json TEXT");
                    TryExecute("ALTER TABLE products ADD COLUMN manual_sources_json TEXT");
                    TryExecute("ALTER TABLE products ADD COLUMN pack_json TEXT");
                    TryExecute("ALTER TABLE products ADD COLUMN micronutrients_json TEXT");
                    TryExecute("ALTER TABLE inventory ADD COLUMN unit TEXT DEFAULT 'g'");

                    TryExecute(@"
CREATE TABLE IF NOT EXISTS recounts (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    created_at TEXT NOT NULL,
    note TEXT
);");

                    TryExecute(@"
CREATE TABLE IF NOT EXISTS recount_items (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    recount_id INTEGER NOT NULL,
    product_id INTEGER NOT NULL,
    old_quantity INTEGER NOT NULL,
    new_quantity INTEGER NOT NULL,
    delta INTEGER NOT NULL,
    unit TEXT NOT NULL,
    FOREIGN KEY(recount_id) REFERENCES recounts(id),
    FOREIGN KEY(product_id) REFERENCES products(id)
);");

                    TryExecute("CREATE INDEX IF NOT EXISTS idx_inventory_product_id ON inventory(product_id);");
                    TryExecute("CREATE INDEX IF NOT EXISTS idx_recount_items_recount_id ON recount_items(recount_id);");
                    TryExecute("CREATE INDEX IF NOT EXISTS idx_products_name ON products(name);");
                }
                catch
                {
                    // Ignore migration issues
                }
            }
        }

        public DataTable QueryTable(string sql)
        {
            var dt = new DataTable();
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            using var reader = cmd.ExecuteReader();
            dt.Load(reader);
            return dt;
        }

        #region Products

        public IEnumerable<Product> GetProducts()
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, name, weight_per_unit_g, nutrition_json, price_json, barcodes_json, links_json, manual_sources_json, pack_json, inventory_unit, micronutrients_json 
                                FROM products 
                                ORDER BY name ASC";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return Product.FromReader(reader);
            }
        }

        public Product? GetProductById(int id)
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, name, weight_per_unit_g, nutrition_json, price_json, barcodes_json, links_json, manual_sources_json, pack_json, inventory_unit, micronutrients_json 
                                FROM products 
                                WHERE id = @id LIMIT 1";
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Product.FromReader(reader) : null;
        }

        public Product? GetProductByName(string name)
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, name, weight_per_unit_g, nutrition_json, price_json, barcodes_json, links_json, manual_sources_json, pack_json, inventory_unit, micronutrients_json 
                                FROM products 
                                WHERE name = @n LIMIT 1";
            cmd.Parameters.AddWithValue("@n", name);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Product.FromReader(reader) : null;
        }

        public int InsertProduct(Product p)
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO products (name, weight_per_unit_g, nutrition_json, price_json, barcodes_json, links_json, manual_sources_json, pack_json, inventory_unit, micronutrients_json) 
VALUES (@n, @w, @nut, @pr, @bc, @lnk, @ms, @pack, @unit, @micro);
SELECT last_insert_rowid();";
            AddProductParameters(cmd, p);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void UpdateProduct(Product p)
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
UPDATE products 
SET name=@n, weight_per_unit_g=@w, nutrition_json=@nut, price_json=@pr, barcodes_json=@bc, 
    links_json=@lnk, manual_sources_json=@ms, pack_json=@pack, inventory_unit=@unit, micronutrients_json=@micro 
WHERE id=@id";
            AddProductParameters(cmd, p);
            cmd.Parameters.AddWithValue("@id", p.Id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteProduct(int id)
        {
            using var conn = CreateOpenConnection();
            using var tran = conn.BeginTransaction();
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tran;
                    cmd.CommandText = "DELETE FROM inventory WHERE product_id=@id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd2 = conn.CreateCommand())
                {
                    cmd2.Transaction = tran;
                    cmd2.CommandText = "DELETE FROM products WHERE id=@id";
                    cmd2.Parameters.AddWithValue("@id", id);
                    cmd2.ExecuteNonQuery();
                }

                tran.Commit();
            }
            catch
            {
                try { tran.Rollback(); } catch { }
                throw;
            }
        }

        private static void AddProductParameters(SqliteCommand cmd, Product p)
        {
            cmd.Parameters.AddWithValue("@n", p.Name);
            cmd.Parameters.AddWithValue("@w", p.WeightPerUnitG.HasValue ? p.WeightPerUnitG.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@nut", JsonSerializer.Serialize(p.Nutrition));
            cmd.Parameters.AddWithValue("@pr", JsonSerializer.Serialize(p.Prices));
            cmd.Parameters.AddWithValue("@bc", JsonSerializer.Serialize(p.Barcodes));
            cmd.Parameters.AddWithValue("@lnk", JsonSerializer.Serialize(p.Links));
            cmd.Parameters.AddWithValue("@ms", JsonSerializer.Serialize(p.ManualPriceSources?.ToList() ?? new List<string>()));
            cmd.Parameters.AddWithValue("@pack", JsonSerializer.Serialize(p.PackSizes ?? new Dictionary<string, int>()));
            cmd.Parameters.AddWithValue("@unit", p.InventoryUnit ?? "g");
            cmd.Parameters.AddWithValue("@micro", JsonSerializer.Serialize(p.Micronutrients ?? new MicronutrientInfo()));
        }

        #endregion

        #region Recipes

        public IEnumerable<Recipe> GetRecipes()
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, name, status, ingredients_json FROM recipes ORDER BY name ASC";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return Recipe.FromReader(reader);
            }
        }

        public Recipe? GetRecipeById(int id)
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, name, status, ingredients_json FROM recipes WHERE id = @id LIMIT 1";
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Recipe.FromReader(reader) : null;
        }

        public int InsertRecipe(Recipe r)
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO recipes (name, status, ingredients_json) 
VALUES (@n, @s, @ing);
SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@n", r.Name);
            cmd.Parameters.AddWithValue("@s", r.Status);
            cmd.Parameters.AddWithValue("@ing", JsonSerializer.Serialize(r.Ingredients));
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void UpdateRecipe(Recipe r)
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE recipes SET name=@n, status=@s, ingredients_json=@ing WHERE id=@id";
            cmd.Parameters.AddWithValue("@n", r.Name);
            cmd.Parameters.AddWithValue("@s", r.Status);
            cmd.Parameters.AddWithValue("@ing", JsonSerializer.Serialize(r.Ingredients));
            cmd.Parameters.AddWithValue("@id", r.Id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteRecipe(int id)
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM recipes WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region Inventory

        public (int quantity, string unit) GetInventoryQuantityForProduct(string productName)
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT p.inventory_unit, COALESCE(SUM(i.quantity), 0) 
FROM products p
LEFT JOIN inventory i ON p.id = i.product_id
WHERE p.name = @n
GROUP BY p.inventory_unit";
            cmd.Parameters.AddWithValue("@n", productName);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var unit = reader.IsDBNull(0) ? "g" : reader.GetString(0);
                var qty = reader.IsDBNull(1) ? 0 : Convert.ToInt32(reader.GetInt64(1));
                return (qty, unit);
            }
            return (0, "g");
        }

        public (int quantity, string unit) GetInventoryQuantityForProductById(int productId)
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT p.inventory_unit, COALESCE(SUM(i.quantity), 0) 
FROM products p
LEFT JOIN inventory i ON p.id = i.product_id
WHERE p.id = @id
GROUP BY p.inventory_unit";
            cmd.Parameters.AddWithValue("@id", productId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var unit = reader.IsDBNull(0) ? "g" : reader.GetString(0);
                var qty = reader.IsDBNull(1) ? 0 : Convert.ToInt32(reader.GetInt64(1));
                return (qty, unit);
            }
            return (0, "g");
        }

        /// <summary>
        /// Retrieves inventory summaries for all products in a single SQL query.
        /// Resolves the N+1 query issue previously affecting the inventory view.
        /// </summary>
        public IReadOnlyList<InventorySummary> GetAllInventorySummaries()
        {
            var list = new List<InventorySummary>();
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT p.id, p.name, COALESCE(p.inventory_unit, 'g'), COALESCE(SUM(i.quantity), 0)
FROM products p
LEFT JOIN inventory i ON p.id = i.product_id
GROUP BY p.id, p.name, p.inventory_unit
ORDER BY p.name ASC";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new InventorySummary
                {
                    ProductId = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    Unit = reader.GetString(2),
                    Quantity = Convert.ToInt32(reader.GetInt64(3))
                });
            }
            return list;
        }

        public void AddInventory(int productId, int quantity, string unit)
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO inventory (product_id, quantity, unit) VALUES (@pid, @q, @u)";
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@q", quantity);
            cmd.Parameters.AddWithValue("@u", unit ?? "g");
            cmd.ExecuteNonQuery();
        }

        public void ReduceInventory(int productId, int quantity, string unit)
        {
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO inventory (product_id, quantity, unit) VALUES (@pid, @q, @u)";
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@q", -quantity);
            cmd.Parameters.AddWithValue("@u", unit ?? "g");
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region Recounts

        public void PerformRecount(int productId, int newQuantity, string unit, string? note = null)
        {
            var (oldQty, dbUnit) = GetInventoryQuantityForProductById(productId);
            var usedUnit = string.IsNullOrEmpty(unit) ? dbUnit : unit;
            var item = new BulkRecountItem(productId, oldQty, newQuantity, usedUnit);
            PerformBulkRecount(new[] { item }, note);
        }

        public void PerformBulkRecount((int productId, int oldQty, int newQty, string unit)[] items, string? note = null)
        {
            if (items == null || items.Length == 0) return;
            var bulkItems = items.Select(i => new BulkRecountItem(i.productId, i.oldQty, i.newQty, i.unit)).ToArray();
            PerformBulkRecount(bulkItems, note);
        }

        public void PerformBulkRecount(BulkRecountItem[] items, string? note = null)
        {
            if (items == null || items.Length == 0) return;

            using var conn = CreateOpenConnection();
            using var tran = conn.BeginTransaction();
            try
            {
                long recountId;
                using (var c = conn.CreateCommand())
                {
                    c.Transaction = tran;
                    c.CommandText = "INSERT INTO recounts (created_at, note) VALUES (@ts, @note); SELECT last_insert_rowid();";
                    c.Parameters.AddWithValue("@ts", DateTime.UtcNow.ToString("o"));
                    c.Parameters.AddWithValue("@note", note ?? string.Empty);
                    recountId = Convert.ToInt64(c.ExecuteScalar());
                }

                foreach (var it in items)
                {
                    var delta = it.NewQuantity - it.OldQuantity;
                    using (var ci = conn.CreateCommand())
                    {
                        ci.Transaction = tran;
                        ci.CommandText = @"
INSERT INTO recount_items (recount_id, product_id, old_quantity, new_quantity, delta, unit)
VALUES (@rid, @pid, @old, @new, @delta, @unit)";
                        ci.Parameters.AddWithValue("@rid", recountId);
                        ci.Parameters.AddWithValue("@pid", it.ProductId);
                        ci.Parameters.AddWithValue("@old", it.OldQuantity);
                        ci.Parameters.AddWithValue("@new", it.NewQuantity);
                        ci.Parameters.AddWithValue("@delta", delta);
                        ci.Parameters.AddWithValue("@unit", it.Unit ?? "g");
                        ci.ExecuteNonQuery();
                    }

                    if (delta != 0)
                    {
                        using var adj = conn.CreateCommand();
                        adj.Transaction = tran;
                        adj.CommandText = "INSERT INTO inventory (product_id, quantity, unit) VALUES (@pid, @q, @unit)";
                        adj.Parameters.AddWithValue("@pid", it.ProductId);
                        adj.Parameters.AddWithValue("@q", delta);
                        adj.Parameters.AddWithValue("@unit", it.Unit ?? "g");
                        adj.ExecuteNonQuery();
                    }
                }

                tran.Commit();
            }
            catch
            {
                try { tran.Rollback(); } catch { }
                throw;
            }
        }

        public List<RecountSession> GetRecountSessions()
        {
            var list = new List<RecountSession>();
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, created_at, note FROM recounts ORDER BY created_at DESC, id DESC";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new RecountSession
                {
                    Id = r.GetInt64(0),
                    CreatedAt = DateTime.TryParse(r.GetString(1), out var dt) ? dt : DateTime.MinValue,
                    Note = r.IsDBNull(2) ? string.Empty : r.GetString(2)
                });
            }
            return list;
        }

        public List<RecountItemRow> GetRecountItemsByRecountId(long recountId)
        {
            var list = new List<RecountItemRow>();
            using var conn = CreateOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT ri.product_id, COALESCE(p.name, ''), ri.old_quantity, ri.new_quantity, ri.delta, ri.unit
FROM recount_items ri
LEFT JOIN products p ON ri.product_id = p.id
WHERE ri.recount_id = @rid
ORDER BY p.name ASC";
            cmd.Parameters.AddWithValue("@rid", recountId);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new RecountItemRow
                {
                    ProductId = r.IsDBNull(0) ? 0 : r.GetInt32(0),
                    ProductName = r.GetString(1),
                    OldQuantity = r.IsDBNull(2) ? 0 : r.GetInt32(2),
                    NewQuantity = r.IsDBNull(3) ? 0 : r.GetInt32(3),
                    Delta = r.IsDBNull(4) ? 0 : r.GetInt32(4),
                    Unit = r.IsDBNull(5) ? "g" : r.GetString(5)
                });
            }
            return list;
        }

        #endregion

        #region Backup & Restore

        public void BackupDatabase(string targetFilePath)
        {
            if (string.IsNullOrWhiteSpace(targetFilePath)) throw new ArgumentException("Шлях до файлу резервної копії не може бути порожнім.", nameof(targetFilePath));

            // Run checkpoint to ensure all WAL transactions are flushed to disk
            using (var conn = CreateOpenConnection())
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "PRAGMA wal_checkpoint(FULL);";
                try { cmd.ExecuteNonQuery(); } catch { }
            }

            File.Copy(_dbPath, targetFilePath, overwrite: true);
        }

        public void RestoreDatabase(string sourceFilePath)
        {
            if (!File.Exists(sourceFilePath)) throw new FileNotFoundException("Файл резервної копії не знайдено.", sourceFilePath);

            SqliteConnection.ClearAllPools();
            File.Copy(sourceFilePath, _dbPath, overwrite: true);
            EnsureDatabase();
        }

        #endregion
    }
}
