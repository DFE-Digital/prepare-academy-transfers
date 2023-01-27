using System.Linq;
using FluentValidation;
using Dfe.PrepareTransfers.Web.Models.Benefits;

namespace Dfe.PrepareTransfers.Web.Validators.BenefitsAndRisks
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