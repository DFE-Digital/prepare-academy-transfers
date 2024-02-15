using System;
using System.Collections.Generic;
using System.Globalization;

namespace Dfe.PrepareTransfers.Services;

public class DateValidationService
{
   private readonly IDateValidationMessageProvider _messages;

   public DateValidationService(IDateValidationMessageProvider messages)
   {
      _messages = messages ?? new DefaultDateValidationMessageProvider();
   }

   public (bool, string) Validate(string dayInput, string monthInput, string yearInput, string displayName)
   {
      List<string> missingParts = new();

      if (string.IsNullOrWhiteSpace(dayInput)) missingParts.Add("day");
      if (string.IsNullOrWhiteSpace(monthInput)) missingParts.Add("month");
      if (string.IsNullOrWhiteSpace(yearInput)) missingParts.Add("year");

      if (missingParts.Count == 3)
      {
         return (false, _messages.AllMissing(displayName));
      }

      if (missingParts.Count > 0)
      {
         return (false, _messages.SomeMissing(displayName, missingParts));
      }

      bool yearParsed = int.TryParse(yearInput, out int year);
      bool monthParsed = int.TryParse(monthInput, out int month);
      bool dayParsed = int.TryParse(dayInput, out int day);
      (bool, string) validatedDateParts = ValidateDateParts(dayParsed, monthParsed, yearParsed, month, year, day);
      if (validatedDateParts.Item1 is false) return validatedDateParts;

      (bool valid, string message) = _messages.ContextSpecificValidation(day, month, year);
      if (!valid) return (false, message);

      bool validDate = DateTime.TryParseExact($"{yearInput}-{monthInput}-{dayInput}", "yyyy-M-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

      return validDate ? (true, string.Empty) : (false, _messages.DefaultMessage);
   }

   private (bool, string) ValidateDateParts(bool dayParsed, bool monthParsed, bool yearParsed, int month, int year, int day)
   {
      if (!dayParsed || !monthParsed || !yearParsed)
         return (false, _messages.DefaultMessage);

      if (month < 1 || month > 12)
         return (false, _messages.MonthOutOfRange);

      if (year < 2000 || year > 2050)
         return (false, _messages.YearOutOfRange);

      if (day < 1 || day > DateTime.DaysInMonth(year, month))
         return (false, _messages.DayOutOfRange(DateTime.DaysInMonth(year, month)));
      return (true, string.Empty);
   }

   private sealed class DefaultDateValidationMessageProvider : IDateValidationMessageProvider
   {
      public string AllMissing(string displayName)
      {
         return $"Enter a date for the {displayName.ToLower()}";
      }

      public string SomeMissing(string displayName, IEnumerable<string> missingParts)
      {
         return $"{displayName} must include a {string.Join(" and ", missingParts)}";
      }

      public string DefaultMessage => "Enter a date in the correct format";
      public string MonthOutOfRange => "Month must be between 1 and 12";

      public string DayOutOfRange(int daysInMonth)
      {
         return $"Day must be between 1 and {daysInMonth}";
      }
   }
}
