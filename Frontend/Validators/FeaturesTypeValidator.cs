using Data.Models.Projects;
using FluentValidation;
using Frontend.Models.Features;

namespace Frontend.Validators
{
    public class FeaturesTypeValidator : AbstractValidator<FeaturesTypeViewModel>
    {
        public FeaturesTypeValidator()
        {
            CascadeMode = CascadeMode.Stop;
            
            RuleFor(x => x.TypeOfTransfer)
                          .NotEqual(TransferFeatures.TransferTypes.Empty)
                          .WithMessage("Select the type of transfer");

            RuleFor(x => x.OtherType)
                .NotEmpty()
                .When(x => x.TypeOfTransfer == TransferFeatures.TransferTypes.Other)
                .WithMessage("Enter the type of transfer");
        }
    }
}