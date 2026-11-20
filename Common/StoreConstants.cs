using System;
using System.Collections.Generic;

namespace FoodManager.Common
{
    /// <summary>
    /// Centralized constants and definitions for supported grocery stores and markets.
    /// </summary>
    public static class StoreConstants
    {
        public const string ATB = "АТБ";
        public const string Silpo = "Сільпо";
        public const string Stolichny = "Столичний ринок";
        public const string Metro = "Метро";
        public const string VelykaKyshenya = "Велика Кишеня";
        public const string Ashan = "Ашан";
        public const string Novus = "NOVUS";

        public const string UnitGrams = "g";
        public const string UnitPieces = "pcs";

        /// <summary>
        /// All supported store identifiers in display order.
        /// </summary>
        public static readonly IReadOnlyList<string> AllStores = new[]
        {
            ATB,
            Silpo,
            Metro,
            Stolichny,
            Ashan,
            VelykaKyshenya,
            Novus
        };

        /// <summary>
        /// Checks whether the specified unit represents grams.
        /// </summary>
        public static bool IsGrams(string? unit) =>
            string.Equals(unit, UnitGrams, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Checks whether the specified unit represents pieces.
        /// </summary>
        public static bool IsPieces(string? unit) =>
            string.Equals(unit, UnitPieces, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Returns display unit text in Ukrainian ("г" or "шт").
        /// </summary>
        public static string GetDisplayUnit(string? unit) =>
            IsPieces(unit) ? "шт" : "г";
    }
}
