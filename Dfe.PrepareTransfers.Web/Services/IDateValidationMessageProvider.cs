using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Services;

public interface IDateValidationMessageProvider
{
   public string DefaultMessage => "Enter a date in the correct format";
   public string MonthOutOfRange => "Month must be between 1 and 12";
   public string YearOutOfRange => "Year must be between 2000 and 2050";

   public string AllMissing(string displayName);

   public string SomeMissing(string displayName, IEnumerable<string> missingParts)
   {
      return $"{displayName} must include a {string.Join(" and ", missingParts)}";
   }

   public string DayOutOfRange(int daysInMonth)
   {
      return $"Day must be between 1 and {daysInMonth}";
   }

   /// <summary>
   ///    Allows the implementor to specify an additional validation step that
   ///    will be executed after all others have passed. Will not be executed
   ///    if any preceding step fails.
   /// </summary>
   /// <param name="day">the day of the month</param>
   /// <param name="month">the month of the year</param>
   /// <param name="year">the year</param>
   /// <returns>
   ///    an anonymous tuple containing the result (<see cref="bool" />)
   ///    and, if the step failed, an error message
   /// </returns>
   public (bool, string) ContextSpecificValidation(int day, int month, int year)
   {
      return (true, string.Empty);
   }
}
