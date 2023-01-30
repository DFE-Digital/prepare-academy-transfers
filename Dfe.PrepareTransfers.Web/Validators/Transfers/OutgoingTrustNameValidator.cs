using FluentValidation;
using Dfe.PrepareTransfers.Web.Pages.Transfers;

namespace Dfe.PrepareTransfers.Web.Validators.Transfers
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