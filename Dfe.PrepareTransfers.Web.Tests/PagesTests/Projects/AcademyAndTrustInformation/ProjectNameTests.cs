using Dfe.PrepareTransfers.Web.Pages.Projects.AcademyAndTrustInformation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.AcademyAndTrustInformation
{
    public class ProjectTests : BaseTests
    {
        private readonly IncomingTrustNameModel _subject;

        protected ProjectTests()
        {
            _subject = new IncomingTrustNameModel(ProjectRepository.Object) { Urn = ProjectUrn0001 };
        }
        public class OnPostAsync : ProjectTests
        {
            public OnPostAsync()
            {
                _subject.Urn = ProjectUrn0001;
            }

            [Fact]
            public async void GivenErrorInModelState_ReturnsCorrectPage()
            {
                _subject.ModelState.AddModelError(nameof(_subject.IncomingTrustName), "error");
                var result = await _subject.OnPostAsync();

                ProjectRepository.Verify(r => r.UpdateIncomingTrustName(It.IsAny<string>(), It.IsAny<string>(), string.Empty), Times.Never);

                Assert.IsType<PageResult>(result);
            }

            [Fact]
            public async void GivenUrnAndProject_UpdatesTheProject()
            {
                _subject.IncomingTrustName = "New Project Name";
                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                        r.UpdateIncomingTrustName(It.IsAny<string>(), It.IsAny<string>(), string.Empty),
                    Times.Once);
            }
        }

    }

}