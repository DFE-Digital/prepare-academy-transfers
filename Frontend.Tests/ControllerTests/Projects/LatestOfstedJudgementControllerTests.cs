using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.Academies;
using Data.Models.Projects;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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

            _subject = new LatestOfstedJudgementController(_getInformationForProject.Object,
                _projectsRepository.Object);
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
                    LatestOfstedJudgementAdditionalInformation = "some info"
                };

                _foundAcademy = new Academy
                {
                    Ukprn = "ukprn",
                    GeneralInformation = new GeneralInformation()
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

                    Assert.Equal(_foundProject.Urn, viewModel.Urn);
                }

                [Fact]
                public async void GivenAcademy_AssignsTheProjectToTheViewModel()
                {
                    var response = await _subject.Index(_projectUrn);

                    var viewResponse = Assert.IsType<ViewResult>(response);
                    var viewModel = Assert.IsType<LatestOfstedJudgementViewModel>(viewResponse.Model);

                    Assert.Equal(_foundAcademy.Urn, viewModel.OutgoingAcademyUrn);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.Index("errorUrn");
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }

                [Fact]
                public async void GivenReturnToPreview_AssignsToTheViewModel()
                {
                    var response = await _subject.Index(_projectUrn, false, true);
                    var viewModel =
                        ControllerTestHelpers.AssertViewModelFromResult<LatestOfstedJudgementViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                    Assert.True(viewModel.AdditionalInformation.ReturnToPreview);
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
                    Assert.Equal(additionalInformation, _foundProject.LatestOfstedJudgementAdditionalInformation);
                }

                [Fact]
                public async void GivenAdditionalInformation_UpdatesTheViewModel()
                {
                    var response = await _subject.Index(_projectUrn);

                    var viewResponse = Assert.IsType<ViewResult>(response);
                    var viewModel = Assert.IsType<LatestOfstedJudgementViewModel>(viewResponse.Model);

                    Assert.Equal(_projectUrn, viewModel.AdditionalInformation.Urn);
                    Assert.False(viewModel.AdditionalInformation.AddOrEditAdditionalInformation);
                    Assert.Equal("some info", viewModel.AdditionalInformation.AdditionalInformation);
                }

                [Fact]
                public async void GivenAdditionalInformation_UpdatesTheProjectCorrectly()
                {
                    var additionalInfo = "test info";

                    await _subject.Index(_projectUrn, additionalInfo);
                    _projectsRepository.Verify(r => r.Update(It.Is<Project>(
                        project => project.LatestOfstedJudgementAdditionalInformation == additionalInfo
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

                    var response = await _subject.Index(_projectUrn, "test");
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewResult.Model);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectsToPreviewPage()
                {
                    var response = await _subject.Index(_projectUrn, "meow", true);
                    
                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response,
                        Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new {id = _projectUrn})
                    );
                }
            }
        }
    }
}