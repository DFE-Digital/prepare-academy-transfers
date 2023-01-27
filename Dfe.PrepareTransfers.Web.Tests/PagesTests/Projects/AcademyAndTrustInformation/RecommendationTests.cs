using System.Collections.Generic;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.AcademyAndTrustInformation;
using Dfe.PrepareTransfers.Web.Pages.Projects.AcademyAndTrustInformation;
using Dfe.PrepareTransfers.Web.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using System.Linq;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.AcademyAndTrustInformation
{
    public class RecommendationTests : BaseTests
    {
        protected readonly Recommendation Subject;

        protected RecommendationTests()
        {
            Subject = new Recommendation(ProjectRepository.Object);
        }
    }

    public class RecommendationGetTests : RecommendationTests
    {
        [Fact]
        public async void GivenUrn_FetchesProjectFromTheRepository()
        {
            await Subject.OnGetAsync(ProjectUrn0001);

            ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
        }

        [Fact]
        public async void GivenReturnToPreview_AssignItToTheView()
        {
            var response = await Subject.OnGetAsync(ProjectUrn0001, true);

            Assert.IsType<PageResult>(response);
            Assert.True(Subject.ReturnToPreview);
        }
    }

    public class RecommendationPostTests : RecommendationTests
    {
        private readonly RecommendationViewModel _vm;

        public RecommendationPostTests()
        {
            _vm = new RecommendationViewModel
            {
                Urn = ProjectUrn0001,
                Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Approve,
                Author = "Author"
            };
        }

        [Fact]
        public async void GivenUrnAndRecommendationAndAuthor_UpdatesTheProject()
        {
            await Subject.OnPostAsync(_vm);

            ProjectRepository.Verify(r =>
                r.Update(It.Is<Project>(project =>
                    project.AcademyAndTrustInformation.Recommendation == _vm.Recommendation &&
                    project.AcademyAndTrustInformation.Author == _vm.Author)), Times.Once);
        }

        [Fact]
        public async void GivenUrnAndRecommendationAndAuthor_RedirectsBackToTheSummary()
        {
            var result = await Subject.OnPostAsync(_vm);

            var routeValues = new RouteValueDictionary(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Urn", ProjectUrn0001)
            });
            ControllerTestHelpers.AssertResultRedirectsToPage(result,
                $"/Projects/AcademyAndTrustInformation/{nameof(Index)}", routeValues);
        }

        [Fact]
        public async void GivenReturnToPreview_ReturnToThePreviewPage()
        {
            _vm.ReturnToPreview = true;

            var response = await Subject.OnPostAsync(_vm);

            ControllerTestHelpers.AssertResultRedirectsToPage(response,
                Links.HeadteacherBoard.Preview.PageName, new RouteValueDictionary(new {Urn = ProjectUrn0001}));
        }
    }

    public class RecommendationRadioButtonTests
    {
        [Fact]
        public void GivenEmptySelectedValue_GeneratesListWithNoItemsChecked()
        {
            const TransferAcademyAndTrustInformation.RecommendationResult recommendationResult =
                TransferAcademyAndTrustInformation.RecommendationResult.Empty;

            var result = Recommendation.RecommendedRadioButtons(recommendationResult);

            Assert.All(result, item => Assert.False(item.Checked));
        }

        [Fact]
        public void GivenASelectedValue_GeneratesListWithRelevantItemChecked()
        {
            const TransferAcademyAndTrustInformation.RecommendationResult recommendationResult =
                TransferAcademyAndTrustInformation.RecommendationResult.Approve;

            var result = Recommendation.RecommendedRadioButtons(recommendationResult);

            Assert.Single(result.Where(item => item.Value == recommendationResult.ToString() && item.Checked));
        }
    }
}