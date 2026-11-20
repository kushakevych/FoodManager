using System;

namespace FoodManager
{
    /// <summary>
    /// Metadata header for an inventory recount session.
    /// </summary>
    public class RecountSession
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Note { get; set; } = string.Empty;
    }

    /// <summary>
    /// Line item recorded in an inventory recount session.
    /// </summary>
    public class RecountItemRow
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
        public int Delta { get; set; }
        public string Unit { get; set; } = "g";
    }

    /// <summary>
    /// DTO for performing a bulk recount item adjustment.
    /// </summary>
    public record BulkRecountItem(int ProductId, int OldQuantity, int NewQuantity, string Unit);

    /// <summary>
    /// Summary of current inventory state for a product.
    /// </summary>
    public class InventorySummary
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = "g";
        public string DisplayText => Unit == "pcs" ? $"{Quantity} шт" : $"{Quantity} г";
    }
}
