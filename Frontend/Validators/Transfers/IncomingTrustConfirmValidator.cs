using FluentValidation;
using Frontend.Pages.Transfers;

namespace Frontend.Validators.Transfers
{
    public class IncomingTrustConfirmValidator : AbstractValidator<SearchIncomingTrustModel>
    {
        public IncomingTrustConfirmValidator()
        {
            RuleFor(x => x.SelectedTrustId)
                .NotEmpty()
                .WithMessage("Select a trust");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}