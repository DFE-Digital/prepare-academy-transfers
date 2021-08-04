using System.Globalization;
using System.Linq;
using Data.Models.KeyStagePerformance;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Microsoft.AspNetCore.Html;

namespace Frontend.Helpers
{
    public static class PerformanceDataHelpers
    {
        private const string NoDataText = "no data";

        public static HtmlString GetFormattedHtmlResult(DisadvantagedPupilsResult disadvantagedPupilResult)
        {
            if (string.IsNullOrEmpty(disadvantagedPupilResult?.NotDisadvantaged) &&
                string.IsNullOrEmpty(disadvantagedPupilResult?.Disadvantaged))
                return new HtmlString(NoDataText);

            return new HtmlString(
                $"{GetFormattedResult(disadvantagedPupilResult.NotDisadvantaged)}<br>(disadvantaged {GetFormattedResult(disadvantagedPupilResult.Disadvantaged)})");
        }

        public static string GetFormattedStringResult(DisadvantagedPupilsResult disadvantagedPupilResult)
        {
            if (string.IsNullOrEmpty(disadvantagedPupilResult?.NotDisadvantaged) &&
                string.IsNullOrEmpty(disadvantagedPupilResult?.Disadvantaged))
                return NoDataText;

            return $"{GetFormattedResult(disadvantagedPupilResult.NotDisadvantaged)}\n(disadvantaged {GetFormattedResult(disadvantagedPupilResult.Disadvantaged)})";
        }
        
        public static string GetFormattedResult(string result)
        {
            return string.IsNullOrEmpty(result) ? 
                NoDataText : 
                FormatStringAsDouble(result);
        }

        public static string GetFormattedConfidenceInterval(decimal? lowerConfidenceInterval,
            decimal? upperConfidenceInterval)
        {
            if (lowerConfidenceInterval == null && upperConfidenceInterval == null)
                return NoDataText;

            return $"{lowerConfidenceInterval.ToString()} to {upperConfidenceInterval.ToString()}";
        }

        public static string GetFormattedYear(string year)
        {
            if (string.IsNullOrEmpty(year)) return year;
            var trimmedYear = string.Concat(year.Where(c => !char.IsWhiteSpace(c)));
            return trimmedYear.Contains("-") ? trimmedYear.Replace("-", " - ") : year;
        }

        private static string FormatStringAsDouble(string result)
        {
            var resultIsDouble = double.TryParse(result, NumberStyles.Number, CultureInfo.InvariantCulture, out var resultAsDouble);
            return resultIsDouble ? $"{resultAsDouble}" : result;
        }
    }
}