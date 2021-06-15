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
    public class LatestOfstedJudgementControllerTests
    {
        private readonly LatestOfstedJudgementController _subject;
        private readonly Mock<IGetInformationForProject> _getInformationForProject;
        private readonly Mock<IProjects> _projectsRepository;
        

        protected LatestOfstedJudgementControllerTests()
        {
            _getInformationForProject = new Mock<IGetInformationForProject>();
            _projectsRepository = new Mock<IProjects>();

            _subject = new LatestOfstedJudgementController(_getInformationForProject.Object, _projectsRepository.Object);
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
                        {new TransferringAcademies() {OutgoingAcademyUkprn = "1234567"}},
                    LatestOfstedAdditionalInformation = "some info"
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

                _getInformationForProject.Setup(s => s.Execute("errorUrn")).ReturnsAsync(
                    new GetInformationForProjectResponse
                    {
                        ResponseError = new ServiceResponseError
                        {
                            ErrorMessage = "Error"
                        }
                    });

                _projectsRepository.Setup(r => r.GetByUrn("errorUrn"))
                        .ReturnsAsync(new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                StatusCode = System.Net.HttpStatusCode.NotFound,
                                ErrorMessage = "Project not found"
                            }
                        });

                _projectsRepository.Setup(r => r.Update(It.IsAny<Project>()))
                    .ReturnsAsync(new RepositoryResult<Project>());
            }

            public class GetTests : IndexTests
            {
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

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.Index("errorUrn");
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }
            }

            public class PostTests : IndexTests
            {
                [Fact]
                public async void GivenAdditionalInformation_UpdatesTheProjectModel()
                {
                    var additionalInformation = "some additional info";

                    var response = await _subject.Index(_projectUrn, additionalInformation);

                    var redirectToActionResponse = Assert.IsType<RedirectToActionResult>(response);
                    Assert.Equal("LatestOfstedJudgement", redirectToActionResponse.ControllerName);
                    Assert.Equal("Index", redirectToActionResponse.ActionName);
                    Assert.Equal(additionalInformation, _foundProject.LatestOfstedAdditionalInformation);
                }

                [Fact]
                public async void GivenAdditionalInformation_UpdatesTheViewModel()
                {
                    var response = await _subject.Index(_projectUrn);

                    var viewResponse = Assert.IsType<ViewResult>(response);
                    var viewModel = Assert.IsType<LatestOfstedJudgementViewModel>(viewResponse.Model);

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
                        project => project.LatestOfstedAdditionalInformation == additionalInfo
                    )));
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.Index("errorUrn", "test");
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewResult.Model);
                }

                [Fact]
                public async void GivenUpdateReturnsError_DisplayErrorPage()
                {
                    _projectsRepository.Setup(r => r.Update(It.IsAny<Project>()))
                        .ReturnsAsync(new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                StatusCode = System.Net.HttpStatusCode.NotFound,
                                ErrorMessage = "Project not found"
                            }
                        });

                    var controller = new LatestOfstedJudgementController(_getInformationForProject.Object, _projectsRepository.Object);


                    var response = await _subject.Index(_projectUrn, "test");
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewResult.Model);
                }
            }
        }
    }
}