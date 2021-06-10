using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.Academies;
using Data.Models.Projects;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class AcademyPerformanceControllerTests
    {
        private readonly AcademyPerformanceController _subject;
        private readonly Mock<IGetInformationForProject> _getInformationForProject;
        private readonly Mock<IProjects> _projectsRepository;

        public AcademyPerformanceControllerTests()
        {
            _getInformationForProject = new Mock<IGetInformationForProject>();
            _projectsRepository = new Mock<IProjects>();

            _subject = new AcademyPerformanceController(_getInformationForProject.Object, _projectsRepository.Object);
        }

        public class IndexTests : AcademyPerformanceControllerTests
        {
            private readonly string _projectUrn;
            private readonly Project _foundProject;
            private readonly Academy _foundAcademy;

            public IndexTests()
            {
                _projectUrn = "ProjectUrn";
                var academyUkprn = "1234567";

                _foundProject = new Project
                {
                    Urn = _projectUrn,
                    TransferringAcademies = new List<TransferringAcademies>
                        {new TransferringAcademies {OutgoingAcademyUkprn = academyUkprn}},
                    AcademyPerformanceAdditionalInformation = "some info"
                };

                _foundAcademy = new Academy
                {
                    Ukprn = "ukprn",
                    Performance = new AcademyPerformance()
                };

                _getInformationForProject.Setup(s => s.Execute(_projectUrn)).ReturnsAsync(
                    new GetInformationForProjectResponse
                    {
                        Project = _foundProject,
                        OutgoingAcademy = _foundAcademy
                    });

                _projectsRepository.Setup(s => s.GetByUrn(_projectUrn)).ReturnsAsync(
                   new RepositoryResult<Project>
                   {
                       Result = _foundProject
                   });
            }

            [Fact]
            public async void GivenProjectId_GetsInformationForTheProject()
            {
                await _subject.Index(_projectUrn);

                _getInformationForProject.Verify(s => s.Execute(_projectUrn), Times.Once);
            }

            [Fact]
            public async void GivenExistingProject_AssignsTheProjectToTheViewModel()
            {
                var response = await _subject.Index(_projectUrn);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<AcademyPerformanceViewModel>(viewResponse.Model);

                Assert.Equal(_foundProject, viewModel.Project);
            }

            [Fact]
            public async void GivenAcademy_AssignsTheProjectToTheViewModel()
            {
                var response = await _subject.Index(_projectUrn);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<AcademyPerformanceViewModel>(viewResponse.Model);

                Assert.Equal(_foundAcademy, viewModel.OutgoingAcademy);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectModel()
            {
                var additionalInformation = "some additional info";

                var response = await _subject.Index(_projectUrn, additionalInformation);

                var redirectToActionResponse = Assert.IsType<RedirectToActionResult>(response);
                Assert.Equal("AcademyPerformance", redirectToActionResponse.ControllerName);
                Assert.Equal("Index", redirectToActionResponse.ActionName);
                Assert.Equal(additionalInformation, _foundProject.AcademyPerformanceAdditionalInformation);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheViewModel()
            {
                var response = await _subject.Index(_projectUrn);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<AcademyPerformanceViewModel>(viewResponse.Model);

                Assert.Equal(_projectUrn, viewModel.AdditionalInformationModel.Urn);
                Assert.False(viewModel.AdditionalInformationModel.AddOrEditAdditionalInformation);
                Assert.Equal("some info", viewModel.AdditionalInformationModel.AdditionalInformation);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectCorrectly()
            {
                var additionalInfo = "test info";

                await _subject.Index(_projectUrn, additionalInfo);
                _projectsRepository.Verify(r => r.Update(It.Is<Project>(
                    project => project.AcademyPerformanceAdditionalInformation == additionalInfo
                )));
            }
        }
    }
}