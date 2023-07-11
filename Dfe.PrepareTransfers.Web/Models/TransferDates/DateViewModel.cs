using System.Collections.Generic;
using Dfe.PrepareTransfers.Helpers;
using Dfe.PrepareTransfers.Web.Models.Forms;

namespace Dfe.PrepareTransfers.Web.Models.TransferDates;

public class DateViewModel
{
   public DateInputViewModel Date { get; set; }
   public bool UnknownDate { get; set; }

   /// <summary>
   ///    For use in cases where the user is not prompted for the day of the month, instructs
   ///    validation to not report a missing day field.
   /// </summary>
   public bool IgnoreDayPart { get; set; }

   public string DateInputAsString()
   {
      if (string.IsNullOrWhiteSpace(Date?.Day) && string.IsNullOrWhiteSpace(Date?.Month) && string.IsNullOrWhiteSpace(Date?.Year))
      {
          return null;
      }

      var day = string.IsNullOrWhiteSpace(Date.Day) ? "" : Date.Day.PadLeft(2, '0');
      var month = string.IsNullOrWhiteSpace(Date.Month) ? "" : Date.Month.PadLeft(2, '0');
      var year = string.IsNullOrWhiteSpace(Date.Year) ? "" : Date.Year;

      return $"{day}/{month}/{year}";
   }

   public static DateInputViewModel SplitDateIntoDayMonthYear(string dateAsString)
   {
      List<string> splitDate = DatesHelper.DateStringToDayMonthYear(dateAsString);
      return new DateInputViewModel { Day = splitDate[0], Month = splitDate[1], Year = splitDate[2] };
   }
}
