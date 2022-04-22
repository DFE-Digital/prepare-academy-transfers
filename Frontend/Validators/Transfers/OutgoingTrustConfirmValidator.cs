using FluentValidation;
using Frontend.Pages.Transfers;

namespace Frontend.Validators.Transfers
{
    public class OutgoingTrustConfirmValidator : AbstractValidator<OutgoingTrustDetailsModel>
    {
        public OutgoingTrustConfirmValidator()
        {
            RuleFor(x => x.TrustId)
                .NotEmpty()
                .WithMessage("Select the outgoing trust");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}