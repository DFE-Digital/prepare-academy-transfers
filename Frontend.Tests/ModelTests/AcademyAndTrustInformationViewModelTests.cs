using System.Linq;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.AcademyAndTrustInformation;
using Xunit;

namespace Frontend.Tests.ModelTests
{
    public class AcademyAndTrustInformationViewModelTests
    {
        public class RecommendedRadioButtonsTests : AcademyAndTrustInformationViewModelTests
        {
            [Fact]
            public void GivenEmptySelectedValue_GeneratesListWithNoItemsChecked()
            {
                const TransferAcademyAndTrustInformation.RecommendationResult recommendationResult = 
                    TransferAcademyAndTrustInformation.RecommendationResult.Empty;

                var result = RecommendationViewModel.RecommendedRadioButtons(recommendationResult);

                Assert.All(result, item => Assert.False(item.Checked));
            }
            
            [Fact]
            public void GivenASelectedValue_GeneratesListWithRelevantItemChecked()
            {
                const TransferAcademyAndTrustInformation.RecommendationResult recommendationResult = 
                    TransferAcademyAndTrustInformation.RecommendationResult.Approve;

                var result = RecommendationViewModel.RecommendedRadioButtons(recommendationResult);

                Assert.Single(result.Where(item => item.Value == recommendationResult.ToString() && item.Checked));
            }
        }
    }
}