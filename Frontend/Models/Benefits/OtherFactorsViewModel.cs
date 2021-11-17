using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.Models.Projects;
using Helpers;

namespace Frontend.Models.Benefits
{
    public class OtherFactorsViewModel
    {
        public string ProjectUrn { get; set; }
        public string OutgoingAcademyName { get; set; }
        public bool ReturnToPreview { get; set; }
        public List<OtherFactorsItemViewModel> OtherFactorsVm { get; set; }
    }

    
    public class OtherFactorsItemViewModel
    {
        public TransferBenefits.OtherFactor OtherFactor { get; set; }
        public bool Checked { get; set; }
        public string Description { get; set; }
    }
}