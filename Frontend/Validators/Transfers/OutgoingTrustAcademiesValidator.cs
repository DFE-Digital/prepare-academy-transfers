using FluentValidation;

namespace Frontend.Validators.Transfers
{
    public class OutgoingTrustAcademiesValidator : AbstractValidator<string>
    {
        public OutgoingTrustAcademiesValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .WithMessage("Select an academy");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}