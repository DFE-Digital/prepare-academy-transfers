using FluentValidation;
using Dfe.PrepareTransfers.Web.Pages.Transfers;

namespace Dfe.PrepareTransfers.Web.Validators.Transfers
{
    public class ProposedTrustNameValidator : AbstractValidator<ProposedTrustNameModel>
    {
        public ProposedTrustNameValidator()
        {
            RuleFor(x => x.ProposedTrustName)
                .NotEmpty()
                .WithMessage("Enter the Proposed trust name");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}