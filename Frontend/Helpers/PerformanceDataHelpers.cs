using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Data.Models.KeyStagePerformance;
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

            return
                $"{GetFormattedResult(disadvantagedPupilResult.NotDisadvantaged)}\n(disadvantaged {GetFormattedResult(disadvantagedPupilResult.Disadvantaged)})";
        }

        public static string GetFormattedResult(string result)
        {
            return string.IsNullOrEmpty(result) ? NoDataText : FormatStringAsDouble(result);
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
            return trimmedYear.Contains("-") ? trimmedYear.Replace("-", " to ") : year;
        }

        public static bool HasKeyStage2PerformanceInformation(IList<KeyStage2> keyStage2Results)
        {
            return keyStage2Results != null &&
                   keyStage2Results.Any(result => HasValue(result.MathsProgressScore)
                                                                           || HasValue(result.ReadingProgressScore)
                                                                           || HasValue(result.WritingProgressScore)
                                                                           || HasValue(result
                                                                               .PercentageAchievingHigherStdInRWM)
                                                                           || HasValue(result
                                                                               .PercentageMeetingExpectedStdInRWM));
        }

        public static bool HasKeyStage4PerformanceInformation(IList<KeyStage4> keyStage4Results)
        {
            return keyStage4Results != null &&
                   keyStage4Results.Any(result =>
                       HasValue(result.SipAttainment8score)
                       || HasValue(result.SipAttainment8scoreebacc)
                       || HasValue(result.SipAttainment8scoreenglish)
                       || HasValue(result.SipAttainment8scoremaths)
                       || HasValue(result.SipAttainment8score)
                       || HasValue(result.SipProgress8ebacc)
                       || HasValue(result.SipProgress8english)
                       || HasValue(result.SipProgress8maths)
                       || HasValue(result.SipProgress8Score)
                       || HasValue(result.SipNumberofpupilsprogress8));
        }

        public static bool HasKeyStage5PerformanceInformation(IList<KeyStage5> keyStage5Results)
        {
            return keyStage5Results != null &&
                   keyStage5Results.Any(ks5 =>
                       ks5.Academy != null && (
                           !string.IsNullOrEmpty(ks5.Academy.AcademicAverage) ||
                           !string.IsNullOrEmpty(ks5.Academy.AppliedGeneralAverage)
                       )
                   );
        }

        private static bool HasValue(DisadvantagedPupilsResult disadvantagedPupilResult)
        {
            return !string.IsNullOrEmpty(disadvantagedPupilResult.Disadvantaged) ||
                   !string.IsNullOrEmpty(disadvantagedPupilResult.NotDisadvantaged);
        }

        private static string FormatStringAsDouble(string result)
        {
            var resultIsDouble = double.TryParse(result, NumberStyles.Number, CultureInfo.InvariantCulture,
                out var resultAsDouble);
            return resultIsDouble ? $"{resultAsDouble}" : result;
        }
    }
}