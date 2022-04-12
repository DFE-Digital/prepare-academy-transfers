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
        public DateValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Date.Day)
                .Custom((day, context) =>
                {
                    var dateVm = context.InstanceToValidate;
                    if (!dateVm.UnknownDate && DateIsEmpty(dateVm.Date))
                    {
                        context.AddFailure("You must enter the date or confirm that you don't know it " + dateVm.DisplayName);
                    }
                });

            RuleFor(x => x.Date.Day)
                .Custom((day, context) =>
                {
                    var dateVm = context.InstanceToValidate;
                    if (dateVm.UnknownDate && !DateIsEmpty(dateVm.Date))
                    {
                        context.AddFailure("You must either enter the date or select 'I do not know this'");
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