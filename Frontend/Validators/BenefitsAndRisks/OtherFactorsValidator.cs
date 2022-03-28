using System.Linq;
using FluentValidation;
using Frontend.Models.Benefits;

namespace Frontend.Validators.BenefitsAndRisks
{
    public class OtherFactorsValidator : AbstractValidator<OtherFactorsViewModel>
    {
        public OtherFactorsValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.OtherFactorsVm.Where(o => o.Checked))
                .NotEmpty()
                .WithMessage("Select the risks with this transfer");
        }
    }
}