using Data.Models.Projects;
using FluentValidation;
using Frontend.Models;
using Frontend.Models.Features;

namespace Frontend.Validators
{
    public class FeaturesInitiatedValidator : AbstractValidator<FeaturesInitiatedViewModel>
    {
        public FeaturesInitiatedValidator()
        {
            RuleFor(x => x.WhoInitiated)
                .NotEqual(TransferFeatures.ProjectInitiators.Empty)
                .WithMessage("Select who initiated the project");
        }
    }
}
