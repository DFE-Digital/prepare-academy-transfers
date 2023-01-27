using Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models.Forms;

namespace Dfe.PrepareTransfers.Web.Models.AcademyAndTrustInformation
{
    public class RecommendationViewModel : CommonViewModel
    {
        public TransferAcademyAndTrustInformation.RecommendationResult Recommendation { get; set; }
        public string Author { get; set; }
    }
}