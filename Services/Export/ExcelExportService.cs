using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using FoodManager.Common;
using FoodManager.Services.Database;
using FoodManager.Services.Planning;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace FoodManager.Services.Export
{
    /// <summary>
    /// Implements polished, styled Excel workbook generation using EPPlus.
    /// </summary>
    public class ExcelExportService : IExcelExportService
    {
        private readonly IDatabaseService _db;

        static ExcelExportService()
        {
            ExcelPackage.License.SetNonCommercialPersonal("FoodManager");
        }

        public ExcelExportService(IDatabaseService? db = null)
        {
            _db = db ?? DatabaseService.Default;
        }

        public void ExportWeeklyPlan(string filePath, IEnumerable<WeeklyPlanItem> planItems)
        {
            var itemsList = planItems.Where(i => i.Servings > 0).ToList();
            if (itemsList.Count == 0) throw new InvalidOperationException("Немає записів для експорту.");

            var allProducts = _db.GetProducts().ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            var allRecipes = _db.GetRecipes().ToDictionary(r => r.Id);

            using var package = new ExcelPackage(new FileInfo(filePath));
            var ws = package.Workbook.Worksheets.Add("WeeklyPlan");

            // Header row
            string[] headers =
            [
                "День", "Страва", "Порцій", "Калорії (ккал)",
                "Білки (г)", "Жири (г)", "Насичені (г)", "Вуглеводи (г)", "Цукри (г)", "Вартість (грн)"
            ];

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[1, i + 1].Value = headers[i];
            }

            using (var hdr = ws.Cells[1, 1, 1, headers.Length])
            {
                hdr.Style.Font.Bold = true;
                hdr.Style.Fill.PatternType = ExcelFillStyle.Solid;
                hdr.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 230, 241));
                hdr.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            int row = 2;
            var days = itemsList.Select(p => p.Day).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var weekTotals = new NutritionTotals();

            foreach (var day in days)
            {
                var rowsForDay = itemsList.Where(p => string.Equals(p.Day, day, StringComparison.OrdinalIgnoreCase)).ToList();
                var dayTotals = new NutritionTotals();

                foreach (var item in rowsForDay)
                {
                    if (!allRecipes.TryGetValue(item.RecipeId, out var recipe)) continue;

                    var single = recipe.CalculateTotals(allProducts);
                    var itemTotals = single.Multiply(item.Servings);

                    ws.Cells[row, 1].Value = day;
                    ws.Cells[row, 2].Value = recipe.Name;
                    ws.Cells[row, 3].Value = item.Servings;
                    ws.Cells[row, 4].Value = itemTotals.Kcal;
                    ws.Cells[row, 5].Value = itemTotals.Protein;
                    ws.Cells[row, 6].Value = itemTotals.Fats;
                    ws.Cells[row, 7].Value = itemTotals.Saturated;
                    ws.Cells[row, 8].Value = itemTotals.Carbs;
                    ws.Cells[row, 9].Value = itemTotals.Sugars;
                    ws.Cells[row, 10].Value = itemTotals.Cost;

                    ws.Cells[row, 4].Style.Numberformat.Format = "0.0";
                    ws.Cells[row, 5, row, 9].Style.Numberformat.Format = "0.##";
                    ws.Cells[row, 10].Style.Numberformat.Format = "0.00";

                    dayTotals.Add(itemTotals);
                    weekTotals.Add(itemTotals);
                    row++;
                }

                // Subtotal for the day
                dayTotals.Round();
                ws.Cells[row, 1].Value = $"Всього {day}";
                ws.Cells[row, 4].Value = dayTotals.Kcal;
                ws.Cells[row, 5].Value = dayTotals.Protein;
                ws.Cells[row, 6].Value = dayTotals.Fats;
                ws.Cells[row, 7].Value = dayTotals.Saturated;
                ws.Cells[row, 8].Value = dayTotals.Carbs;
                ws.Cells[row, 9].Value = dayTotals.Sugars;
                ws.Cells[row, 10].Value = dayTotals.Cost;

                using (var sub = ws.Cells[row, 1, row, headers.Length])
                {
                    sub.Style.Font.Bold = true;
                    sub.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sub.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));
                }
                ws.Cells[row, 4].Style.Numberformat.Format = "0.0";
                ws.Cells[row, 5, row, 9].Style.Numberformat.Format = "0.##";
                ws.Cells[row, 10].Style.Numberformat.Format = "0.00";
                row++;
            }

            // Total for week
            weekTotals.Round();
            ws.Cells[row, 1].Value = "РАЗОМ ЗА ТИЖДЕНЬ";
            ws.Cells[row, 4].Value = weekTotals.Kcal;
            ws.Cells[row, 5].Value = weekTotals.Protein;
            ws.Cells[row, 6].Value = weekTotals.Fats;
            ws.Cells[row, 7].Value = weekTotals.Saturated;
            ws.Cells[row, 8].Value = weekTotals.Carbs;
            ws.Cells[row, 9].Value = weekTotals.Sugars;
            ws.Cells[row, 10].Value = weekTotals.Cost;

            using (var tot = ws.Cells[row, 1, row, headers.Length])
            {
                tot.Style.Font.Bold = true;
                tot.Style.Fill.PatternType = ExcelFillStyle.Solid;
                tot.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 225, 242));
            }
            ws.Cells[row, 4].Style.Numberformat.Format = "0.0";
            ws.Cells[row, 5, row, 9].Style.Numberformat.Format = "0.##";
            ws.Cells[row, 10].Style.Numberformat.Format = "0.00";

            ws.Cells.AutoFitColumns();
            package.Save();
        }

        public void ExportShoppingPlan(
            string filePath,
            IEnumerable<PlanEntry> planEntries,
            IReadOnlyDictionary<string, Product> products,
            IReadOnlyList<string> stores,
            IReadOnlyDictionary<string, string?>? selectedStores = null)
        {
            var list = planEntries.ToList();
            if (list.Count == 0) throw new InvalidOperationException("Немає записів плану для експорту.");

            using var package = new ExcelPackage(new FileInfo(filePath));
            var ws = package.Workbook.Worksheets.Add("Список покупок");

            // Header columns
            int col = 1;
            ws.Cells[1, col++].Value = "Продукт";
            ws.Cells[1, col++].Value = "Одиниця";
            ws.Cells[1, col++].Value = "Кількість (розрах.)";
            ws.Cells[1, col++].Value = "Упаковок";
            ws.Cells[1, col++].Value = "Обраний магазин";
            ws.Cells[1, col++].Value = "Вартість (грн)";

            foreach (var st in stores)
            {
                ws.Cells[1, col++].Value = $"Ціна в {st}";
            }

            int totalCols = col - 1;
            using (var hdr = ws.Cells[1, 1, 1, totalCols])
            {
                hdr.Style.Font.Bold = true;
                hdr.Style.Fill.PatternType = ExcelFillStyle.Solid;
                hdr.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(226, 239, 218));
                hdr.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            int row = 2;
            decimal totalCost = 0m;

            foreach (var pe in list)
            {
                col = 1;
                ws.Cells[row, col++].Value = pe.Name;
                ws.Cells[row, col++].Value = StoreConstants.GetDisplayUnit(pe.Unit);
                ws.Cells[row, col++].Value = pe.TotalQty;
                ws.Cells[row, col++].Value = pe.Packs.HasValue ? pe.Packs.Value : "";

                products.TryGetValue(pe.Name, out var prod);

                string? selStore = null;
                selectedStores?.TryGetValue(pe.Name, out selStore);
                if (string.IsNullOrEmpty(selStore))
                {
                    var (bestSt, _, _) = prod?.GetCheapestStoreInfo() ?? (null, null, null);
                    selStore = bestSt ?? "";
                }

                ws.Cells[row, col++].Value = selStore;

                // Selected store price calculation
                decimal? lineCost = null;
                if (prod != null && !string.IsNullOrEmpty(selStore) && prod.Prices.TryGetValue(selStore, out var rawPr))
                {
                    var ppu = prod.PricePerUnitFromStore(selStore, rawPr);
                    if (ppu.HasValue)
                    {
                        lineCost = Math.Round(ppu.Value * pe.TotalQty, 2);
                        totalCost += lineCost.Value;
                    }
                }

                ws.Cells[row, col++].Value = lineCost.HasValue ? lineCost.Value : "";

                // Per-store prices
                foreach (var st in stores)
                {
                    if (prod != null && prod.Prices.TryGetValue(st, out var stPrice))
                    {
                        var ppu = prod.PricePerUnitFromStore(st, stPrice);
                        if (ppu.HasValue)
                        {
                            var totalForStore = Math.Round(ppu.Value * pe.TotalQty, 2);
                            ws.Cells[row, col].Value = totalForStore;
                        }
                    }
                    col++;
                }

                ws.Cells[row, 6, row, totalCols].Style.Numberformat.Format = "0.00";
                row++;
            }

            // Total row
            ws.Cells[row, 1].Value = "РАЗОМ";
            ws.Cells[row, 6].Value = totalCost;
            using (var tot = ws.Cells[row, 1, row, totalCols])
            {
                tot.Style.Font.Bold = true;
                tot.Style.Fill.PatternType = ExcelFillStyle.Solid;
                tot.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 225, 242));
            }
            ws.Cells[row, 6].Style.Numberformat.Format = "0.00";

            ws.Cells.AutoFitColumns();
            package.Save();
        }
    }
}
