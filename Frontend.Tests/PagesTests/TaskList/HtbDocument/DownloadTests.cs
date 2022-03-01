using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.TaskList.HtbDocument
{
    public class DownloadTests : BaseTests
    {
        private readonly Pages.TaskList.HtbDocument.Download _subject;
        private readonly Mock<ICreateProjectTemplate> _createHtbDocument;

        protected DownloadTests()
        {
            _createHtbDocument = new Mock<ICreateProjectTemplate>();
            _subject = new Pages.TaskList.HtbDocument.Download(_createHtbDocument.Object,
                GetInformationForProject.Object)
            {
                Urn = ProjectUrn0001
            };

            _createHtbDocument.Setup(s => s.Execute(ProjectUrn0001)).ReturnsAsync(new CreateProjectTemplateResponse
                {Document = new byte[] {0, 1}});
        }

        public class GetTests : DownloadTests
        {
            [Fact]
            public async void GivenId_GetsProjectInformation()
            {
                FoundInformationForProject.Project.TransferringAcademies[0].IncomingTrustName = "Incoming Trust";
                await _subject.OnGetAsync();

                GetInformationForProject.Verify(s => s.Execute(ProjectUrn0001), Times.Once);
            }
        }

        public class GetDownloadTests : DownloadTests
        {
            [Fact]
            public async void GivenId_GeneratesAnHtbDocumentForTheProject()
            {
                FoundInformationForProject.Project.TransferringAcademies[0].IncomingTrustName = "Incoming Trust";
                await _subject.OnGetGenerateDocumentAsync();
                _createHtbDocument.Verify(s => s.Execute(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenId_ReturnsAFileWithTheGeneratedDocument()
            {
                var fileContents = new byte[] {1, 2, 3, 4};
                var createDocumentResponse = new CreateProjectTemplateResponse
                {
                    Document = fileContents
                };
                _createHtbDocument.Setup(s => s.Execute(ProjectUrn0001)).ReturnsAsync(createDocumentResponse);
                FoundInformationForProject.Project.TransferringAcademies[0].IncomingTrustName = "Incoming Trust";
                var response = await _subject.OnGetGenerateDocumentAsync();
                var fileResponse = Assert.IsType<FileContentResult>(response);

                Assert.Equal(fileContents, fileResponse.FileContents);
            }
        }
    }
}