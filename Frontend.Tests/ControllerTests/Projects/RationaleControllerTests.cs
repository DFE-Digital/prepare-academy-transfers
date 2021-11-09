using Data;
using Data.Models;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class RationaleControllerTests
    {
        private const string _errorWithGetByUrn = "errorUrn";
        private readonly RationaleController _subject;
        private readonly Mock<IProjects> _projectRepository;

        public RationaleControllerTests()
        {
            _projectRepository = new Mock<IProjects>();
            _projectRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResult<Project> {Result = new Project()});
            _projectRepository.Setup(s => s.GetByUrn(_errorWithGetByUrn))
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

            _subject = new RationaleController(_projectRepository.Object);
        }

        public class IndexTests : RationaleControllerTests
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
                var response = await _subject.Index(_errorWithGetByUrn);
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }
        }

        public class ProjectTests : RationaleControllerTests
        {
            public class GetTests : ProjectTests
            {
                [Fact]
                public async void GivenUrn_FetchesProjectFromTheRepository()
                {
                    await _subject.Project("0001");

                    _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.Project(_errorWithGetByUrn);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }

                [Fact]
                public async void GivenReturnToPreview_AssignToTheViewModel()
                {
                    var response = await _subject.Project("0001", true);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<RationaleViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }
            }

            public class PostTests : ProjectTests
            {
                [Fact]
                public async void GivenUrnAndRationale_UpdatesTheProject()
                {
                    const string rationale = "This is the project rationale";
                    await _subject.ProjectPost("0001", rationale);

                    _projectRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Rationale.Project == rationale)), Times.Once);
                }

                [Fact]
                public async void GivenUrnAndRationale_RedirectsBackToTheSummary()
                {
                    const string rationale = "This is the project rationale";
                    var result = await _subject.ProjectPost("0001", rationale);

                    ControllerTestHelpers.AssertResultRedirectsToAction(result, "Index");
                }

                [Fact]
                public async void GivenUrnAndNoRationale_AddsErrorToTheModel()
                {
                    var result = await _subject.ProjectPost("0001", "");
                    var model = ControllerTestHelpers.AssertViewModelFromResult<RationaleViewModel>(result);
                    Assert.True(model.FormErrors.HasErrors);
                    Assert.True(model.FormErrors.HasErrorForField("rationale"));
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.ProjectPost(_errorWithGetByUrn, "rationale");
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

                    var response = await _subject.ProjectPost("errorWithUpdate", "rationale");
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Update error", viewResult.Model);
                }

                [Fact]
                public async void GivenInvalidInputAndReturnToPreview_AssignsToTheViewModel()
                {
                    var response = await _subject.ProjectPost("0001", null, true);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<RationaleViewModel>(response);
                    
                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectsToPreviewPage()
                {
                    var response = await _subject.ProjectPost("0001", "meow", true);

                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response, Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new {id = "0001"})
                    );
                }
            }
        }

        public class TrustOrSponsorTests : RationaleControllerTests
        {
            public class GetTests : TrustOrSponsorTests
            {
                [Fact]
                public async void GivenUrn_FetchesProjectFromTheRepository()
                {
                    await _subject.TrustOrSponsor("0001");

                    _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.TrustOrSponsor(_errorWithGetByUrn);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }
                
                [Fact]
                public async void GivenReturnToPreview_AssignToTheViewModel()
                {
                    var response = await _subject.TrustOrSponsor("0001", true);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<RationaleViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }
            }

            public class PostTests : TrustOrSponsorTests
            {
                [Fact]
                public async void GivenUrnAndRationale_UpdatesTheProject()
                {
                    const string rationale = "This is the trust rationale";
                    await _subject.TrustOrSponsorPost("0001", rationale);

                    _projectRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Rationale.Trust == rationale)), Times.Once);
                }

                [Fact]
                public async void GivenUrnAndRationale_RedirectsBackToTheSummary()
                {
                    const string rationale = "This is the project rationale";
                    var result = await _subject.TrustOrSponsorPost("0001", rationale);

                    ControllerTestHelpers.AssertResultRedirectsToAction(result, "Index");
                }

                [Fact]
                public async void GivenUrnAndNoRationale_AddsErrorToTheModel()
                {
                    var result = await _subject.TrustOrSponsorPost("0001", "");
                    var model = ControllerTestHelpers.AssertViewModelFromResult<RationaleViewModel>(result);
                    Assert.True(model.FormErrors.HasErrors);
                    Assert.True(model.FormErrors.HasErrorForField("rationale"));
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.TrustOrSponsorPost(_errorWithGetByUrn, "rationale");
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

                    var response = await _subject.TrustOrSponsorPost("errorWithUpdate", "rationale");
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Update error", viewResult.Model);
                }

                [Fact]
                public async void GivenInvalidInputAndReturnToPreview_AssignsToTheViewModel()
                {
                    var response = await _subject.TrustOrSponsorPost("0001", null, true);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<RationaleViewModel>(response);
                    
                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectsToPreviewPage()
                {
                    var response = await _subject.TrustOrSponsorPost("0001", "meow", true);

                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response, Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new {id = "0001"})
                    );
                }
            }
        }
    }
}