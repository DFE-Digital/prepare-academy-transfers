using FluentValidation;
using Frontend.Models.Rationale;

namespace Frontend.Validators.Rationale
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