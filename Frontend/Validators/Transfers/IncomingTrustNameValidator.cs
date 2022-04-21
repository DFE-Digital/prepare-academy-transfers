using FluentValidation;
using Frontend.Pages.Transfers;

namespace Frontend.Validators.Transfers
{
    public class IncomingTrustNameValidator : AbstractValidator<SearchIncomingTrustModel>
    {
        public IncomingTrustNameValidator()
        {
            RuleFor(x => x.SearchQuery)
                .NotEmpty()
                .WithMessage("Enter the incoming trust name");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}