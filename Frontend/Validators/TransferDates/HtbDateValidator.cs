using FluentValidation;
using Frontend.Models.TransferDates;
using Helpers;

namespace Frontend.Validators.TransferDates
{
    public class HtbDateValidator : AbstractValidator<HtbDateViewModel>
    {
        public HtbDateValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.HtbDate)
                .SetValidator(new DateValidator());

            RuleFor(x => x.HtbDate)
                .SetValidator(new FutureDateValidator());

            RuleFor(x => x.HtbDate.Date.Day)
                .Custom((day, context) =>
                {
                    if (!context.RootContextData.TryGetValue("TargetDate", out var targetDate)) return;

                    var dateVm = context.InstanceToValidate;
                    if (string.IsNullOrWhiteSpace((string) targetDate))
                    {
                        return;
                    }

                    if (DatesHelper.SourceDateStringIsGreaterThanToTargetDateString(
                            dateVm.HtbDate.DateInputAsString(),
                            (string) targetDate))
                    {
                        context.AddFailure(
                            "The Advisory Board date must be on or before the target date for the transfer");
                    }
                });
        }
    }
}