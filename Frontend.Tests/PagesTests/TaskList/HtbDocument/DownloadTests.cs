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
            {
                Document = new byte[] { 0, 1 }
            });

            FoundInformationForProject.Project.TransferringAcademies[0].IncomingTrustName = "Incoming Trust";
            FoundInformationForProject.Project.OutgoingTrustName = "Outgoing Trust";
            FoundInformationForProject.Project.Reference = "SW-MAT-10000001";
        }

        public class GetTests : DownloadTests
        {
            [Fact]
            public async void GivenId_GetsProjectInformation()
            {
                await _subject.OnGetAsync();

                GetInformationForProject.Verify(s => s.Execute(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenId_SetsFileName()
            {
                await _subject.OnGetAsync();

                Assert.Equal("SW-MAT-10000001_Outgoing-Trust_Incoming-Trust_project-template", _subject.FileName);
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
                var fileContents = new byte[] { 1, 2, 3, 4 };
                var createDocumentResponse = new CreateProjectTemplateResponse
                {
                    Document = fileContents
                };
                _createHtbDocument.Setup(s => s.Execute(ProjectUrn0001)).ReturnsAsync(createDocumentResponse);

                var response = await _subject.OnGetGenerateDocumentAsync();

                var fileResponse = Assert.IsType<FileContentResult>(response);
                Assert.Equal(fileContents, fileResponse.FileContents);
                Assert.Equal("SW-MAT-10000001_Outgoing-Trust_Incoming-Trust_project-template.docx", fileResponse.FileDownloadName);
            }
        }
    }
}