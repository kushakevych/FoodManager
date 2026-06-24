using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Text.Json;

namespace FoodManager
{
    public static class DataAccess
    {
        private static string DbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "food.db");
        private static string ConnStr => $"Data Source={DbPath}";

        public static void EnsureDatabase()
        {
            var needCreate = !File.Exists(DbPath);
            if (needCreate)
            {
                using var conn = new SqliteConnection(ConnStr);
                conn.Open();

                var cmd = conn.CreateCommand();

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
    inventory_unit TEXT DEFAULT 'g'
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
";
                cmd.ExecuteNonQuery();

                // sample product
                var insert = conn.CreateCommand();
                insert.CommandText = @"
INSERT INTO products (name, weight_per_unit_g, nutrition_json, price_json, barcodes_json, links_json, manual_sources_json, pack_json, inventory_unit) VALUES (@n, @w, @nut, @pr, @bc, @lnk, @ms, @pack, @unit);
";
                insert.Parameters.AddWithValue("@n", "Яблуко");
                insert.Parameters.AddWithValue("@w", 150); // this is default pack / weight per unit as used now
                insert.Parameters.AddWithValue("@nut", JsonSerializer.Serialize(new
                {
                    carbs_100g = 13.8,
                    sugars_100g = 10.4,
                    fats_100g = 0.2,
                    saturated_100g = 0.0,
                    protein_100g = 0.3,
                    kcal_100g = 52,
                    fiber_100g = 2.4
                }));
                insert.Parameters.AddWithValue("@pr", JsonSerializer.Serialize(new Dictionary<string, decimal> { { "MarketA", 12.5M }, { "MarketB", 11.0M } }));
                insert.Parameters.AddWithValue("@bc", JsonSerializer.Serialize(new string[] { "1234567890123" }));
                insert.Parameters.AddWithValue("@lnk", JsonSerializer.Serialize(new Dictionary<string, string>()));
                insert.Parameters.AddWithValue("@ms", JsonSerializer.Serialize(new List<string>()));
                insert.Parameters.AddWithValue("@pack", JsonSerializer.Serialize(new Dictionary<string, int>()));
                insert.Parameters.AddWithValue("@unit", "g");
                insert.ExecuteNonQuery();

                // initial inventory
                var insert2 = conn.CreateCommand();
                insert2.CommandText = @"
INSERT INTO inventory (product_id, quantity, unit) VALUES (1, 1000, 'g');
";
                insert2.ExecuteNonQuery();

                // sample recipe
                var r = conn.CreateCommand();
                r.CommandText = @"
INSERT INTO recipes (name, status, ingredients_json) VALUES (@n, @s, @ing);
";
                r.Parameters.AddWithValue("@n", "Яблучний салат");
                r.Parameters.AddWithValue("@s", "перекус");
                var ing = new Dictionary<string, IngredientAmount> { { "Яблуко", new IngredientAmount { Quantity = 150, Unit = "g" } } };
                r.Parameters.AddWithValue("@ing", JsonSerializer.Serialize(ing));
                r.ExecuteNonQuery();
            }
            else
            {
                // best-effort migrations: add new columns if absent
                try
                {
                    using var conn = new SqliteConnection(ConnStr);
                    conn.Open();

                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "ALTER TABLE products ADD COLUMN inventory_unit TEXT DEFAULT 'g'";
                    try { cmd.ExecuteNonQuery(); } catch { /* ignore */ }

                    using var cmdAddLinks = conn.CreateCommand();
                    cmdAddLinks.CommandText = "ALTER TABLE products ADD COLUMN links_json TEXT";
                    try { cmdAddLinks.ExecuteNonQuery(); } catch { /* ignore */ }

                    using var cmdAddManual = conn.CreateCommand();
                    cmdAddManual.CommandText = "ALTER TABLE products ADD COLUMN manual_sources_json TEXT";
                    try { cmdAddManual.ExecuteNonQuery(); } catch { /* ignore */ }

                    using var cmdAddPackJson = conn.CreateCommand();
                    cmdAddPackJson.CommandText = "ALTER TABLE products ADD COLUMN pack_json TEXT";
                    try { cmdAddPackJson.ExecuteNonQuery(); } catch { /* ignore */ }

                    using var cmd2 = conn.CreateCommand();
                    cmd2.CommandText = "ALTER TABLE inventory ADD COLUMN unit TEXT DEFAULT 'g'";
                    try { cmd2.ExecuteNonQuery(); } catch { /* ignore */ }

                    using var createRecount = conn.CreateCommand();
                    createRecount.CommandText = @"
CREATE TABLE IF NOT EXISTS recounts (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    created_at TEXT NOT NULL,
    note TEXT
);
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
);
";
                    try { createRecount.ExecuteNonQuery(); } catch { /* ignore */ }
                }
                catch
                {
                    // ignore migration errors
                }
            }
        }

        public static DataTable QueryTable(string sql)
        {
            var dt = new DataTable();
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            using var reader = cmd.ExecuteReader();
            dt.Load(reader);
            return dt;
        }

        public static IEnumerable<Product> GetProducts()
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, name, weight_per_unit_g, nutrition_json, price_json, barcodes_json, links_json, manual_sources_json, pack_json, inventory_unit FROM products";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return Product.FromReader(reader);
            }
        }

        public static Product? GetProductByName(string name)
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, name, weight_per_unit_g, nutrition_json, price_json, barcodes_json, links_json, manual_sources_json, pack_json, inventory_unit FROM products WHERE name = @n LIMIT 1";
            cmd.Parameters.AddWithValue("@n", name);
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) return Product.FromReader(reader);
            return null;
        }

        public static IEnumerable<Recipe> GetRecipes()
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, name, status, ingredients_json FROM recipes";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return Recipe.FromReader(reader);
            }
        }

        public static int InsertProduct(Product p)
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO products (name, weight_per_unit_g, nutrition_json, price_json, barcodes_json, links_json, manual_sources_json, pack_json, inventory_unit) VALUES (@n,@w,@nut,@pr,@bc,@lnk,@ms,@pack,@unit);
SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@n", p.Name);
            cmd.Parameters.AddWithValue("@w", p.WeightPerUnitG.HasValue ? (object)p.WeightPerUnitG.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@nut", JsonSerializer.Serialize(p.Nutrition));
            cmd.Parameters.AddWithValue("@pr", JsonSerializer.Serialize(p.Prices));
            cmd.Parameters.AddWithValue("@bc", JsonSerializer.Serialize(p.Barcodes));
            cmd.Parameters.AddWithValue("@lnk", JsonSerializer.Serialize(p.Links));
            cmd.Parameters.AddWithValue("@ms", JsonSerializer.Serialize(p.ManualPriceSources?.ToList() ?? new List<string>()));
            cmd.Parameters.AddWithValue("@pack", JsonSerializer.Serialize(p.PackSizes ?? new Dictionary<string, int>()));
            cmd.Parameters.AddWithValue("@unit", p.InventoryUnit ?? "g");
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public static void UpdateProduct(Product p)
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE products SET name=@n, weight_per_unit_g=@w, nutrition_json=@nut, price_json=@pr, barcodes_json=@bc, links_json=@lnk, manual_sources_json=@ms, pack_json=@pack, inventory_unit=@unit WHERE id=@id";
            cmd.Parameters.AddWithValue("@n", p.Name);
            cmd.Parameters.AddWithValue("@w", p.WeightPerUnitG.HasValue ? (object)p.WeightPerUnitG.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@nut", JsonSerializer.Serialize(p.Nutrition));
            cmd.Parameters.AddWithValue("@pr", JsonSerializer.Serialize(p.Prices));
            cmd.Parameters.AddWithValue("@bc", JsonSerializer.Serialize(p.Barcodes));
            cmd.Parameters.AddWithValue("@lnk", JsonSerializer.Serialize(p.Links));
            cmd.Parameters.AddWithValue("@ms", JsonSerializer.Serialize(p.ManualPriceSources?.ToList() ?? new List<string>()));
            cmd.Parameters.AddWithValue("@pack", JsonSerializer.Serialize(p.PackSizes ?? new Dictionary<string, int>()));
            cmd.Parameters.AddWithValue("@unit", p.InventoryUnit ?? "g");
            cmd.Parameters.AddWithValue("@id", p.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteProduct(int id)
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM products WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            using var cmd2 = conn.CreateCommand();
            cmd2.CommandText = "DELETE FROM inventory WHERE product_id=@id";
            cmd2.Parameters.AddWithValue("@id", id);
            cmd2.ExecuteNonQuery();
        }

        public static int InsertRecipe(Recipe r)
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO recipes (name, status, ingredients_json) VALUES (@n,@s,@ing);
SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@n", r.Name);
            cmd.Parameters.AddWithValue("@s", r.Status);
            cmd.Parameters.AddWithValue("@ing", JsonSerializer.Serialize(r.Ingredients));
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static void UpdateRecipe(Recipe r)
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE recipes SET name=@n, status=@s, ingredients_json=@ing WHERE id=@id";
            cmd.Parameters.AddWithValue("@n", r.Name);
            cmd.Parameters.AddWithValue("@s", r.Status);
            cmd.Parameters.AddWithValue("@ing", JsonSerializer.Serialize(r.Ingredients));
            cmd.Parameters.AddWithValue("@id", r.Id);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteRecipe(int id)
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM recipes WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public static (int quantity, string unit) GetInventoryQuantityForProduct(string productName)
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT p.inventory_unit, COALESCE(SUM(i.quantity),0) FROM products p
LEFT JOIN inventory i ON p.id = i.product_id
WHERE p.name = @n
GROUP BY p.inventory_unit
";
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

        public static (int quantity, string unit) GetInventoryQuantityForProductById(int productId)
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT p.inventory_unit, COALESCE(SUM(i.quantity),0) FROM products p
LEFT JOIN inventory i ON p.id = i.product_id
WHERE p.id = @id
GROUP BY p.inventory_unit
";
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

        public static void AddInventory(int productId, int quantity, string unit)
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO inventory (product_id, quantity, unit) VALUES (@pid, @q, @u)";
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@q", quantity);
            cmd.Parameters.AddWithValue("@u", unit ?? "g");
            cmd.ExecuteNonQuery();
        }

        public static void ReduceInventory(int productId, int quantity, string unit)
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO inventory (product_id, quantity, unit) VALUES (@pid, @q, @u)";
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@q", -quantity);
            cmd.Parameters.AddWithValue("@u", unit ?? "g");
            cmd.ExecuteNonQuery();
        }

        public static void PerformRecount(int productId, int newQuantity, string unit, string? note = null)
        {
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                int oldQty = 0;
                string dbUnit = "g";
                using (var c = conn.CreateCommand())
                {
                    c.Transaction = tran;
                    c.CommandText = @"SELECT p.inventory_unit, COALESCE(SUM(i.quantity),0) FROM products p
LEFT JOIN inventory i ON p.id = i.product_id
WHERE p.id = @id
GROUP BY p.inventory_unit";
                    c.Parameters.AddWithValue("@id", productId);
                    using var r = c.ExecuteReader();
                    if (r.Read())
                    {
                        dbUnit = r.IsDBNull(0) ? "g" : r.GetString(0);
                        oldQty = r.IsDBNull(1) ? 0 : Convert.ToInt32(r.GetInt64(1));
                    }
                }

                var usedUnit = string.IsNullOrEmpty(unit) ? dbUnit : unit;

                var delta = newQuantity - oldQty;

                long recountId;
                using (var c = conn.CreateCommand())
                {
                    c.Transaction = tran;
                    c.CommandText = "INSERT INTO recounts (created_at, note) VALUES (@ts, @note); SELECT last_insert_rowid();";
                    c.Parameters.AddWithValue("@ts", DateTime.UtcNow.ToString("o"));
                    c.Parameters.AddWithValue("@note", note ?? "");
                    recountId = Convert.ToInt64(c.ExecuteScalar());
                }

                using (var c = conn.CreateCommand())
                {
                    c.Transaction = tran;
                    c.CommandText = @"INSERT INTO recount_items (recount_id, product_id, old_quantity, new_quantity, delta, unit)
VALUES (@rid, @pid, @old, @new, @delta, @unit)";
                    c.Parameters.AddWithValue("@rid", recountId);
                    c.Parameters.AddWithValue("@pid", productId);
                    c.Parameters.AddWithValue("@old", oldQty);
                    c.Parameters.AddWithValue("@new", newQuantity);
                    c.Parameters.AddWithValue("@delta", delta);
                    c.Parameters.AddWithValue("@unit", usedUnit);
                    c.ExecuteNonQuery();
                }

                if (delta != 0)
                {
                    using var c = conn.CreateCommand();
                    c.Transaction = tran;
                    c.CommandText = @"INSERT INTO inventory (product_id, quantity, unit) VALUES (@pid, @q, @unit)";
                    c.Parameters.AddWithValue("@pid", productId);
                    c.Parameters.AddWithValue("@q", delta);
                    c.Parameters.AddWithValue("@unit", usedUnit);
                    c.ExecuteNonQuery();
                }

                tran.Commit();
            }
            catch
            {
                try { tran.Rollback(); } catch { }
                throw;
            }
        }

        public class RecountSession
        {
            public long Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Note { get; set; } = "";
        }

        public class RecountItemRow
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = "";
            public int OldQuantity { get; set; }
            public int NewQuantity { get; set; }
            public int Delta { get; set; }
            public string Unit { get; set; } = "g";
        }

        public static void PerformBulkRecount((int productId, int oldQty, int newQty, string unit)[] items, string? note = null)
        {
            if (items == null || items.Length == 0) return;

            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                long recountId;
                using (var c = conn.CreateCommand())
                {
                    c.Transaction = tran;
                    c.CommandText = "INSERT INTO recounts (created_at, note) VALUES (@ts, @note); SELECT last_insert_rowid();";
                    c.Parameters.AddWithValue("@ts", DateTime.UtcNow.ToString("o"));
                    c.Parameters.AddWithValue("@note", note ?? "");
                    recountId = Convert.ToInt64(c.ExecuteScalar());
                }

                foreach (var it in items)
                {
                    var delta = it.newQty - it.oldQty;
                    using var ci = conn.CreateCommand();
                    ci.Transaction = tran;
                    ci.CommandText = @"INSERT INTO recount_items (recount_id, product_id, old_quantity, new_quantity, delta, unit)
VALUES (@rid, @pid, @old, @new, @delta, @unit)";
                    ci.Parameters.AddWithValue("@rid", recountId);
                    ci.Parameters.AddWithValue("@pid", it.productId);
                    ci.Parameters.AddWithValue("@old", it.oldQty);
                    ci.Parameters.AddWithValue("@new", it.newQty);
                    ci.Parameters.AddWithValue("@delta", delta);
                    ci.Parameters.AddWithValue("@unit", it.unit ?? "g");
                    ci.ExecuteNonQuery();

                    if (delta != 0)
                    {
                        using var adj = conn.CreateCommand();
                        adj.Transaction = tran;
                        adj.CommandText = @"INSERT INTO inventory (product_id, quantity, unit) VALUES (@pid, @q, @unit)";
                        adj.Parameters.AddWithValue("@pid", it.productId);
                        adj.Parameters.AddWithValue("@q", delta);
                        adj.Parameters.AddWithValue("@unit", it.unit ?? "g");
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

        public static List<RecountSession> GetRecountSessions()
        {
            var list = new List<RecountSession>();
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, created_at, note FROM recounts ORDER BY created_at DESC, id DESC";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var s = new RecountSession();
                s.Id = r.GetInt64(0);
                s.CreatedAt = DateTime.TryParse(r.GetString(1), out var dt) ? dt : DateTime.MinValue;
                s.Note = r.IsDBNull(2) ? "" : r.GetString(2);
                list.Add(s);
            }
            return list;
        }

        public static List<RecountItemRow> GetRecountItemsByRecountId(long recountId)
        {
            var list = new List<RecountItemRow>();
            using var conn = new SqliteConnection(ConnStr);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT ri.product_id, p.name, ri.old_quantity, ri.new_quantity, ri.delta, ri.unit
FROM recount_items ri
LEFT JOIN products p ON ri.product_id = p.id
WHERE ri.recount_id = @rid
ORDER BY p.name";
            cmd.Parameters.AddWithValue("@rid", recountId);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var item = new RecountItemRow
                {
                    ProductId = r.IsDBNull(0) ? 0 : r.GetInt32(0),
                    ProductName = r.IsDBNull(1) ? "" : r.GetString(1),
                    OldQuantity = r.IsDBNull(2) ? 0 : r.GetInt32(2),
                    NewQuantity = r.IsDBNull(3) ? 0 : r.GetInt32(3),
                    Delta = r.IsDBNull(4) ? 0 : r.GetInt32(4),
                    Unit = r.IsDBNull(5) ? "g" : r.GetString(5)
                };
                list.Add(item);
            }
            return list;
        }
    }
}