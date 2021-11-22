using FluentValidation;
using Frontend.Models.TransferDates;
using Helpers;

namespace Frontend.Validators.TransferDates
{
    public class HtbDateValidator : AbstractValidator<HtbDateViewModel>
    {
        public HtbDateValidator()
        {
            RuleFor(x => x.HtbDate)
                .SetValidator(new DateValidator());

            RuleFor(x => x.HtbDate.Date.Day)
                .Custom((day, context) =>
                {
                    if (!context.RootContextData.TryGetValue("TargetDate", out var targetDate)) return;
                    
                    var dateVm = context.InstanceToValidate;
                    if (DatesHelper.SourceDateStringIsGreaterThanToTargetDateString(
                        dateVm.HtbDate.DateInputAsString(),
                        targetDate as string) == true)
                    {
                        context.AddFailure("The Advisory Board date must be on or before the target date for the transfer");
                    }
                });

        }
    }
}