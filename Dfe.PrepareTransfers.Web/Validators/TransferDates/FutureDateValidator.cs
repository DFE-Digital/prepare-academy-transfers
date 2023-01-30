using System;
using System.Globalization;
using FluentValidation;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Models.TransferDates;
using Dfe.PrepareTransfers.Helpers;

namespace Dfe.PrepareTransfers.Web.Validators.TransferDates
{
    public class FutureDateValidator : AbstractValidator<DateViewModel>
    {
        public FutureDateValidator()
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
                    if (dateTime.Date < DateTime.Today)
                    {
                        context.AddFailure("You must enter a future date");
                    }
                });
        }
    }
}