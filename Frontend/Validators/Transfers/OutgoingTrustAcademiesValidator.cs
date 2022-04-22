using FluentValidation;
using Frontend.Pages.Transfers;

namespace Frontend.Validators.Transfers
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