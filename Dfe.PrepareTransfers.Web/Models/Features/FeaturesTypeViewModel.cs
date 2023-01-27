using System.Collections.Generic;
using System.Linq;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Helpers;

namespace Dfe.PrepareTransfers.Web.Models.Features
{
    public class FeaturesTypeViewModel
    {
        public TransferFeatures.TransferTypes TypeOfTransfer { get; set; }
        public string OtherType { get; set; }
    }
}