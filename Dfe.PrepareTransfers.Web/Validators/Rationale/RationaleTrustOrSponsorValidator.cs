using FluentValidation;
using Dfe.PrepareTransfers.Web.Models.Rationale;

namespace Dfe.PrepareTransfers.Web.Validators.Rationale
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