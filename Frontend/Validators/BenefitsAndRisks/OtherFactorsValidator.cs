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
            RuleFor(x => x.OtherFactorsVm)
                .Custom((list, context) =>
                {
                    if (!list.Any(o => o.Checked))
                    {
                        context.AddFailure("Select the risks with this transfer");
                    }
                });
        }
    }
}