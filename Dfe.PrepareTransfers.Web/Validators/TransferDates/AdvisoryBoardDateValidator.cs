using FluentValidation;
using Dfe.PrepareTransfers.Web.Models.TransferDates;
using Helpers;

namespace Dfe.PrepareTransfers.Web.Validators.TransferDates
{
    public class AdvisoryBoardDateValidator : AbstractValidator<AdvisoryBoardViewModel>
    {
        public AdvisoryBoardDateValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.AdvisoryBoardDate)
                .SetValidator(new DateValidator() {ErrorDisplayName= "Advisory board date"});

            RuleFor(x => x.AdvisoryBoardDate)
                .SetValidator(new FutureDateValidator());

            RuleFor(x => x.AdvisoryBoardDate.Date.Day)
                .Custom((day, context) =>
                {
                    if (!context.RootContextData.TryGetValue("TargetDate", out var targetDate)) return;

                    var dateVm = context.InstanceToValidate;
                    if (string.IsNullOrWhiteSpace((string) targetDate))
                    {
                        return;
                    }

                    if (!dateVm.AdvisoryBoardDate.UnknownDate && DatesHelper.SourceDateStringIsGreaterThanToTargetDateString(
                            dateVm.AdvisoryBoardDate.DateInputAsString(),
                            (string) targetDate))
                    {
                        context.AddFailure(
                            "The Advisory board date must be on or before the target date for the transfer");
                    }
                });
        }
    }
}