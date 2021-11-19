using FluentValidation;
using Frontend.Models.TransferDates;

namespace Frontend.Validators.TransferDates
{
    public class HtbDateValidator : AbstractValidator<HtbDateViewModel>
    {
        public HtbDateValidator()
        {
            RuleFor(x => x.HtbDate)
                .SetValidator(new DateValidator());
        }
    }
}