using System;
using FluentValidation;
using Dfe.PrepareTransfers.Web.Models.TransferDates;
using Helpers;

namespace Dfe.PrepareTransfers.Web.Validators.TransferDates
{
    public class PastDateValidator : AbstractValidator<DateViewModel>
    {
        public string ErrorMessage { get; set; }
        
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

                    var dateTime = DatesHelper.ParseDateTime(dateVm.DateInputAsString());
                    if (dateTime.Date > DateTime.Today)
                    {
                        context.AddFailure(ErrorMessage ?? "The date must be in the past");
                    }
                });
        }
    }
    
    
}
