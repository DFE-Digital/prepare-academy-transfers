using System;
using System.Globalization;
using Dfe.PrepareTransfers.Helpers;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Models.TransferDates;
using FluentValidation;

namespace Dfe.PrepareTransfers.Web.Validators.TransferDates;

public class DateValidator : AbstractValidator<DateViewModel>
{
   public DateValidator()
   {
      ClassLevelCascadeMode = CascadeMode.Stop;

      EnsureEitherUnknownOrDateIsSpecified();
      EnsureAllDatePartsAreProvided();
      EnsureTheSpecifiedDateIsValid();
      EnsureTheYearIsWithinAcceptableRange();
   }

   public string ErrorDisplayName { get; init; }

   private void EnsureTheYearIsWithinAcceptableRange()
   {
      RuleFor(x => x.Date.Year)
         .Custom((_, context) =>
         {
            DateViewModel dateVm = context.InstanceToValidate;
            if (!dateVm.UnknownDate && !IsAValidYear(dateVm.Date))
               context.AddFailure("Year must be between 2000 and 2050");
         });
   }

   private void EnsureTheSpecifiedDateIsValid()
   {
      RuleFor(x => x.Date.Day)
         .Custom((_, context) =>
         {
            DateViewModel dateVm = context.InstanceToValidate;
            if (!dateVm.UnknownDate && !IsAValidDate(dateVm.Date)) context.AddFailure("Enter a valid date");
         });
   }

   private void EnsureAllDatePartsAreProvided()
   {
      RuleFor(x => x.Date.Day)
         .Custom((_, context) =>
         {
            DateViewModel dateVm = context.InstanceToValidate;

            if (dateVm.UnknownDate) return;

            if (string.IsNullOrEmpty(dateVm.Date.Day))
               context.AddFailure(nameof(dateVm.Date.Day), $"The {ErrorDisplayName} must include a day");

            if (string.IsNullOrEmpty(dateVm.Date.Month))
               context.AddFailure(nameof(dateVm.Date.Month), $"The {ErrorDisplayName} must include a month");

            if (string.IsNullOrEmpty(dateVm.Date.Year))
               context.AddFailure(nameof(dateVm.Date.Year), $"The {ErrorDisplayName} must include a year");
         });
   }

   private void EnsureEitherUnknownOrDateIsSpecified()
   {
      RuleFor(x => x.Date.Day)
         .Custom((_, context) =>
         {
            DateViewModel dateVm = context.InstanceToValidate;
            if ((!dateVm.UnknownDate && DateIsEmpty(dateVm.Date)) || (dateVm.UnknownDate && !DateIsEmpty(dateVm.Date)))
               context.AddFailure($"Enter {ErrorDisplayName} or select I do not know this");
         });
   }

   private static bool DateIsEmpty(DateInputViewModel dateVm)
   {
      return string.IsNullOrEmpty(dateVm.Day) &&
             string.IsNullOrEmpty(dateVm.Month) &&
             string.IsNullOrEmpty(dateVm.Year);
   }

   private static bool IsAValidDate(DateInputViewModel dateVm)
   {
      var dateString =
         DatesHelper.DayMonthYearToDateString(dateVm.Day, dateVm.Month, dateVm.Year);

      return !string.IsNullOrEmpty(dateString) &&
             DateTime.TryParseExact(dateString, "dd/MM/yyyy", null, DateTimeStyles.None, out _);
   }

   private static bool IsAValidYear(DateInputViewModel dateVm)
   {
      DateTime date =
         DatesHelper.ParseDateTime(DatesHelper.DayMonthYearToDateString(dateVm.Day, dateVm.Month, dateVm.Year));

      return date.Year is >= 2000 and <= 2050;
   }
}
