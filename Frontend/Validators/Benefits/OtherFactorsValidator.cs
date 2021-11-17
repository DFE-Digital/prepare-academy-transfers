using System.Linq;
using FluentValidation;
using Frontend.Models.Benefits;

namespace Frontend.Validators.Benefits
{
    public class OtherFactorsValidator : AbstractValidator<OtherFactorsViewModel>
    {
        public OtherFactorsValidator()
        {
            RuleForEach(x => x.OtherFactorsVm)
                .SetValidator(new OtherFactorsItemValidator());
        }
      
    }

    public class OtherFactorsItemValidator : AbstractValidator<OtherFactorsItemViewModel>
    {
        public OtherFactorsItemValidator()
        {
            RuleFor(i => i.Description)
                .NotEmpty()
                .When(i => i.Checked)
                .WithMessage("Specify the concern further");
        }
    }
}