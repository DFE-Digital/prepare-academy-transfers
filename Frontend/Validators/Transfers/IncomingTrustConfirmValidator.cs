using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace Frontend.Validators.Transfers
{
    public class IncomingTrustConfirmValidator : AbstractValidator<string>
    {
        public IncomingTrustConfirmValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .WithMessage("Select the incoming trust");
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            //Allow Null
        }
    }
}