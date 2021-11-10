using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Models.Rationale;
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
                .ReturnsAsync(new RepositoryResult<Project> {Result = new Project
                {
                    TransferringAcademies = new List<TransferringAcademies>
                    {
                        new TransferringAcademies
                        {
                            OutgoingAcademyName="Outgoing Academy"
                        }
                    }
                }});
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
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<RationaleProjectViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }
            }

            public class PostTests : ProjectTests
            {
                [Fact]
                public async void GivenUrnAndRationale_UpdatesTheProject()
                {
                    const string rationale = "This is the project rationale";
                    var vm = new RationaleProjectViewModel
                    {
                        Urn = "0001",
                        ProjectRationale = rationale
                    };
                    await _subject.ProjectPost(vm);

                    _projectRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Rationale.Project == rationale)), Times.Once);
                }

                [Fact]
                public async void GivenUrnAndRationale_RedirectsBackToTheSummary()
                {
                    const string rationale = "This is the project rationale";
                    var vm = new RationaleProjectViewModel
                    {
                        Urn = "0001",
                        ProjectRationale = rationale
                    };
                    var result = await _subject.ProjectPost(vm);

                    ControllerTestHelpers.AssertResultRedirectsToAction(result, "Index");
                }

                [Fact]
                public async void GivenErrorInModelState_ReturnsCorrectView()
                {
                    var vm = new RationaleProjectViewModel
                    {
                        Urn = "0001",
                        ProjectRationale = ""
                    };
                    _subject.ModelState.AddModelError(nameof(vm.ProjectRationale), "error");
                    var result = await _subject.ProjectPost(vm);

                    _projectRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Rationale.Project == "")), Times.Never);
                    
                    var viewResult = Assert.IsType<ViewResult>(result);

                    Assert.False(_subject.ModelState.IsValid);
                    Assert.Equal(vm, viewResult.Model);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var vm = new RationaleProjectViewModel
                    {
                        Urn = _errorWithGetByUrn,
                        ProjectRationale = "rationale"
                    };
                    var response = await _subject.ProjectPost(vm);
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

                    var vm = new RationaleProjectViewModel
                    {
                        Urn = "errorWithUpdate",
                        ProjectRationale = "rationale"
                    };
                    
                    var response = await _subject.ProjectPost(vm);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Update error", viewResult.Model);
                }

                [Fact]
                public async void GivenInvalidInputAndReturnToPreview_AssignsToTheViewModel()
                {
                    var vm = new RationaleProjectViewModel
                    {
                        Urn = "0001",
                        ProjectRationale = null,
                        ReturnToPreview = true
                    };
                    _subject.ModelState.AddModelError(nameof(vm.ProjectRationale), "error");
                    
                    var response = await _subject.ProjectPost(vm);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<RationaleProjectViewModel>(response);
                    
                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectsToPreviewPage()
                {
                    var vm = new RationaleProjectViewModel
                    {
                        Urn = "0001",
                        ProjectRationale = "meow",
                        ReturnToPreview = true
                    };
                    
                    var response = await _subject.ProjectPost(vm);

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
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<RationaleTrustOrSponsorViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }
            }

            public class PostTests : TrustOrSponsorTests
            {
                [Fact]
                public async void GivenUrnAndRationale_UpdatesTheProject()
                {
                    const string rationale = "This is the trust rationale";
                    var vm = new RationaleTrustOrSponsorViewModel
                    {
                        Urn = "0001",
                        TrustOrSponsorRationale = rationale
                    };
                    await _subject.TrustOrSponsorPost(vm);

                    _projectRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Rationale.Trust == rationale)), Times.Once);
                }

                [Fact]
                public async void GivenUrnAndRationale_RedirectsBackToTheSummary()
                {
                    const string rationale = "This is the project rationale";
                    var vm = new RationaleTrustOrSponsorViewModel
                    {
                        Urn = "0001",
                        TrustOrSponsorRationale = rationale
                    };
                    var result = await _subject.TrustOrSponsorPost(vm);

                    ControllerTestHelpers.AssertResultRedirectsToAction(result, "Index");
                }

                [Fact]
                public async void GivenErrorInModelState_ReturnsCorrectView()
                {
                    var vm = new RationaleTrustOrSponsorViewModel()
                    {
                        Urn = "0001",
                        TrustOrSponsorRationale = ""
                    };
                    _subject.ModelState.AddModelError(nameof(vm.TrustOrSponsorRationale), "error");
                    var result = await _subject.TrustOrSponsorPost(vm);

                    _projectRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Rationale.Trust == "")), Times.Never);
                    
                    var viewResult = Assert.IsType<ViewResult>(result);

                    Assert.False(_subject.ModelState.IsValid);
                    Assert.Equal(vm, viewResult.Model);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var vm = new RationaleTrustOrSponsorViewModel()
                    {
                        Urn = _errorWithGetByUrn,
                        TrustOrSponsorRationale = "rationale"
                    };
                    var response = await _subject.TrustOrSponsorPost(vm);
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

                    var vm = new RationaleTrustOrSponsorViewModel()
                    {
                        Urn = "errorWithUpdate",
                        TrustOrSponsorRationale = "rationale"
                    };
                    
                    var response = await _subject.TrustOrSponsorPost(vm);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Update error", viewResult.Model);
                }

                [Fact]
                public async void GivenInvalidInputAndReturnToPreview_AssignsToTheViewModel()
                {
                    var vm = new RationaleTrustOrSponsorViewModel()
                    {
                        Urn = "errorWithUpdate",
                        TrustOrSponsorRationale = "rationale",
                        ReturnToPreview = true
                    };
                    _subject.ModelState.AddModelError(nameof(vm.TrustOrSponsorRationale), "error");
                    var response = await _subject.TrustOrSponsorPost(vm);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<RationaleTrustOrSponsorViewModel>(response);
                    
                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectsToPreviewPage()
                {
                    var vm = new RationaleTrustOrSponsorViewModel()
                    {
                        Urn = "0001",
                        TrustOrSponsorRationale = "meow",
                        ReturnToPreview = true
                    };
                    var response = await _subject.TrustOrSponsorPost(vm);

                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response, Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new {id = "0001"})
                    );
                }
            }
        }
    }
}