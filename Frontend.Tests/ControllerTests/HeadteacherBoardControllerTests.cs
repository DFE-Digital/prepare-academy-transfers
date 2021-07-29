using System.Collections.Generic;
using Data.Models;
using Data.Models.Projects;
using Frontend.Controllers;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests
{
    public class HeadteacherBoardControllerTests
    {
        private const string _errorUrn = "errorUrn";
        private readonly HeadteacherBoardController _subject;
        private readonly Mock<ICreateHtbDocument> _createHtbDocument;
        private readonly Mock<IGetInformationForProject> _getInformationForProject;
        private readonly string _projectUrn;
        private readonly Project _foundProject;
        private readonly Academy _foundAcademy;

        public HeadteacherBoardControllerTests()
        {
            _createHtbDocument = new Mock<ICreateHtbDocument>();
            _getInformationForProject = new Mock<IGetInformationForProject>();

            _subject = new HeadteacherBoardController(_createHtbDocument.Object, _getInformationForProject.Object);

            _projectUrn = "ProjectId";
            var academyUkprn = "12345";

            _foundProject = new Project
            {
                Urn = _projectUrn,
                TransferringAcademies = new List<TransferringAcademies>
                    {
                        new TransferringAcademies
                            {OutgoingAcademyUkprn = academyUkprn}
                    }
            };

            _foundAcademy = new Academy { Ukprn = "FoundNonDynamicsUkprn" };

            _createHtbDocument.Setup(s => s.Execute(_projectUrn)).ReturnsAsync(
                    new CreateHtbDocumentResponse { Document = new byte[] { 0, 1 } });
            _getInformationForProject.Setup(s => s.Execute(_projectUrn)).ReturnsAsync(
                new GetInformationForProjectResponse { Project = _foundProject, OutgoingAcademy = _foundAcademy });
            _getInformationForProject.Setup(s => s.Execute(_errorUrn)).ReturnsAsync(
                new GetInformationForProjectResponse
                {
                    ResponseError = new ServiceResponseError
                    {
                        ErrorCode = ErrorCode.NotFound,
                        ErrorMessage = "Error"
                    }
                });
        }

        public class DownloadTests : HeadteacherBoardControllerTests
        {
            [Fact]
            public async void GivenId_GetsProjectInformation()
            {
                await _subject.Download(_projectUrn);

                _getInformationForProject.Verify(s => s.Execute(_projectUrn), Times.Once);
            }

            [Fact]
            public async void GivenId_PutsTheProjectInTheViewModel()
            {
                var response = await _subject.Download(_projectUrn);
                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<ProjectViewModel>(viewResponse.Model);

                Assert.Equal(_foundProject, viewModel.Project);
            }

            [Fact]
            public async void GivenGetInformationReturnsError_DisplayErrorPage()
            {
                var response = await _subject.Download(_errorUrn);
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }
        }

        public class GenerateDocumentTests : HeadteacherBoardControllerTests
        {
            [Fact]
            public async void GivenId_GeneratesAnHtbDocumentForTheProject()
            {
                await _subject.GenerateDocument(_projectUrn);

                _createHtbDocument.Verify(s => s.Execute(_projectUrn), Times.Once);
            }

            [Fact]
            public async void GivenId_ReturnsAFileWithTheGeneratedDocument()
            {
                const string projectUrn = "projectUrn";
                var fileContents = new byte[] {1, 2, 3, 4};
                var createDocumentResponse = new CreateHtbDocumentResponse
                {
                    Document = fileContents
                };
                _createHtbDocument.Setup(s => s.Execute(projectUrn)).ReturnsAsync(createDocumentResponse);
                var response = await _subject.GenerateDocument(projectUrn);
                var fileResponse = Assert.IsType<FileContentResult>(response);

                Assert.Equal(fileContents, fileResponse.FileContents);
            }

            [Fact]
            public async void GivenExecuteReturnsError_GeneratesErrorResponse()
            {
                var errorUrn = "errorUrn";
                var createDocumentErrorResponse = new CreateHtbDocumentResponse
                {
                    ResponseError = new ServiceResponseError
                    {
                        ErrorCode = ErrorCode.ApiError,
                        ErrorMessage = "Error"
                    }
                };

                _createHtbDocument.Setup(s => s.Execute(errorUrn)).ReturnsAsync(createDocumentErrorResponse);
                var response = await _subject.GenerateDocument(_errorUrn);
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }
        }
    }
}