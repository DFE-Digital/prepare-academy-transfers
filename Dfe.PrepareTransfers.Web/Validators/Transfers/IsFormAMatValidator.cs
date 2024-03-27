using FluentValidation;
using Dfe.PrepareTransfers.Web.Models;

namespace Dfe.PrepareTransfers.Web.Validators.Transfers
{
    public class IsFormAMatValidator : AbstractValidator<IsFormAMatViewModel>
    {
        public IsFormAMatValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.IsFormAMat)
                .NotNull()
                .WithMessage("Select yes if this transfer result in the forming of a new multi academy trust");
        }
    }
}
