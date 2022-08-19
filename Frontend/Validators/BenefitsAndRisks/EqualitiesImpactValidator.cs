using FluentValidation;
using Frontend.Models.Benefits;

namespace Frontend.Validators.BenefitsAndRisks
{
    public class EqualitiesImpactValidator : AbstractValidator<EqualitiesImpactAssessmentViewModel>
    {
        public EqualitiesImpactValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.EqualitiesImpactAssessmentConsidered)
                .NotNull()
                .WithMessage("Select yes if a equalities impact assessment has been considered");
        }
    }
}
