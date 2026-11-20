using FoodManager.Services.Scrapers;
using Xunit;

namespace FoodManager.Tests
{
    public class PriceParsingTests
    {
        [Theory]
        [InlineData("12.50", 12.50)]
        [InlineData("12,50", 12.50)]
        [InlineData("31.99 грн", 31.99)]
        [InlineData("Ціна: 150,00 ₴", 150.00)]
        [InlineData("45 90", 45.90)]
        [InlineData("45\u00A090", 45.90)]
        [InlineData("99", 99.0)]
        public void ParsePrice_ValidStrings_ParsesCorrectly(string input, decimal expected)
        {
            var result = PriceScraperService.ParsePrice(input);
            Assert.NotNull(result);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("Немає в наявності")]
        public void ParsePrice_InvalidOrEmpty_ReturnsNull(string? input)
        {
            var result = PriceScraperService.ParsePrice(input);
            Assert.Null(result);
        }
    }
}
