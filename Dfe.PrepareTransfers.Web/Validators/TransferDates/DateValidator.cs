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

            if (dateVm.UnknownDate || DateIsIncomplete(dateVm.Date) || IsAValidYear(dateVm.Date)) return;

            context.AddFailure("Year must be between 2000 and 2050");
         });
   }

   private void EnsureTheSpecifiedDateIsValid()
   {
      RuleFor(x => x.Date)
         .Custom((_, context) =>
         {
            DateViewModel dateVm = context.InstanceToValidate;

            if (dateVm.UnknownDate || DateIsEmpty(dateVm.Date) || IsAValidDate(dateVm.Date)) return;

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
            if ((!dateVm.UnknownDate && DateIsEmpty(dateVm.Date)) || (dateVm.UnknownDate && !DateIsEmpty(dateVm.Date)))
               context.AddFailure($"Enter {ErrorDisplayName} or select I do not know this");
         });
   }

   private static bool DateIsEmpty(DateInputViewModel dateVm)
   {
      return string.IsNullOrWhiteSpace(dateVm.Day) &&
             string.IsNullOrWhiteSpace(dateVm.Month) &&
             string.IsNullOrWhiteSpace(dateVm.Year);
   }

   private static bool DateIsIncomplete(DateInputViewModel dateVm)
   {
      return string.IsNullOrWhiteSpace(dateVm.Day) ||
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

   private static bool IsAValidYear(DateInputViewModel dateVm)
   {
      DateTime date =
         DatesHelper.ParseDateTime(DatesHelper.DayMonthYearToDateString(dateVm.Day, dateVm.Month, dateVm.Year));

      return date.Year is >= 2000 and <= 2050;
   }
}
