using System.Collections.Generic;
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
    public class LatestOfstedJudgementControllerTests
    {
        private readonly LatestOfstedJudgementController _subject;
        private readonly Mock<IGetInformationForProject> _getInformationForProject;

        protected LatestOfstedJudgementControllerTests()
        {
            _getInformationForProject = new Mock<IGetInformationForProject>();

            _subject = new LatestOfstedJudgementController(_getInformationForProject.Object);
        }

        public class IndexTests : LatestOfstedJudgementControllerTests
        {
            private readonly string _projectUrn;
            private readonly Project _foundProject;
            private readonly Academy _foundAcademy;

            public IndexTests()
            {
                _projectUrn = "ProjectUrn";

                _foundProject = new Project
                {
                    Urn = _projectUrn,
                    TransferringAcademies = new List<TransferringAcademies>
                        {new TransferringAcademies() {OutgoingAcademyUkprn = "1234567"}}
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
            }

            [Fact]
            public async void GivenProjectId_GetsInformationAboutTheProject()
            {
                await _subject.Index(_projectUrn);

                _getInformationForProject.Verify(s => s.Execute(_projectUrn), Times.Once);
            }

            [Fact]
            public async void GivenExistingProject_AssignsTheProjectToTheViewModel()
            {
                var response = await _subject.Index(_projectUrn);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<LatestOfstedJudgementViewModel>(viewResponse.Model);

                Assert.Equal(_foundProject, viewModel.Project);
            }

            [Fact]
            public async void GivenAcademy_AssignsTheProjectToTheViewModel()
            {
                var response = await _subject.Index(_projectUrn);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<LatestOfstedJudgementViewModel>(viewResponse.Model);

                Assert.Equal(_foundAcademy, viewModel.Academy);
            }
        }
    }
}