using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FoodManager.Common;

namespace FoodManager.Services.Export
{
    public interface IMessengerExportService
    {
        string FormatForMessenger(
            IEnumerable<PlanEntry> planEntries,
            IReadOnlyDictionary<string, Product> products,
            IReadOnlyDictionary<string, string?>? selectedStores = null);
    }

    public class MessengerExportService : IMessengerExportService
    {
        public string FormatForMessenger(
            IEnumerable<PlanEntry> planEntries,
            IReadOnlyDictionary<string, Product> products,
            IReadOnlyDictionary<string, string?>? selectedStores = null)
        {
            var entries = planEntries.Where(p => p.TotalQty > 0).ToList();
            if (entries.Count == 0) return "🛒 Список покупок порожній.";

            var sb = new StringBuilder();
            sb.AppendLine("🛒 СПИСОК ЗАКУПОК (Food Manager)");
            sb.AppendLine($"📅 {DateTime.Now:dd.MM.yyyy HH:mm}");
            sb.AppendLine("──────────────────────");

            // Group by store
            var byStore = new Dictionary<string, List<(PlanEntry Entry, decimal? Cost)>>(StringComparer.OrdinalIgnoreCase);
            decimal grandTotal = 0m;
            int totalItemsCount = 0;

            foreach (var pe in entries)
            {
                products.TryGetValue(pe.Name, out var prod);

                string? chosenStore = null;
                selectedStores?.TryGetValue(pe.Name, out chosenStore);
                if (string.IsNullOrEmpty(chosenStore))
                {
                    var (bestStore, _, _) = prod?.GetCheapestStoreInfo() ?? (null, null, null);
                    chosenStore = bestStore ?? "Будь-який магазин";
                }

                decimal? lineCost = null;
                if (prod != null && prod.Prices.TryGetValue(chosenStore, out var rawPrice))
                {
                    var ppu = prod.PricePerUnitFromStore(chosenStore, rawPrice);
                    if (ppu.HasValue)
                    {
                        lineCost = Math.Round(ppu.Value * pe.TotalQty, 2);
                        grandTotal += lineCost.Value;
                    }
                }

                if (!byStore.ContainsKey(chosenStore))
                {
                    byStore[chosenStore] = new();
                }

                byStore[chosenStore].Add((pe, lineCost));
                totalItemsCount++;
            }

            foreach (var group in byStore.OrderBy(g => g.Key))
            {
                sb.AppendLine();
                sb.AppendLine($"🏪 {group.Key.ToUpper()}:");
                foreach (var (entry, cost) in group.Value)
                {
                    var unitText = StoreConstants.GetDisplayUnit(entry.Unit);
                    var packsNote = entry.Packs.HasValue ? $" ({entry.Packs.Value} упак.)" : "";
                    var costText = cost.HasValue ? $" — {cost.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)} грн" : "";

                    sb.AppendLine($"  ▫️ {entry.Name}: {entry.TotalQty} {unitText}{packsNote}{costText}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("──────────────────────");
            if (grandTotal > 0)
            {
                sb.AppendLine($"💰 Орієнтовна вартість: ~{grandTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)} грн");
            }
            sb.AppendLine($"📦 Всього товарів: {totalItemsCount}");

            return sb.ToString();
        }
    }
}
