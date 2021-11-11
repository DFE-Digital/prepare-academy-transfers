using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace Frontend.Validators.Transfers
{
    public class OutgoingTrustConfirmValidator : AbstractValidator<string>
    {
        public OutgoingTrustConfirmValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .WithMessage("Select the outgoing trust");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}