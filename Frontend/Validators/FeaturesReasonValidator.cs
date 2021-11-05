using FluentValidation;
using Frontend.Models.Features;

namespace Frontend.Validators
{
    public class FeaturesReasonValidator : AbstractValidator<FeaturesReasonViewModel>
    {
        public FeaturesReasonValidator()
        {
            var subjectToInterventionError = "Select whether or not the transfer is subject to intervention";
            RuleFor(x => x.IsSubjectToIntervention).NotNull().WithMessage(subjectToInterventionError);
        }
    }
}