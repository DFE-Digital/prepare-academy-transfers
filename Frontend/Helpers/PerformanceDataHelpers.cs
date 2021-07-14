using System.Globalization;

namespace Frontend.Helpers
{
    public static class PerformanceDataHelpers
    {
        private const string NoDataText = "no data";
        public static string GetFormattedResult(string result)
        {
            return string.IsNullOrEmpty(result) ? 
                NoDataText : 
                FormatStringAsDouble(result);
        }
        
        private static string FormatStringAsDouble(string result)
        {
            var resultIsDouble = double.TryParse(result, NumberStyles.Number, CultureInfo.InvariantCulture, out var resultAsDouble);
            return resultIsDouble ? $"{resultAsDouble}" : result;
        }
    }
}