using FluentValidation;
using Frontend.Models.TransferDates;

namespace Frontend.Validators.TransferDates
{
    public class FirstDiscussedDateValidator : AbstractValidator<FirstDiscussedViewModel>
    {
        public FirstDiscussedDateValidator()
        {
            RuleFor(x => x.FirstDiscussed)
                .SetInheritanceValidator(v =>
                {
                    v.Add<DateViewModel>(new DateValidator());
                });
        }
    }
}