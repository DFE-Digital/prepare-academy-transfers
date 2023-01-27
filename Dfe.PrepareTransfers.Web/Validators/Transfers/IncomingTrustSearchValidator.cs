using FluentValidation;
using Dfe.PrepareTransfers.Web.Pages.Transfers;
using System.Linq;

namespace Dfe.PrepareTransfers.Web.Validators.Transfers
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
