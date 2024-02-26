using Dfe.PrepareTransfers.Data.Models.Projects;
using FluentValidation;
using Dfe.PrepareTransfers.Web.Pages.Projects.Features;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace Dfe.PrepareTransfers.Web.Validators.Features
{
    public class FeaturesSpecificReasonValidator : AbstractValidator<SpecificReason>
    {
        public FeaturesSpecificReasonValidator()
        {
            RuleFor(x => x.SpecificReasonsForTheTransfer)
                .Must(collection => collection.IsNullOrEmpty() || collection.All(item => item != TransferFeatures.SpecificReasonForTheTransferTypes.Empty))
                .WithMessage("Select a specific reason for the transfer");
        }
    }
}