using System;
using System.Globalization;
using FluentValidation;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Helpers;

namespace Frontend.Validators.TransferDates
{
    public class PastDateValidator : AbstractValidator<DateViewModel>
    {
        public PastDateValidator()
        {
            RuleFor(x => x.Date.Day)
                .Custom((day, context) =>
                {
                    var dateVm = context.InstanceToValidate;
                    if (dateVm.UnknownDate)
                    {
                       return;
                    }
                    DateTime.TryParseExact(dateVm.DateInputAsString(), "dd/MM/yyyy", null, DateTimeStyles.None, out var dateTime);
                    if (dateTime.Date > DateTime.Today)
                    {
                        context.AddFailure("You must enter a past date");
                    }
                });
        }
    }
}