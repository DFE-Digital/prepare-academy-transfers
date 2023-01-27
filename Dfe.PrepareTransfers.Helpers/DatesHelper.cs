using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Dfe.PrepareTransfers.Helpers
{
    public static class DatesHelper
    {
        public static string ToShortDate(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }

        public static string DayMonthYearToDateString(string day, string month, string year)
        {
            if (string.IsNullOrEmpty(day) && string.IsNullOrEmpty(month) && string.IsNullOrEmpty(year))
                return null;

            day = string.IsNullOrEmpty(day) ? "" : day.PadLeft(2, '0');
            month = string.IsNullOrEmpty(month) ? "" : month.PadLeft(2, '0');
            year = string.IsNullOrEmpty(year) ? "" : year;

            return $"{day}/{month}/{year}";
        }

        public static List<string> DateStringToDayMonthYear(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return new List<string>() {"", "", ""};
            }

            return dateString.Split("/").ToList();
        }

        public static string FormatDateString(string dateString, bool? hasDate,
            string unKnownDateText = "I do not know this")
        {
            if (hasDate ?? true)
                return DateStringToGovUkDate(dateString);
            return unKnownDateText;
        }

        public static string DateStringToGovUkDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return dateString;
            }

            var splitDate = dateString.Split('-', '/');
            var date = new DateTime(int.Parse(splitDate[2]), int.Parse(splitDate[1]), int.Parse(splitDate[0]));
            return date.ToString("d MMMM yyyy");
        }

        public static bool IsValidDate(string dateString)
        {
            return DateTime.TryParseExact(dateString, "dd/MM/yyyy", null, DateTimeStyles.None, out _);
        }

        public static List<DateTime> GetFirstWorkingDaysOfTheTheMonthForTheNextYear(string startDateString)
        {
            DateTime.TryParseExact(startDateString, "dd/MM/yyyy", null, DateTimeStyles.None, out var date);
            var dates = new List<DateTime>();

            if (DateIsFirstWorkingDayOfTheMonth(date))
            {
                dates.Add(date);
            }

            while (dates.Count < 12)
            {
                date = GetNextFirstWorkingDate(date);

                dates.Add(date);
            }

            return dates;
        }

        private static DateTime GetNextMondayForDate(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                date = date.AddDays(2);
            }

            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }

            return date;
        }

        private static bool DateIsAWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        private static bool DateIsFirstWorkingDayOfTheMonth(DateTime date)
        {
            return !DateIsAWeekend(date) && date.DayOfWeek == DayOfWeek.Monday && date.Day <= 3 ||
                   !DateIsAWeekend(date) && date.Day == 1;
        }

        private static DateTime GetNextFirstWorkingDate(DateTime date)
        {
            if (DateIsAWeekend(date))
            {
                return GetNextMondayForDate(date);
            }

            date = date.AddMonths(1);
            date = new DateTime(date.Year, date.Month, 1);

            if (DateIsAWeekend(date))
            {
                date = GetNextMondayForDate(date);
            }

            return date;
        }

        public static bool SourceDateStringIsGreaterThanToTargetDateString(string sourceDateString,
            string targetDateString)
        {
            if (string.IsNullOrWhiteSpace(sourceDateString))
                throw new ArgumentNullException(nameof(sourceDateString));
            
            var sourceDate = ParseDateTime(sourceDateString);

            if (string.IsNullOrWhiteSpace(targetDateString))
                throw new ArgumentNullException(nameof(targetDateString));
            
            var targetDate = ParseDateTime(targetDateString);

            return sourceDate > targetDate;
        }

        public static DateTime ParseDateTime(string date)
        {
            DateTime.TryParseExact(date, new[] {"dd/MM/yyyy", "dd-MM-yyyy"}, null,
                DateTimeStyles.None, out var parsedDate);
            return parsedDate;
        }
    }
}