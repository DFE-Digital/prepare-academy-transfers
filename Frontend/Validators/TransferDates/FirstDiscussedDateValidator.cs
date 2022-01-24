using FluentValidation;
using Frontend.Models.TransferDates;

namespace Frontend.Validators.TransferDates
{
    public class FirstDiscussedDateValidator : AbstractValidator<FirstDiscussedViewModel>
    {
        public FirstDiscussedDateValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.FirstDiscussed)
                .SetValidator(new DateValidator());

            RuleFor(x => x.FirstDiscussed)
                .SetValidator(new PastDateValidator()
                    {ErrorMessage = "The date the transfer was first discussed with a trust must be in the past"});
        }
    }
}