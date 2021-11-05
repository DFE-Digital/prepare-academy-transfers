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
            var whoInitiatedError = "Select who initiated the project";
            RuleFor(x => x.WhoInitiated).NotEqual(TransferFeatures.ProjectInitiators.Empty).WithMessage(whoInitiatedError);
        }
    }
}
