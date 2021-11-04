using FluentValidation;
using Frontend.Models;

namespace Frontend.Validators
{
    public class FeaturesValidator : AbstractValidator<FeaturesViewModel>
    {
        public FeaturesValidator()
        {
            var whoInitiatedError = "Select who initiated the project";
            RuleFor(x => x.WhoInitiated).NotEmpty().WithMessage(whoInitiatedError);
        }
    }
}
