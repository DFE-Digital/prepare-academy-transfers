using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace Frontend.Validators.Transfers
{
    public class OutgoingTrustNameValidator : AbstractValidator<string>
    {
        public OutgoingTrustNameValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .WithMessage("Enter the outgoing trust name");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}