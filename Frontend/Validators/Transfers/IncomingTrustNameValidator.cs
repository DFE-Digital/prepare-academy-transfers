using FluentValidation;

namespace Frontend.Validators.Transfers
{
    public class IncomingTrustNameValidator : AbstractValidator<string>
    {
        public IncomingTrustNameValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .WithMessage("Enter the incoming trust name");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}