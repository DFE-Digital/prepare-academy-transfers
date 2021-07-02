using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Controllers.Projects;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class AcademyAndTrustInformationControllerTests
    {
        private const string ErrorWithGetByUrn = "errorUrn";
        private readonly AcademyAndTrustInformationController _subject;
        private readonly Mock<IProjects> _projectRepository;

        public AcademyAndTrustInformationControllerTests()
        {
            _projectRepository = new Mock<IProjects>();
            _projectRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResult<Project> { Result = new Project() });
            _projectRepository.Setup(s => s.GetByUrn(ErrorWithGetByUrn))
                .ReturnsAsync(
                    new RepositoryResult<Project>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            ErrorMessage = "Error"
                        }
                    });
            _projectRepository.Setup(r => r.Update(It.IsAny<Project>()))
                .ReturnsAsync(new RepositoryResult<Project>());
            _subject = new AcademyAndTrustInformationController(_projectRepository.Object);
        }

        public class IndexTests : AcademyAndTrustInformationControllerTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.Index("0001");

                _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var response = await _subject.Index(ErrorWithGetByUrn);
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }
        }

        public class RecommendationTests : AcademyAndTrustInformationControllerTests
        {
            public class GetTests : RecommendationTests
            {
                [Fact]
                public async void GivenUrn_FetchesProjectFromTheRepository()
                {
                    await _subject.Recommendation("0001");

                    _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.Recommendation(ErrorWithGetByUrn);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }
            }

            public class PostTests : RecommendationTests
            {
                [Fact]
                public async void GivenUrnAndRecommendationAndAuthor_UpdatesTheProject()
                {
                    const string author = "test author";
                    var recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Approve;
                    await _subject.Recommendation("0001", recommendation, author);

                    _projectRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.AcademyAndTrustInformation.Recommendation == recommendation && 
                                                           project.AcademyAndTrustInformation.Author == author)), Times.Once);
                }

                [Fact]
                public async void GivenUrnAndRecommendationAndAuthor_RedirectsBackToTheSummary()
                {
                    const string author = "test author";
                    const TransferAcademyAndTrustInformation.RecommendationResult recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Approve;
                    var result = await _subject.Recommendation("0001", recommendation, author);

                    ControllerTestHelpers.AssertResultRedirectsToAction(result, "Index");
                }
                
                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    const string author = "test author";
                    var recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Approve;
                    var response = await _subject.Recommendation(ErrorWithGetByUrn, recommendation, author);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }
                
                [Fact]
                public async void GivenUpdateReturnsError_DisplayErrorPage()
                {
                    _projectRepository.Setup(s => s.Update(It.IsAny<Project>())).ReturnsAsync(
                        new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                ErrorMessage = "Update error"
                            }
                        });

                    var recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Approve;
                    var response = await _subject.Recommendation("0001", recommendation, null);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Update error", viewResult.Model);
                }
            }
        }
    }
}