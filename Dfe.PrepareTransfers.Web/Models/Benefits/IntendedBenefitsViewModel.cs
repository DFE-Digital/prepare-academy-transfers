using System.Collections.Generic;
using Dfe.PrepareTransfers.Data.Models.Projects;

namespace Dfe.PrepareTransfers.Web.Models.Benefits
{
    public class IntendedBenefitsViewModel
    {
        public IList<TransferBenefits.IntendedBenefit> SelectedIntendedBenefits { get; set; } = new List<TransferBenefits.IntendedBenefit>();
        public string OtherBenefit { get; set; }
    }
}
