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
                .NotEmpty()
                .WithMessage("Select Yes or No");
        }
    }
}