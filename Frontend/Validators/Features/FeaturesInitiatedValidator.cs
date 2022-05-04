using Data.Models.Projects;
using FluentValidation;
using Frontend.Models.Features;

namespace Frontend.Validators.Features
{
    public class FeaturesInitiatedValidator : AbstractValidator<FeaturesInitiatedViewModel>
    {
        public FeaturesInitiatedValidator()
        {
            RuleFor(x => x.WhoInitiated)
                .NotEqual(TransferFeatures.ReasonForTheTransferTypes.Empty)
                .WithMessage("Select who initiated the project");
        }
    }
}
