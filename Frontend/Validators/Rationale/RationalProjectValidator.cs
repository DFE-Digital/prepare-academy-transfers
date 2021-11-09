using FluentValidation;
using Frontend.Models.Rationale;

namespace Frontend.Validators.Rationale
{
    public class RationalProjectValidator : AbstractValidator<RationaleProjectViewModel>
    {
        public RationalProjectValidator()
        {
            RuleFor(x => x.ProjectRationale)
                .NotEmpty()
                .WithMessage("Enter the rationale for the project");
        }
    }
}