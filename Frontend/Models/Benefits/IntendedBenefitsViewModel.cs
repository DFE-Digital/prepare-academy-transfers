using System.Collections.Generic;
using Data.Models.Projects;

namespace Frontend.Models.Benefits
{
    public class IntendedBenefitsViewModel
    {
        public string ProjectUrn { get; set; }
        public string OutgoingAcademyName { get; set; }
        public bool ReturnToPreview { get; set; }
        public List<TransferBenefits.IntendedBenefit> SelectedIntendedBenefits { get; set; }
    }
}
