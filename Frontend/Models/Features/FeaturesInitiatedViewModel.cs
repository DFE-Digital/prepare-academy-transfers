using System.Collections.Generic;
using System.Linq;
using Data.Models.Projects;
using Frontend.Models.Forms;
using Helpers;

namespace Frontend.Models.Features
{
    public class FeaturesInitiatedViewModel
    {
        public TransferFeatures.ProjectInitiators WhoInitiated { get; set; }
    }
}