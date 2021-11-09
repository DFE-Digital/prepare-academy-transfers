using Data.Models;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Frontend.Validators.Transfers
{
    public class TrustSearchResultValidator : AbstractValidator<List<TrustSearchResult>>
    {
        public TrustSearchResultValidator()
        {
            RuleFor(x => x)
                .Must(x => x.Count > 0 && x.Any(t => t.Academies.Count > 0))
                .WithMessage("We could not find any trusts matching your search criteria");
        }
    }
}
