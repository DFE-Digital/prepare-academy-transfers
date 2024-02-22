using Dfe.PrepareTransfers.Data.Models.Projects;
using FluentValidation;
using Dfe.PrepareTransfers.Web.Pages.Projects.Features;
using System.Linq;

namespace Dfe.PrepareTransfers.Web.Validators.Features
{
    public class FeaturesSpecificReasonValidator : AbstractValidator<SpecificReason>
    {
        public FeaturesSpecificReasonValidator()
        {
            RuleFor(x => x.SpecificReasonsForTheTransfer)
                .Must(collection => collection == null || collection.All(item => item != TransferFeatures.SpecificReasonForTheTransferTypes.Empty))
                .WithMessage("Select a specific reason for the transfer");
        }
    }
}