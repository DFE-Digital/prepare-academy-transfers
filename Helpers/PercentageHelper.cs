using System.Globalization;

namespace Helpers
{
    public static class PercentageHelper
    {
        public static string CalculatePercentageFromStrings(string value, string total)
        {
            if (!double.TryParse(total, out var totalAsDouble))
                return null;

            if (totalAsDouble == 0)
                return null;

            var cultureToUse = InvariantCultureWithSpaceBeforePercentageSignRemoved();

            if (!double.TryParse(value, out var valueAsDouble))
                return 0.ToString("P1", cultureToUse);

            return (valueAsDouble / totalAsDouble).ToString("P1", cultureToUse);
        }

        public static string DisplayAsPercentage(string value) => !double.TryParse(value, out _) ? value : $"{value}%";

        private static CultureInfo InvariantCultureWithSpaceBeforePercentageSignRemoved()
        {
            if (!(CultureInfo.InvariantCulture.Clone() is CultureInfo cultureInfo))
                return CultureInfo.InvariantCulture;
            
            cultureInfo.NumberFormat.PercentNegativePattern = 1;
            cultureInfo.NumberFormat.PercentPositivePattern = 1;
            
            return cultureInfo;
        }
    }
}