using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Models.AcademyAndTrustInformation;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class AcademyAndTrustInformationControllerTests
    {
        private const string ProjectErrorUrn = "errorUrn";
        private const string ProjectUrn = "0001";
        private readonly AcademyAndTrustInformationController _subject;
        private readonly Mock<IProjects> _projectRepository;
        private readonly Mock<IGetInformationForProject> _getInformationForProject;

        public AcademyAndTrustInformationControllerTests()
        {
            _getInformationForProject = new Mock<IGetInformationForProject>();
            _projectRepository = new Mock<IProjects>();

            _subject = new AcademyAndTrustInformationController(_projectRepository.Object,
                _getInformationForProject.Object);
        }

        public class IndexTests : AcademyAndTrustInformationControllerTests
        {
            public IndexTests()
            {
                _getInformationForProject.Setup(s => s.Execute(ProjectUrn))
                    .ReturnsAsync(
                        new GetInformationForProjectResponse
                        {
                            Project = new Project
                            {
                                Urn = ProjectUrn,
                                TransferringAcademies = new List<TransferringAcademies>()
                                {
                                    new TransferringAcademies{OutgoingAcademyName = "Outgoing Academy One"}
                                }
                            }
                        });

                _getInformationForProject.Setup(s => s.Execute(ProjectErrorUrn))
                    .ReturnsAsync(
                        new GetInformationForProjectResponse
                        {
                            ResponseError = new ServiceResponseError
                            {
                                ErrorMessage = "Error"
                            }
                        });
            }

            
        }

        public class RecommendationTests : AcademyAndTrustInformationControllerTests
        {
            public RecommendationTests()
            {
                _projectRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                    .ReturnsAsync(
                        new RepositoryResult<Project>
                        {
                            Result = new Project()
                            {
                                TransferringAcademies = new List<TransferringAcademies>
                                {
                                    new TransferringAcademies
                                    {
                                        OutgoingAcademyName = "Outgoing Academy One"
                                    }
                                }
                            }
                        });

                _projectRepository.Setup(r => r.GetByUrn(ProjectErrorUrn))
                    .ReturnsAsync(
                        new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                ErrorMessage = "Error"
                            }
                        });
            }

            public class GetTests : RecommendationTests
            {
                [Fact]
                public async void GivenUrn_FetchesProjectFromTheRepository()
                {
                    await _subject.Recommendation(ProjectUrn);

                    _projectRepository.Verify(r => r.GetByUrn(ProjectUrn), Times.Once);
                }

                [Fact]
                public async void GivenReturnToPreview_AssignItToTheView()
                {
                    var response = await _subject.Recommendation(ProjectUrn, true);
                    var model =
                        ControllerTestHelpers.AssertViewModelFromResult<RecommendationViewModel>(response);

                    Assert.True(model.ReturnToPreview);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.Recommendation(ProjectErrorUrn);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }
            }

            public class PostTests : RecommendationTests
            {
                const string Author = "test author";
            
                const TransferAcademyAndTrustInformation.RecommendationResult Recommendation =
                    TransferAcademyAndTrustInformation.RecommendationResult.Approve;
                
                public PostTests()
                {
                    _projectRepository.Setup(r => r.Update(It.IsAny<Project>()))
                        .ReturnsAsync(
                            new RepositoryResult<Project>());
                }
            
                [Fact]
                public async void GivenUrnAndRecommendationAndAuthor_UpdatesTheProject()
                {
                    var vm = new RecommendationViewModel
                    {
                        Urn = ProjectUrn,
                        Recommendation = Recommendation,
                        Author = Author
                    };
                    
                    await _subject.Recommendation(vm);
            
                    _projectRepository.Verify(r =>
                        r.Update(It.Is<Project>(project =>
                            project.AcademyAndTrustInformation.Recommendation == Recommendation &&
                            project.AcademyAndTrustInformation.Author == Author)), Times.Once);
                }
            
                [Fact]
                public async void GivenUrnAndRecommendationAndAuthor_RedirectsBackToTheSummary()
                {
                    var vm = new RecommendationViewModel
                    {
                        Urn = ProjectUrn,
                        Recommendation = Recommendation,
                        Author = Author
                    };
                    
                    var result = await _subject.Recommendation(vm);
            
                    ControllerTestHelpers.AssertResultRedirectsToAction(result, "Index");
                }
            
                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var vm = new RecommendationViewModel
                    {
                        Urn = ProjectErrorUrn,
                        Recommendation = Recommendation,
                        Author = Author
                    };
                    var response = await _subject.Recommendation(vm);
                    var viewResult = Assert.IsType<ViewResult>(response);
            
                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }
            
                [Fact]
                public async void GivenUpdateReturnsError_DisplayErrorPage()
                {
                    _projectRepository.Setup(s => s.Update(It.IsAny<Project>()))
                        .ReturnsAsync(
                            new RepositoryResult<Project>
                            {
                                Error = new RepositoryResultBase.RepositoryError
                                {
                                    ErrorMessage = "Update error"
                                }
                            });
                    var vm = new RecommendationViewModel()
                    {
                        Urn = ProjectUrn,
                        Recommendation = Recommendation
                    };
            
                    var response = await _subject.Recommendation(vm);
                    var viewResult = Assert.IsType<ViewResult>(response);
            
                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Update error", viewResult.Model);
                }
            
                [Fact]
                public async void GivenReturnToPreview_ReturnToThePreviewPage()
                {
                    var vm = new RecommendationViewModel
                    {
                        Urn = ProjectUrn,
                        Recommendation = Recommendation,
                        Author = Author,
                        ReturnToPreview = true
                    };
                    
                    var response = await _subject.Recommendation(vm);
                    ControllerTestHelpers.AssertResultRedirectsToPage(response,
                        Links.HeadteacherBoard.Preview.PageName, new RouteValueDictionary(new {id = ProjectUrn}));
                }
            }
        }
    }
}