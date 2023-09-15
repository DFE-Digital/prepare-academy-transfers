using System;
using System.Globalization;
using Dfe.PrepareTransfers.Web.Models.TransferDates;
using FluentValidation;

namespace Dfe.PrepareTransfers.Web.Validators.TransferDates;

public class FutureDateValidator : AbstractValidator<DateViewModel>
{
   public FutureDateValidator()
   {
      RuleFor(x => x.Date)
         .Custom((_, context) =>
         {
            DateViewModel dateVm = context.InstanceToValidate;

            if (dateVm.UnknownDate)
            {
                return;
            }

            DateTime.TryParseExact(dateVm.DateInputAsString(), "dd/MM/yyyy", null, DateTimeStyles.None,
               out DateTime dateTime);

            if (dateTime.Date < DateTime.Today)
            {
                context.AddFailure("You must enter a future date");
            }
         });
   }
}
