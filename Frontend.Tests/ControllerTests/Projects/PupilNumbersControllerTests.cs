using System;
using System.Collections.Generic;
using API.Models.Upstream.Response;
using Data.Models;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class PupilNumbersControllerTests
    {
        private readonly PupilNumbersController _subject;
        private readonly Mock<IGetInformationForProject> _getInformationForProject;

        protected PupilNumbersControllerTests()
        {
            _getInformationForProject = new Mock<IGetInformationForProject>();

            _subject = new PupilNumbersController(_getInformationForProject.Object);
        }

        public class IndexTests : PupilNumbersControllerTests
        {
            private readonly Guid _projectId;
            private readonly GetProjectsResponseModel _foundProject;
            private readonly Academy _foundAcademy;

            public IndexTests()
            {
                _projectId = Guid.NewGuid();
                var academyId = Guid.NewGuid();

                _foundProject = new GetProjectsResponseModel
                {
                    ProjectId = _projectId,
                    ProjectAcademies = new List<GetProjectsAcademyResponseModel>
                        {new GetProjectsAcademyResponseModel {AcademyId = academyId}}
                };

                _foundAcademy = new Academy
                {
                    Ukprn = "ukprn",
                    Performance = new AcademyPerformance()
                };

                _getInformationForProject.Setup(s => s.Execute(_projectId)).ReturnsAsync(
                    new GetInformationForProjectResponse()
                    {
                        Project = _foundProject,
                        OutgoingAcademy = _foundAcademy
                    });
            }

            [Fact]
            public async void GivenProjectId_GetsInformationAboutProject()
            {
                await _subject.Index(_projectId);

                _getInformationForProject.Verify(s => s.Execute(_projectId), Times.Once);
            }

            [Fact]
            public async void GivenExistingProject_AssignsTheProjectToTheViewModel()
            {
                var response = await _subject.Index(_projectId);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<PupilNumbersViewModel>(viewResponse.Model);

                Assert.Equal(_foundProject, viewModel.Project);
            }

            [Fact]
            public async void GivenAcademy_AssignsTheProjectToTheViewModel()
            {
                var response = await _subject.Index(_projectId);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<PupilNumbersViewModel>(viewResponse.Model);

                Assert.Equal(_foundAcademy, viewModel.OutgoingAcademy);
            }
        }
    }
}