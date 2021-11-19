using Data.Models.Projects;

namespace Frontend.Models.Benefits
{
    public class OtherFactorsItemViewModel
    {
        public TransferBenefits.OtherFactor OtherFactor { get; set; }
        public bool Checked { get; set; }
        public string Description { get; set; }
    }
}