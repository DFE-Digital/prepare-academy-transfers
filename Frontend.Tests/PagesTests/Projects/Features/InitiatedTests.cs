using Data.Models;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Tests.Helpers;
using Frontend.Validators.Features;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.Features
{
    public class InitiatedTests : PageTests
    {
        private readonly Pages.Projects.Features.Initiated _subject;
        public InitiatedTests()
        {
            _subject = new Pages.Projects.Features.Initiated(ProjectRepository.Object)
            {
                Urn = ProjectUrn0001
            };
        }

        [Fact]
        public async void GivenUrn_GetsProjectFromRepository()
        {
            await _subject.OnGetAsync();
            ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
        }

        [Fact]
        public async void GivenUrnAndInitiator_GetsProjectFromRepository()
        {
            _subject.FeaturesInitiatedViewModel.WhoInitiated = TransferFeatures.ProjectInitiators.Dfe;
            await _subject.OnPostAsync();
            ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
        }

        [Fact]
        public async void GivenUrnAndInitiator_RedirectsProjectToFeaturesSummary()
        {
            _subject.FeaturesInitiatedViewModel.WhoInitiated = TransferFeatures.ProjectInitiators.Dfe;
            var request = await _subject.OnPostAsync();

            ControllerTestHelpers.AssertResultRedirectsToPage(request, "/Projects/Features/Index",
                new RouteValueDictionary(new {Urn = ProjectUrn0001}));
        }

        [Theory]
        [InlineData(TransferFeatures.ProjectInitiators.Dfe)]
        [InlineData(TransferFeatures.ProjectInitiators.OutgoingTrust)]
        public async void GivenUrnAndWhoInitiated_AssignsTheCorrectEnumAndUpdatesTheProject(
            TransferFeatures.ProjectInitiators whoInitiated)
        {
            _subject.FeaturesInitiatedViewModel.WhoInitiated = whoInitiated;
            await _subject.OnPostAsync();
            ProjectRepository.Verify(
                r => r.Update(
                    It.Is<Project>(project => project.Features.WhoInitiatedTheTransfer == whoInitiated))
            );
        }

        [Fact]
        public async void GivenEmptyInitiator_DoNotUpdateTheModel()
        {
            _subject.FeaturesInitiatedViewModel.WhoInitiated = TransferFeatures.ProjectInitiators.Empty;

            await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesInitiatedValidator(),
                _subject.FeaturesInitiatedViewModel,
                _subject.ModelState);
            await _subject.OnPostAsync();

            //Assert
            ProjectRepository.Verify(r => r.Update(It.IsAny<Project>()), Times.Never);
        }

        [Fact]
        public async void GivenReturnToPreview_RedirectsToPreviewPage()
        {
            _subject.ReturnToPreview = true;
            var response = await _subject.OnPostAsync();

            ControllerTestHelpers.AssertResultRedirectsToPage(
                response, Links.HeadteacherBoard.Preview.PageName,
                new RouteValueDictionary(new {id = ProjectUrn0001})
            );
        }
    }
}