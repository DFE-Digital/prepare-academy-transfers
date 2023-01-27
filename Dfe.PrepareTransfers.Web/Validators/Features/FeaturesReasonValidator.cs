using Dfe.PrepareTransfers.Data.Models.Projects;
using FluentValidation;
using Dfe.PrepareTransfers.Web.Pages.Projects.Features;

namespace Dfe.PrepareTransfers.Web.Validators.Features
{
    public class FeaturesReasonValidator : AbstractValidator<Reason>
    {
        public FeaturesReasonValidator()
        {
            RuleFor(x => x.ReasonForTheTransfer)
                .NotNull()
                .NotEqual(TransferFeatures.ReasonForTheTransferTypes.Empty)
                .WithMessage("Select a reason for the transfer");
        }
    }
}