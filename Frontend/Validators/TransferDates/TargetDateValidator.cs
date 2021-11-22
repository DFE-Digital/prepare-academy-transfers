using FluentValidation;
using Frontend.Models.TransferDates;
using Helpers;

namespace Frontend.Validators.TransferDates
{
    public class TargetDateValidator : AbstractValidator<TargetDateViewModel>
    {
        public TargetDateValidator()
        {
            RuleFor(x => x.TargetDate)
                .SetValidator(new DateValidator());

            RuleFor(x => x.TargetDate.Date.Day)
                .Custom((day, context) =>
                {
                    if (!context.RootContextData.TryGetValue("HtbDate", out var htbDate)) return;

                    var dateVm = context.InstanceToValidate;
                    if (DatesHelper.SourceDateStringIsGreaterThanToTargetDateString(
                        htbDate as string, dateVm.TargetDate.DateInputAsString()) == true)
                    {
                        context.AddFailure(
                            "The target transfer date must be on or after the Advisory Board date");
                    }
                });
        }
    }
}