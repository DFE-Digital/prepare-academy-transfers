using System.Collections.Generic;
using Data.Models;
using Data.Models.Projects;
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
        private readonly Academy _foundAcademy;

        protected DownloadTests()
        {
            _createHtbDocument = new Mock<ICreateHtbDocument>();
            _subject = new Pages.TaskList.HtbDocument.Download(_createHtbDocument.Object,
                GetInformationForProject.Object)
            {
                Urn = ProjectUrn0001
            };

            _foundAcademy = new Academy {Ukprn = "FoundNonDynamicsUkprn"};

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

            [Fact]
            public async void GivenGetInformationReturnsError_DisplayErrorPage()
            {
                _subject.Urn = ProjectErrorUrn;
                var response = await _subject.OnGetAsync();
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal(ErrorPageName, viewResult.ViewName);
                Assert.Equal(ErrorMessage, viewResult.Model);
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
            
            [Fact]
            public async void GivenExecuteReturnsError_GeneratesErrorResponse()
            {
                var createDocumentErrorResponse = new CreateHtbDocumentResponse
                {
                    ResponseError = new ServiceResponseError
                    {
                        ErrorCode = ErrorCode.ApiError,
                        ErrorMessage = ErrorMessage
                    }
                };

                _createHtbDocument.Setup(s => s.Execute(It.IsAny<string>())).ReturnsAsync(createDocumentErrorResponse);
                var response = await _subject.OnGetGenerateDocumentAsync();
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal(ErrorPageName, viewResult.ViewName);
                Assert.Equal(ErrorMessage, viewResult.Model);
            }
        }
    }
}