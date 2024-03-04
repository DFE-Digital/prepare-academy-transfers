using FluentValidation;
using Dfe.PrepareTransfers.Web.Pages.Projects.AcademyAndTrustInformation;

namespace Dfe.PrepareTransfers.Web.Validators.Transfers
{
    public class EditIncomingTrustNameValidator : AbstractValidator<IncomingTrustNameModel>
    {
        public EditIncomingTrustNameValidator()
        {
            RuleFor(x => x.IncomingTrustName)
                .NotEmpty()
                .WithMessage("Enter the Incoming trust name");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}