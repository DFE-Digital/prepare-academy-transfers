using System.Linq;
using FluentValidation;
using Frontend.Models.Benefits;

namespace Frontend.Validators.BenefitsAndRisks
{
    public class RisksValidator : AbstractValidator<RisksViewModel>
    {
        public RisksValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.RisksInvolved)
                .NotNull()
                .WithMessage("Select yes if there are risks to consider");
        }
    }
}