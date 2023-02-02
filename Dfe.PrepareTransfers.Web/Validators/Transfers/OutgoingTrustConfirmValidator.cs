using FluentValidation;
using Dfe.PrepareTransfers.Web.Pages.Transfers;

namespace Dfe.PrepareTransfers.Web.Validators.Transfers
{
    public class OutgoingTrustConfirmValidator : AbstractValidator<OutgoingTrustDetailsModel>
    {
        public OutgoingTrustConfirmValidator()
        {
            RuleFor(x => x.TrustId)
                .NotEmpty()
                .WithMessage("Select a trust");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}