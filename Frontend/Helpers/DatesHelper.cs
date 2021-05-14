using System;
using System.Collections.Generic;
using System.Linq;

namespace Frontend.Helpers
{
    public class DatesHelper
    {
        public static string DayMonthYearToDateString(string day, string month, string year)
        {
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

        public static string DateStringToGovUkDate(string dateString)
        {
            var splitDate = dateString.Split("/");
            var date = new DateTime(int.Parse(splitDate[2]), int.Parse(splitDate[1]), int.Parse(splitDate[0]));
            return date.ToString("d MMMM yyyy");
        }
    }
}