using FluentValidation;
using Frontend.Pages.Transfers;
using System.Linq;

namespace Frontend.Validators.Transfers
{
    public class IncomingTrustSearchValidator : AbstractValidator<SearchIncomingTrustModel>
    {
        public IncomingTrustSearchValidator()
        {
            RuleFor(x => x.Trusts)
                .Must(x => x.Any())
                .WithMessage("We could not find any trusts matching your search criteria");
        }
    }
}
