using System;
using System.Globalization;
using FluentValidation;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Helpers;

namespace Frontend.Validators.TransferDates
{
    public class NotPastDateValidator : AbstractValidator<DateViewModel>
    {
        public NotPastDateValidator()
        {
            RuleFor(x => x.Date.Day)
                .Custom((day, context) =>
                {
                    var dateVm = context.InstanceToValidate;
                    DateTime.TryParseExact(dateVm.DateInputAsString(), "dd/MM/yyyy", null, DateTimeStyles.None, out var dateTime);
                    if (dateTime.Date < DateTime.Today)
                    {
                        context.AddFailure("Please enter a future date");
                    }
                });
        }
    }
}