using Frontend.Models.Forms;
using Helpers;

namespace Frontend.Models.TransferDates
{
    public class DateViewModel
    {
        public DateInputViewModel Date { get; set; }
        public bool UnknownDate { get; set; }

        public string DateInputAsString()
        {
            if (string.IsNullOrEmpty(Date?.Day) && string.IsNullOrEmpty(Date?.Month) && string.IsNullOrEmpty(Date?.Year))
                return null;
            
            var day = string.IsNullOrEmpty(Date.Day) ? "" : Date.Day.PadLeft(2, '0');
            var month = string.IsNullOrEmpty(Date.Month) ? "" : Date.Month.PadLeft(2, '0');
            var year = string.IsNullOrEmpty(Date.Year) ? "" : Date.Year;

            return $"{day}/{month}/{year}";
        }
        public static DateInputViewModel SplitDateIntoDayMonthYear(string dateAsString)
        {
            var splitDate = DatesHelper.DateStringToDayMonthYear(dateAsString);
            return new DateInputViewModel {Day = splitDate[0], Month = splitDate[1], Year = splitDate[2]};
        }

        public string DisplayName { get; set; }
    }
}