using Data.Models.Projects;
using Frontend.Models.Forms;

namespace Frontend.Models.AcademyAndTrustInformation
{
    public class RecommendationViewModel : CommonViewModel
    {
        public TransferAcademyAndTrustInformation.RecommendationResult Recommendation { get; set; }
        public string Author { get; set; }
    }
}