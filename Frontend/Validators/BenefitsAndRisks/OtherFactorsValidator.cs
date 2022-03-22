using FluentValidation;
using Frontend.Models.Benefits;

namespace Frontend.Validators.BenefitsAndRisks
{
    public class OtherFactorsValidator : AbstractValidator<OtherFactorsViewModel>
    {
        public OtherFactorsValidator()
        {
            RuleForEach(x => x.OtherFactorsVm)
                .SetValidator(new OtherFactorsItemValidator());
        }
      
    }
}