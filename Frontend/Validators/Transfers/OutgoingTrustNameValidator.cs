using FluentValidation;
using Frontend.Pages.Transfers;

namespace Frontend.Validators.Transfers
{
    public class OutgoingTrustNameValidator : AbstractValidator<TrustSearchModel>
    {
        public OutgoingTrustNameValidator()
        {
            RuleFor(request => request.SearchQuery)
                .NotEmpty()
                .WithMessage("Enter the outgoing trust name");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}