using System.Collections.Generic;
using System.Linq;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Helpers;

namespace Dfe.PrepareTransfers.Web.Models.Features
{
    public class FeaturesInitiatedViewModel
    {
        public TransferFeatures.ReasonForTheTransferTypes WhoInitiated { get; set; }
    }
}