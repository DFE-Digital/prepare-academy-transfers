using FluentValidation;

namespace Frontend.Validators.Transfers
{
    public class IncomingTrustConfirmValidator : AbstractValidator<string>
    {
        public IncomingTrustConfirmValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .WithMessage("Select an incoming trust");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}