using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace FoodManager
{
    /// <summary>
    /// Product entity with nutritional values, per-store prices and packaging details.
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Default package size / weight per piece.
        /// Interpreted in the same unit as <see cref="InventoryUnit"/> ("g" -> grams, "pcs" -> pieces).
        /// </summary>
        public int? WeightPerUnitG { get; set; }

        public NutritionInfo Nutrition { get; set; } = new();
        public MicronutrientInfo Micronutrients { get; set; } = new();
        public Dictionary<string, decimal> Prices { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public List<string> Barcodes { get; set; } = new();
        public Dictionary<string, string> Links { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// "g" (grams) or "pcs" (pieces).
        /// </summary>
        public string InventoryUnit { get; set; } = "g";

        /// <summary>
        /// Store names whose prices must not be automatically overwritten by scrapers.
        /// </summary>
        public HashSet<string> ManualPriceSources { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Per-store package size (store -> pack size).
        /// Interpreted in the same unit as <see cref="InventoryUnit"/>.
        /// </summary>
        public Dictionary<string, int> PackSizes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Returns the pack size for the given store.
        /// Priority: store-specific pack -> default pack (WeightPerUnitG) -> null.
        /// </summary>
        public int? GetPackSizeForStore(string? store)
        {
            if (!string.IsNullOrEmpty(store) && PackSizes != null && PackSizes.TryGetValue(store, out var v) && v > 0)
            {
                return v;
            }

            if (WeightPerUnitG.HasValue && WeightPerUnitG.Value > 0)
            {
                return WeightPerUnitG.Value;
            }

            return null;
        }

        /// <summary>
        /// Computes price per unit (1g or 1 piece) for a specific store.
        /// Returns null if pack size is unknown or zero.
        /// </summary>
        public decimal? PricePerUnitFromStore(string store, decimal storePrice)
        {
            var storePack = GetPackSizeForStore(store);
            if (!storePack.HasValue || storePack.Value <= 0) return null;
            return storePrice / storePack.Value;
        }

        /// <summary>
        /// Normalizes store price to default pack size (WeightPerUnitG).
        /// </summary>
        public decimal? NormalizePriceToDefault(string store, decimal storePrice)
        {
            var storePack = GetPackSizeForStore(store);
            var defaultPack = GetPackSizeForStore(null);
            if (!storePack.HasValue || !defaultPack.HasValue || storePack.Value <= 0) return null;
            return storePrice * defaultPack.Value / storePack.Value;
        }

        /// <summary>
        /// Finds the store with the lowest price per unit and returns (Store, PricePerUnit, StorePrice).
        /// </summary>
        public (string? Store, decimal? PricePerUnit, decimal? StorePrice) GetCheapestStoreInfo()
        {
            string? bestStore = null;
            decimal? bestPricePerUnit = null;
            decimal? bestStorePrice = null;

            if (Prices == null) return (null, null, null);

            foreach (var kvp in Prices)
            {
                var ppu = PricePerUnitFromStore(kvp.Key, kvp.Value);
                if (ppu.HasValue && (!bestPricePerUnit.HasValue || ppu.Value < bestPricePerUnit.Value))
                {
                    bestPricePerUnit = ppu.Value;
                    bestStore = kvp.Key;
                    bestStorePrice = kvp.Value;
                }
            }

            return (bestStore, bestPricePerUnit, bestStorePrice);
        }

        public static Product FromReader(SqliteDataReader r)
        {
            var p = new Product();
            try
            {
                if (HasColumn(r, "id") && !r.IsDBNull(r.GetOrdinal("id")))
                    p.Id = r.GetInt32(r.GetOrdinal("id"));

                if (HasColumn(r, "name") && !r.IsDBNull(r.GetOrdinal("name")))
                    p.Name = r.GetString(r.GetOrdinal("name"));

                if (HasColumn(r, "weight_per_unit_g") && !r.IsDBNull(r.GetOrdinal("weight_per_unit_g")))
                    p.WeightPerUnitG = r.GetInt32(r.GetOrdinal("weight_per_unit_g"));

                if (HasColumn(r, "nutrition_json") && !r.IsDBNull(r.GetOrdinal("nutrition_json")))
                {
                    try { p.Nutrition = JsonSerializer.Deserialize<NutritionInfo>(r.GetString(r.GetOrdinal("nutrition_json"))) ?? new(); }
                    catch { p.Nutrition = new(); }
                }

                if (HasColumn(r, "micronutrients_json") && !r.IsDBNull(r.GetOrdinal("micronutrients_json")))
                {
                    try { p.Micronutrients = JsonSerializer.Deserialize<MicronutrientInfo>(r.GetString(r.GetOrdinal("micronutrients_json"))) ?? new(); }
                    catch { p.Micronutrients = new(); }
                }

                if (HasColumn(r, "price_json") && !r.IsDBNull(r.GetOrdinal("price_json")))
                {
                    try
                    {
                        var dict = JsonSerializer.Deserialize<Dictionary<string, decimal>>(r.GetString(r.GetOrdinal("price_json")));
                        p.Prices = dict != null ? new Dictionary<string, decimal>(dict, StringComparer.OrdinalIgnoreCase) : new(StringComparer.OrdinalIgnoreCase);
                    }
                    catch { p.Prices = new(StringComparer.OrdinalIgnoreCase); }
                }

                if (HasColumn(r, "barcodes_json") && !r.IsDBNull(r.GetOrdinal("barcodes_json")))
                {
                    try { p.Barcodes = JsonSerializer.Deserialize<List<string>>(r.GetString(r.GetOrdinal("barcodes_json"))) ?? new(); }
                    catch { p.Barcodes = new(); }
                }

                if (HasColumn(r, "links_json") && !r.IsDBNull(r.GetOrdinal("links_json")))
                {
                    try
                    {
                        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(r.GetString(r.GetOrdinal("links_json")));
                        p.Links = dict != null ? new Dictionary<string, string>(dict, StringComparer.OrdinalIgnoreCase) : new(StringComparer.OrdinalIgnoreCase);
                    }
                    catch { p.Links = new(StringComparer.OrdinalIgnoreCase); }
                }

                if (HasColumn(r, "manual_sources_json") && !r.IsDBNull(r.GetOrdinal("manual_sources_json")))
                {
                    try
                    {
                        var arr = JsonSerializer.Deserialize<List<string>>(r.GetString(r.GetOrdinal("manual_sources_json"))) ?? new();
                        p.ManualPriceSources = new HashSet<string>(arr, StringComparer.OrdinalIgnoreCase);
                    }
                    catch { p.ManualPriceSources = new(StringComparer.OrdinalIgnoreCase); }
                }

                if (HasColumn(r, "pack_json") && !r.IsDBNull(r.GetOrdinal("pack_json")))
                {
                    try
                    {
                        var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(r.GetString(r.GetOrdinal("pack_json")));
                        p.PackSizes = dict != null ? new Dictionary<string, int>(dict, StringComparer.OrdinalIgnoreCase) : new(StringComparer.OrdinalIgnoreCase);
                    }
                    catch { p.PackSizes = new(StringComparer.OrdinalIgnoreCase); }
                }

                if (HasColumn(r, "inventory_unit") && !r.IsDBNull(r.GetOrdinal("inventory_unit")))
                {
                    p.InventoryUnit = r.GetString(r.GetOrdinal("inventory_unit")) ?? "g";
                }
            }
            catch
            {
                // Best-effort read
            }
            return p;
        }

        private static bool HasColumn(SqliteDataReader r, string name)
        {
            try { var _ = r.GetOrdinal(name); return true; } catch { return false; }
        }
    }
}
