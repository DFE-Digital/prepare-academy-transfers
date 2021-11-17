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
                .NotEmpty()
                .When(a => a.UnknownDate is false)
                .When(a => string.IsNullOrEmpty(a.Date.Month))
                .When(a => string.IsNullOrEmpty(a.Date.Year))
                .WithMessage("You must enter the date or confirm that you don't know it");

            RuleFor(x => x.Date.Day)
                .Must(a => false)
                .When(a => !IsAValidDate(a.Date))
                .WithMessage("Enter a valid date");
            
            RuleFor(x => x.Date.Day)
                .Must(a => false)
                .When(a => a.UnknownDate)
                .When(a => IsAValidDate(a.Date))
                .WithMessage("You must either enter the date or select 'I do not know this'");

        }

        private bool IsAValidDate(DateInputViewModel dateVm)
        {
            var dateString =
                DatesHelper.DayMonthYearToDateString(dateVm.Day, dateVm.Month, dateVm.Year);
            return !string.IsNullOrEmpty(dateString) && 
                   DateTime.TryParseExact(dateString.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None, out _);
        }
    }
}