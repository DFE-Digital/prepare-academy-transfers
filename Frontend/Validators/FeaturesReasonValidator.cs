using FluentValidation;
using Frontend.Models.Features;

namespace Frontend.Validators
{
    public class FeaturesReasonValidator : AbstractValidator<FeaturesReasonViewModel>
    {
        public FeaturesReasonValidator()
        {
            RuleFor(x => x.IsSubjectToIntervention)
                .NotNull()
                .WithMessage("Select whether or not the transfer is subject to intervention");
        }
    }
}