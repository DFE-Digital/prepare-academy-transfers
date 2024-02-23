using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Tests.Dfe.PrepareTransfers.Helpers;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.Features
{
    public class ReasonTests : BaseTests
    {
        private readonly Pages.Projects.Features.Reason _subject;
        public ReasonTests()
        {
            _subject = new Pages.Projects.Features.Reason(ProjectRepository.Object)
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
        public async void GivenUrnAndReason_GetsProjectFromRepository()
        {
            _subject.ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.Dfe;
            await _subject.OnPostAsync();
            ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
        }

        [Fact]
        public async void GivenUrnAndReason_RedirectsProjectToSpecificReason()
        {
            _subject.ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.Dfe;
            var request = await _subject.OnPostAsync();

            ControllerTestHelpers.AssertResultRedirectsToPage(request, "/Projects/Features/SpecificReason",
                new RouteValueDictionary(new {Urn = ProjectUrn0001}));
        }

        [Theory]
        [InlineData(TransferFeatures.ReasonForTheTransferTypes.Dfe)]
        [InlineData(TransferFeatures.ReasonForTheTransferTypes.OutgoingTrust)]
        public async void GivenUrnAndReason_AssignsTheCorrectEnumAndUpdatesTheProject(
            TransferFeatures.ReasonForTheTransferTypes reason)
        {
            _subject.ReasonForTheTransfer = reason;
            await _subject.OnPostAsync();
            ProjectRepository.Verify(
                r => r.UpdateFeatures(
                    It.Is<Project>(project => project.Features.ReasonForTheTransfer == reason))
            );
        }

        [Fact]
        public async void GivenEmptyReason_DoNotUpdateTheModel()
        {
            _subject.ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.Empty;

            await _subject.OnPostAsync();

            //Assert
            ProjectRepository.Verify(r => r.UpdateFeatures(It.IsAny<Project>()), Times.Never);
        }

        [Fact]
        public async void GivenReturnToPreview_RedirectsToPreviewPage()
        {
            _subject.ReturnToPreview = true;
            _subject.ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.Dfe;

            var response = await _subject.OnPostAsync();

            ControllerTestHelpers.AssertResultRedirectsToPage(
                response, Links.HeadteacherBoard.Preview.PageName,
                new RouteValueDictionary(new {Urn = ProjectUrn0001})
            );
        }
    }
}