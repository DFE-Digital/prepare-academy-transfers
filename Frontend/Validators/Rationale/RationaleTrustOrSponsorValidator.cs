using FluentValidation;
using Frontend.Models.Rationale;

namespace Frontend.Validators.Rationale
{
    public class RationaleTrustOrSponsorValidator : AbstractValidator<RationaleTrustOrSponsorViewModel>
    {
        public RationaleTrustOrSponsorValidator()
        {
            RuleFor(x => x.TrustOrSponsorRationale)
                .NotEmpty()
                .WithMessage("Enter the rationale for the incoming trust or sponsor");
        }
    }
}