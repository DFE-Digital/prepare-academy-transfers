using System;
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
            private readonly string _projectUrn;
            private readonly Project _foundProject;
            private readonly Academy _foundAcademy;

            public PreviewTests()
            {
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

                _foundAcademy = new Academy {Ukprn = "FoundNonDynamicsUkprn"};

                _getInformationForProject.Setup(s => s.Execute(_projectUrn)).ReturnsAsync(
                    new GetInformationForProjectResponse {Project = _foundProject, OutgoingAcademy = _foundAcademy});
            }

            [Fact]
            public async void GivenId_GetsProjectInformation()
            {
                await _subject.Preview(_projectUrn);

                _getInformationForProject.Verify(s => s.Execute(_projectUrn), Times.Once);
            }

            [Fact]
            public async void GivenId_PutsTheProjectInTheViewModel()
            {
                var response = await _subject.Preview(_projectUrn);
                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<HeadTeacherBoardPreviewViewModel>(viewResponse.Model);

                Assert.Equal(_foundProject, viewModel.Project);
            }

            [Fact]
            public async void GivenId_PutsTheOutgoingAcademyInTheViewModel()
            {
                var response = await _subject.Preview(_projectUrn);
                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<HeadTeacherBoardPreviewViewModel>(viewResponse.Model);

                Assert.Equal(_foundAcademy, viewModel.OutgoingAcademy);
            }
        }

        public class DownloadTests : HeadteacherBoardControllerTests
        {
            private readonly string _projectUrn;
            private readonly Project _foundProject;

            public DownloadTests()
            {
                _projectUrn = "ProjectId";

                _foundProject = new Project
                {
                    Urn = _projectUrn,
                    TransferringAcademies = new List<TransferringAcademies>
                    {
                        new TransferringAcademies
                            {OutgoingAcademyUkprn = "12345"}
                    }
                };

                var foundAcademy = new Academy {Ukprn = "FoundNonDynamicsUkprn"};

                _getInformationForProject.Setup(s => s.Execute(_projectUrn)).ReturnsAsync(
                    new GetInformationForProjectResponse {Project = _foundProject, OutgoingAcademy = foundAcademy});
            }

            [Fact]
            public async void GivenId_GetsProjectInformation()
            {
                await _subject.Download(_projectUrn);

                _getInformationForProject.Verify(s => s.Execute(_projectUrn), Times.Once);
            }

            [Fact]
            public async void GivenId_PutsTheProjectInTheViewModel()
            {
                var response = await _subject.Preview(_projectUrn);
                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<HeadTeacherBoardPreviewViewModel>(viewResponse.Model);

                Assert.Equal(_foundProject, viewModel.Project);
            }
        }

        public class GenerateDocumentTests : HeadteacherBoardControllerTests
        {
            [Fact]
            public async void GivenId_GeneratesAnHtbDocumentForTheProject()
            {
                const string projectUrn = "projectUrn";
                await _subject.GenerateDocument(projectUrn);

                _createHtbDocument.Verify(s => s.Execute(projectUrn), Times.Once);
            }

            [Fact]
            public async void GivenId_ReturnsAFileWithTheGeneratedDocument()
            {
                const string projectUrn = "projectUrn";
                var fileContents = new byte[] {1, 2, 3, 4};
                _createHtbDocument.Setup(s => s.Execute(projectUrn)).ReturnsAsync(fileContents);
                var response = await _subject.GenerateDocument(projectUrn);
                var fileResponse = Assert.IsType<FileContentResult>(response);

                Assert.Equal(fileContents, fileResponse.FileContents);
            }
        }
    }
}