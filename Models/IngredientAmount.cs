namespace FoodManager
{
    /// <summary>
    /// Quantity and measurement unit for a recipe ingredient.
    /// </summary>
    public class IngredientAmount
    {
        public int Quantity { get; set; }
        public string Unit { get; set; } = "g";

        public IngredientAmount Clone() => new() { Quantity = Quantity, Unit = Unit };
    }
}
