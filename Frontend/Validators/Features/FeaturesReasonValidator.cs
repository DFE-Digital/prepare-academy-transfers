using Data.Models.Projects;
using FluentValidation;
using Frontend.Pages.Projects.Features;

namespace Frontend.Validators.Features
{
    public class FeaturesReasonValidator : AbstractValidator<Reason>
    {
        public FeaturesReasonValidator()
        {
            RuleFor(x => x.ReasonForTheTransfer)
                .NotNull()
                .NotEqual(TransferFeatures.ReasonForTheTransferTypes.Empty)
                .WithMessage("Select a reason for the transfer");
        }
    }
}