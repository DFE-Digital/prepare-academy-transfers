using Data.Models;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Frontend.Validators.Transfers
{
    public class OutgoingTrustSearchValidator : AbstractValidator<List<TrustSearchResult>>
    {
        public OutgoingTrustSearchValidator()
        {
            RuleFor(x => x)
                .Must(x => x.Count > 0 && x.All(r => r.Academies.Count > 0))
                .WithMessage("We could not find any trusts matching your search criteria");
        }
    }
}
