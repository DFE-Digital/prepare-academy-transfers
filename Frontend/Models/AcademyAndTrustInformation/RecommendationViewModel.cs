using System.Collections.Generic;
using System.Linq;
using Data.Models.Projects;
using Frontend.Models.Forms;
using Helpers;

namespace Frontend.Models.AcademyAndTrustInformation
{
    public class RecommendationViewModel : CommonViewModel
    {
        public string OutgoingAcademyUrn { get; set; }
        public TransferAcademyAndTrustInformation.RecommendationResult Recommendation { get; set; }
        public string Author { get; set; }
        
        public static List<RadioButtonViewModel> RecommendedRadioButtons(TransferAcademyAndTrustInformation.RecommendationResult selected)
        {
            var values =
                EnumHelpers<TransferAcademyAndTrustInformation.RecommendationResult>.GetDisplayableValues(TransferAcademyAndTrustInformation.RecommendationResult
                    .Empty);

            var result = values.Select(value => new RadioButtonViewModel
            {
                Value = value.ToString(), 
                Name = "recommendation",
                DisplayName = value.ToString(),
                Checked = selected == value
            }).ToList();

            return result;
        }
    }
}