using System;
using System.Globalization;
using DocumentFormat.OpenXml.Bibliography;
using FluentValidation;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Helpers;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace Frontend.Validators.TransferDates
{
    public class DateValidator : AbstractValidator<DateViewModel>
    {
        public string ErrorDisplayName { get; set; }
        public DateValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Date.Day)
                .Custom((day, context) =>
                {
                    var dateVm = context.InstanceToValidate;
                    if (!dateVm.UnknownDate && DateIsEmpty(dateVm.Date) || dateVm.UnknownDate && !DateIsEmpty(dateVm.Date))
                    {
                        context.AddFailure($"Enter {ErrorDisplayName} or select I do not know this");
                    }
                });
            
            RuleFor(x => x.Date.Day)
              .Custom((day, context) =>
              {
                  var dateVm = context.InstanceToValidate;

                  if (dateVm.UnknownDate)
                  {
                      return;
                  }
                  
                  if (string.IsNullOrEmpty(dateVm.Date.Day))
                  {
                      context.AddFailure(nameof(dateVm.Date.Day),$"The {ErrorDisplayName} must include a day");
                  }
                  if (string.IsNullOrEmpty(dateVm.Date.Month))
                  {
                      context.AddFailure(nameof(dateVm.Date.Month), $"The {ErrorDisplayName} must include a month");
                  }
                  if (string.IsNullOrEmpty(dateVm.Date.Year))
                  {
                      context.AddFailure(nameof(dateVm.Date.Year), $"The {ErrorDisplayName} must include a year");
                  }
              });

            RuleFor(x => x.Date.Day)
                .Custom((day, context) =>
                {
                    var dateVm = context.InstanceToValidate;
                    if (!dateVm.UnknownDate && !IsAValidDate(dateVm.Date))
                    {
                        context.AddFailure("Enter a valid date");
                    }
                });
            RuleFor(x => x.Date.Year)
                .Custom((year, context) =>
                {
                    var dateVm = context.InstanceToValidate;
                    if (!dateVm.UnknownDate && !IsAValidYear(dateVm.Date))
                    {
                        context.AddFailure("Year must be between 2000 and 2050");
                    }
                });
        }

        private static bool DateIsEmpty(DateInputViewModel dateVm) =>
            string.IsNullOrEmpty(dateVm.Day) && string.IsNullOrEmpty(dateVm.Month) && string.IsNullOrEmpty(dateVm.Year);
        
        private bool IsAValidDate(DateInputViewModel dateVm)
        {
            var dateString =
                DatesHelper.DayMonthYearToDateString(dateVm.Day, dateVm.Month, dateVm.Year);
            return !string.IsNullOrEmpty(dateString) && 
                   DateTime.TryParseExact(dateString, "dd/MM/yyyy", null, DateTimeStyles.None, out _);
        }
        private bool IsAValidYear(DateInputViewModel dateVm)
        {
            var date =
                DatesHelper.ParseDateTime(DatesHelper.DayMonthYearToDateString(dateVm.Day, dateVm.Month, dateVm.Year));
            return date.Year >= 2000 && date.Year <= 2050;
        }
    }
}