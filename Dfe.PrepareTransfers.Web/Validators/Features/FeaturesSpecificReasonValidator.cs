using Dfe.PrepareTransfers.Data.Models.Projects;
using FluentValidation;
using Dfe.PrepareTransfers.Web.Pages.Projects.Features;

namespace Dfe.PrepareTransfers.Web.Validators.Features
{
    public class FeaturesSpecificReasonValidator : AbstractValidator<SpecificReason>
    {
        public FeaturesSpecificReasonValidator()
        {
            RuleFor(x => x.SpecificReasonForTheTransfer)
                .NotNull()
                .NotEqual(TransferFeatures.SpecificReasonForTheTransferTypes.Empty)
                .WithMessage("Select a specific reason for the transfer");
        }
    }
}