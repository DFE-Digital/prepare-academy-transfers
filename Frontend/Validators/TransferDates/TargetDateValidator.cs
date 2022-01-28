using FluentValidation;
using Frontend.Models.TransferDates;
using Helpers;

namespace Frontend.Validators.TransferDates
{
    public class TargetDateValidator : AbstractValidator<TargetDateViewModel>
    {
        public TargetDateValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.TargetDate)
                .SetValidator(new DateValidator());

            RuleFor(x => x.TargetDate)
                .SetValidator(new FutureDateValidator());

            RuleFor(x => x.TargetDate.Date.Day)
                .Custom((day, context) =>
                {
                    if (!context.RootContextData.TryGetValue("HtbDate", out var htbDate)) return;
                    if (string.IsNullOrWhiteSpace((string) htbDate)) return;

                    var dateVm = context.InstanceToValidate;
                    if (DatesHelper.SourceDateStringIsGreaterThanToTargetDateString(
                            (string) htbDate, dateVm.TargetDate.DateInputAsString()))
                    {
                        context.AddFailure(
                            "The target transfer date must be on or after the Advisory Board date");
                    }
                });
        }
    }
}