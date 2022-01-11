using System.Collections.Generic;
using System.Linq;
using Data.Models.Projects;
using Frontend.Models.Forms;
using Helpers;

namespace Frontend.Models.Features
{
    public class FeaturesTypeViewModel
    {
        public TransferFeatures.TransferTypes TypeOfTransfer { get; set; }
        public string OtherType { get; set; }
    }
}