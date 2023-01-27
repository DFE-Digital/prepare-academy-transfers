using FluentValidation;
using Dfe.PrepareTransfers.Web.Pages.Transfers;

namespace Dfe.PrepareTransfers.Web.Validators.Transfers
{
    public class OutgoingTrustAcademiesValidator : AbstractValidator<OutgoingTrustAcademiesModel>
    {
        public OutgoingTrustAcademiesValidator()
        {
            RuleFor(request => request.SelectedAcademyIds)
                .NotEmpty()
                .WithMessage("Select an academy");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}