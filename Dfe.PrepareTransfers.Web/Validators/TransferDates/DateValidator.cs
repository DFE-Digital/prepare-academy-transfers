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
      ClassLevelCascadeMode = CascadeMode.Continue;
      RuleLevelCascadeMode = CascadeMode.Continue;

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

            if (dateVm.UnknownDate || DateIsIncomplete(dateVm.Date, dateVm.IgnoreDayPart)) return;

            if (int.TryParse(dateVm.Date.Year, out var year) &&
                year is >= 2000 and <= 2050) return;

            context.AddFailure("Year must be between 2000 and 2050");
         });
   }

   private void EnsureTheSpecifiedDateIsValid()
   {
      RuleFor(x => x.Date)
         .Custom((_, context) =>
         {
            DateViewModel dateVm = context.InstanceToValidate;

            if (dateVm.UnknownDate || DateIsEmpty(dateVm.Date, dateVm.IgnoreDayPart) ||
                IsAValidDate(dateVm.Date)) return;

            context.AddFailure("Enter a valid date");
         });
   }

   private void EnsureAllDatePartsAreProvided()
   {
      RuleFor(x => x.Date.Day)
         .Custom((_, context) =>
         {
            DateViewModel dateVm = context.InstanceToValidate;

            if (dateVm.UnknownDate || dateVm.IgnoreDayPart) return;

            if (string.IsNullOrWhiteSpace(dateVm.Date.Day))
               context.AddFailure($"The {ErrorDisplayName} must include a day");
         });

      RuleFor(x => x.Date.Month)
         .Custom((_, context) =>
         {
            DateViewModel dateVm = context.InstanceToValidate;

            if (dateVm.UnknownDate) return;

            if (string.IsNullOrWhiteSpace(dateVm.Date.Month))
               context.AddFailure($"The {ErrorDisplayName} must include a month");
         });

      RuleFor(x => x.Date.Year)
         .Custom((_, context) =>
         {
            DateViewModel dateVm = context.InstanceToValidate;

            if (dateVm.UnknownDate) return;

            if (string.IsNullOrWhiteSpace(dateVm.Date.Year))
               context.AddFailure($"The {ErrorDisplayName} must include a year");
         });
   }

   private void EnsureEitherUnknownOrDateIsSpecified()
   {
      RuleFor(x => x.Date)
         .Custom((_, context) =>
         {
            DateViewModel dateVm = context.InstanceToValidate;
            if ((!dateVm.UnknownDate && DateIsEmpty(dateVm.Date, dateVm.IgnoreDayPart)) ||
                (dateVm.UnknownDate && DateIsEmpty(dateVm.Date, dateVm.IgnoreDayPart) is false))
               context.AddFailure($"Enter {ErrorDisplayName} or select I do not know this");
         });
   }

   private static bool DateIsEmpty(DateInputViewModel dateVm, bool ignoreDay)
   {
      return ignoreDay is false && string.IsNullOrWhiteSpace(dateVm.Day) &&
             string.IsNullOrWhiteSpace(dateVm.Month) &&
             string.IsNullOrWhiteSpace(dateVm.Year);
   }

   private static bool DateIsIncomplete(DateInputViewModel dateVm, bool ignoreDay)
   {
      return (ignoreDay is false && string.IsNullOrWhiteSpace(dateVm.Day)) ||
             string.IsNullOrWhiteSpace(dateVm.Month) ||
             string.IsNullOrWhiteSpace(dateVm.Year);
   }

   private static bool IsAValidDate(DateInputViewModel dateVm)
   {
      var dateString =
         DatesHelper.DayMonthYearToDateString(dateVm.Day, dateVm.Month, dateVm.Year);

      return !string.IsNullOrWhiteSpace(dateString) &&
             DateTime.TryParseExact(dateString, "dd/MM/yyyy", null, DateTimeStyles.None, out _);
   }
}
