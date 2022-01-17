using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.TaskList.HtbDocument
{
    public class DownloadTests : PageTests
    {
        private readonly Pages.TaskList.HtbDocument.Download _subject;
        private readonly Mock<ICreateHtbDocument> _createHtbDocument;

        protected DownloadTests()
        {
            _createHtbDocument = new Mock<ICreateHtbDocument>();
            _subject = new Pages.TaskList.HtbDocument.Download(_createHtbDocument.Object,
                GetInformationForProject.Object)
            {
                Urn = ProjectUrn0001
            };

            _createHtbDocument.Setup(s => s.Execute(ProjectUrn0001)).ReturnsAsync(new CreateHtbDocumentResponse
                {Document = new byte[] {0, 1}});
        }

        public class GetTests : DownloadTests
        {
            [Fact]
            public async void GivenId_GetsProjectInformation()
            {
                await _subject.OnGetAsync();

                GetInformationForProject.Verify(s => s.Execute(ProjectUrn0001), Times.Once);
            }
        }

        public class GetDownloadTests : DownloadTests
        {
            [Fact]
            public async void GivenId_GeneratesAnHtbDocumentForTheProject()
            {
                await _subject.OnGetGenerateDocumentAsync();
                _createHtbDocument.Verify(s => s.Execute(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenId_ReturnsAFileWithTheGeneratedDocument()
            {
                var fileContents = new byte[] {1, 2, 3, 4};
                var createDocumentResponse = new CreateHtbDocumentResponse
                {
                    Document = fileContents
                };
                _createHtbDocument.Setup(s => s.Execute(ProjectUrn0001)).ReturnsAsync(createDocumentResponse);
                var response = await _subject.OnGetGenerateDocumentAsync();
                var fileResponse = Assert.IsType<FileContentResult>(response);

                Assert.Equal(fileContents, fileResponse.FileContents);
            }
        }
    }
}