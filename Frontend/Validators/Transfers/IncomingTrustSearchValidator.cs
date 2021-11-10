using Data.Models;
using FluentValidation;
using System.Collections.Generic;

namespace Frontend.Validators.Transfers
{
    public class IncomingTrustSearchValidator : AbstractValidator<List<TrustSearchResult>>
    {
        public IncomingTrustSearchValidator()
        {
            RuleFor(x => x)
                .Must(x => x.Count > 0)
                .WithMessage("We could not find any trusts matching your search criteria");
        }
    }
}
