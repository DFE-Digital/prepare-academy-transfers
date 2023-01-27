using FluentValidation;
using Dfe.PrepareTransfers.Web.Models.Rationale;

namespace Dfe.PrepareTransfers.Web.Validators.Rationale
{
    public class RationaleProjectValidator : AbstractValidator<RationaleProjectViewModel>
    {
        public RationaleProjectValidator()
        {
            RuleFor(x => x.ProjectRationale)
                .NotEmpty()
                .WithMessage("Enter the rationale for the project");
        }
    }
}