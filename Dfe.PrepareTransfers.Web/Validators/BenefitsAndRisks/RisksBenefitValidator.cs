using FluentValidation;
using Dfe.PrepareTransfers.Web.Models.Benefits;

namespace Dfe.PrepareTransfers.Web.Validators.BenefitsAndRisks
{
    public class RisksValidator : AbstractValidator<RisksViewModel>
    {
        public RisksValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.RisksInvolved)
                .NotNull()
                .WithMessage("Select yes if there are risks to consider");
        }
    }
}