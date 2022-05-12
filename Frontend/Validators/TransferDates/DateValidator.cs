using System;
using System.Globalization;
using FluentValidation;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Helpers;

namespace Frontend.Validators.TransferDates
{
    public class DateValidator : AbstractValidator<DateViewModel>
    {

        public string ErrorDisplayName { get; set; }
        public DateValidator()
        {
            CascadeMode = CascadeMode.Stop;
            var dateName = "date";

            RuleFor(x => x.Date.Day)
                .Custom((day, context) =>
                {
                    var dateVm = context.InstanceToValidate;
                    if (!dateVm.UnknownDate && DateIsEmpty(dateVm.Date))
                    {
                        context.AddFailure($"Enter {ErrorDisplayName} or select I do not know this");
                        return;
                    }
                });
            
            RuleFor(x => x.Date.Day)
              .Custom((day, context) =>
              {
                  var dateVm = context.InstanceToValidate;
                  if (!dateVm.UnknownDate && string.IsNullOrEmpty(dateVm.Date.Day))
                  {
                      context.AddFailure(nameof(dateVm.Date.Day),($"The {ErrorDisplayName} must include a day"));
                  }
                  if (!dateVm.UnknownDate && string.IsNullOrEmpty(dateVm.Date.Month))
                  {
                      context.AddFailure(nameof(dateVm.Date.Month), ($"The {ErrorDisplayName} must include a month"));
                  }
                  if (!dateVm.UnknownDate && string.IsNullOrEmpty(dateVm.Date.Year))
                  {
                      context.AddFailure(nameof(dateVm.Date.Year), ($"The {ErrorDisplayName} must include a year"));
                  }

              });


            RuleFor(x => x.Date.Day)
                .Custom((day, context) =>
                {
                    var dateVm = context.InstanceToValidate;
                    if (dateVm.UnknownDate && !DateIsEmpty(dateVm.Date))
                    {
                        context.AddFailure($"Either enter {ErrorDisplayName} or select I do not know this");
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
        }

        private static bool DateIsEmpty(DateInputViewModel dateVm) =>
            (string.IsNullOrEmpty(dateVm.Day) && string.IsNullOrEmpty(dateVm.Month) && string.IsNullOrEmpty(dateVm.Year));
        
        private bool IsAValidDate(DateInputViewModel dateVm)
        {
            var dateString =
                DatesHelper.DayMonthYearToDateString(dateVm.Day, dateVm.Month, dateVm.Year);
            return !string.IsNullOrEmpty(dateString) && 
                   DateTime.TryParseExact(dateString.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None, out _);
        }
    }
}