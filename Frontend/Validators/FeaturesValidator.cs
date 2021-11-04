using Data.Models.Projects;
using FluentValidation;
using Frontend.Models;

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

    public class FeaturesReasonValidator : AbstractValidator<FeaturesReasonViewModel>
    {
        public FeaturesReasonValidator()
        {
            var subjectToInterventionError = "Select whether or not the transfer is subject to intervention";
            RuleFor(x => x.IsSubjectToIntervention).NotNull().WithMessage(subjectToInterventionError);
        }
    }
}
