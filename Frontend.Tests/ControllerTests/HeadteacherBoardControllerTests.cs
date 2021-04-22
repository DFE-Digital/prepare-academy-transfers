using System;
using System.Collections.Generic;
using API.Models.Upstream.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Data;
using Data.Models;
using Frontend.Controllers;
using Frontend.Models;
using Frontend.Services;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests
{
    public class HeadteacherBoardControllerTests
    {
        private readonly HeadteacherBoardController _subject;
        private readonly Mock<ICreateHtbDocument> _createHtbDocument;
        private readonly Mock<IGetInformationForProject> _getInformationForProject;

        public HeadteacherBoardControllerTests()
        {
            _createHtbDocument = new Mock<ICreateHtbDocument>();
            _getInformationForProject = new Mock<IGetInformationForProject>();

            _subject = new HeadteacherBoardController(_createHtbDocument.Object, _getInformationForProject.Object);
        }

        public class PreviewTests : HeadteacherBoardControllerTests
        {
            private readonly Guid _projectId;
            private readonly GetProjectsResponseModel _foundProject;
            private readonly Academy _foundAcademy;

            public PreviewTests()
            {
                _projectId = Guid.NewGuid();
                var academyId = Guid.NewGuid();

                _foundProject = new GetProjectsResponseModel
                {
                    ProjectId = _projectId,
                    ProjectAcademies = new List<GetProjectsAcademyResponseModel>
                    {
                        new GetProjectsAcademyResponseModel
                            {AcademyId = academyId, AcademyName = "Cat Academy"}
                    }
                };

                _foundAcademy = new Academy {Ukprn = "FoundNonDynamicsUkprn"};

                _getInformationForProject.Setup(s => s.Execute(_projectId)).ReturnsAsync(
                    new GetInformationForProjectResponse {Project = _foundProject, OutgoingAcademy = _foundAcademy});
            }

            [Fact]
            public async void GivenId_GetsProjectInformation()
            {
                await _subject.Preview(_projectId);

                _getInformationForProject.Verify(s => s.Execute(_projectId), Times.Once);
            }

            [Fact]
            public async void GivenId_PutsTheProjectInTheViewModel()
            {
                var response = await _subject.Preview(_projectId);
                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<HeadTeacherBoardPreviewViewModel>(viewResponse.Model);

                Assert.Equal(_foundProject, viewModel.Project);
            }

            [Fact]
            public async void GivenId_PutsTheOutgoingAcademyInTheViewModel()
            {
                var response = await _subject.Preview(_projectId);
                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<HeadTeacherBoardPreviewViewModel>(viewResponse.Model);

                Assert.Equal(_foundAcademy, viewModel.OutgoingAcademy);
            }
        }

        public class GenerateDocumentTests : HeadteacherBoardControllerTests
        {
            [Fact]
            public async void GivenId_GeneratesAnHtbDocumentForTheProject()
            {
                var projectId = Guid.NewGuid();
                await _subject.GenerateDocument(projectId);

                _createHtbDocument.Verify(s => s.Execute(projectId), Times.Once);
            }

            [Fact]
            public async void GivenId_ReturnsAFileWithTheGeneratedDocument()
            {
                var projectId = Guid.NewGuid();
                var fileContents = new byte[] {1, 2, 3, 4};
                _createHtbDocument.Setup(s => s.Execute(projectId)).ReturnsAsync(fileContents);
                var response = await _subject.GenerateDocument(projectId);
                var fileResponse = Assert.IsType<FileContentResult>(response);

                Assert.Equal(fileContents, fileResponse.FileContents);
            }
        }
    }
}