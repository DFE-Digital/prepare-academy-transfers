using System;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Tests.Helpers;
using Frontend.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using FluentValidation.AspNetCore;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class FeaturesControllerTests
    {
        private readonly FeaturesController _subject;
        private readonly Mock<IProjects> _projectRepository;
        private readonly Project _foundProject;

        public FeaturesControllerTests()
        {
            _projectRepository = new Mock<IProjects>();
            _subject = new FeaturesController(_projectRepository.Object);
            _foundProject = new Project
            {
                Urn = "0001",
                Features = new TransferFeatures
                {
                    ReasonForTransfer = new ReasonForTransfer()
                }
            };

            _projectRepository.Setup(r => r.GetByUrn("0001")).ReturnsAsync(new RepositoryResult<Project>()
            {
                Result = _foundProject
            });

            _projectRepository.Setup(r => r.GetByUrn("errorUrn"))
                .ReturnsAsync(new RepositoryResult<Project>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        ErrorMessage = "Project not found"
                    }
                });

            _projectRepository.Setup(r => r.Update(It.IsAny<Project>()))
                .ReturnsAsync(new RepositoryResult<Project>());
        }

        public class IndexTests : FeaturesControllerTests
        {
            [Fact]
            public async void GivenUrn_GetsProjectFromRepositoryAndAssignsToTheView()
            {
                var request = new Func<Task<IActionResult>>(async () => await _subject.Index("0001"));
                await AssertProjectIsGottenFromRepositoryAndAssignedToView(request);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var response = await _subject.Index("errorUrn");
                var viewResult = Assert.IsType<ViewResult>(response);
                var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Project not found", viewModel);
            }
        }

        public class InitiatedTests : FeaturesControllerTests
        {
            public class GetTests : InitiatedTests
            {
                [Fact]
                public async void GivenUrn_GetsProjectFromRepositoryAndAssignsToTheView()
                {
                    var request = new Func<Task<IActionResult>>(async () => await _subject.Initiated("0001"));
                    await AssertProjectIsGottenFromRepositoryAndAssignedToView(request);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.Initiated("errorUrn");
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }

                [Fact]
                public async void GivenReturnToPreview_AssignsItToTheViewModel()
                {
                    var response = await _subject.Initiated("0001", true);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<FeaturesViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }
            }

            public class PostTests : InitiatedTests
            {
                [Fact]
                public async void GivenUrnAndInitiator_GetsProjectFromRepository()
                {
                    FeaturesViewModel vm = new FeaturesViewModel
                    {
                        Project = new Project { Urn = "0001" },
                        WhoInitiated = TransferFeatures.ProjectInitiators.Dfe.ToString()
                    };
                    await _subject.InitiatedPost(vm);
                    _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
                }

                [Fact]
                public async void GivenUrnAndInitiator_RedirectsProjectToFeaturesSummary()
                {
                    FeaturesViewModel vm = new FeaturesViewModel
                    {
                        Project = new Project { Urn = "0001" },
                        WhoInitiated = TransferFeatures.ProjectInitiators.Dfe.ToString()
                    };
                    var request = await _subject.InitiatedPost(vm);

                    var redirectResponse = Assert.IsType<RedirectToActionResult>(request);
                    Assert.Equal("Index", redirectResponse.ActionName);
                }

                [Theory]
                [InlineData(TransferFeatures.ProjectInitiators.Dfe)]
                [InlineData(TransferFeatures.ProjectInitiators.OutgoingTrust)]
                public async void GivenUrnAndWhoInitiated_AssignsTheCorrectEnumAndUpdatesTheProject(
                    TransferFeatures.ProjectInitiators whoInitiated)
                {
                    FeaturesViewModel vm = new FeaturesViewModel
                    {
                        Project = new Project { Urn = "0001" },
                        WhoInitiated = whoInitiated.ToString()
                    };
                    await _subject.InitiatedPost(vm);
                    _projectRepository.Verify(
                        r => r.Update(
                            It.Is<Project>(project => project.Features.WhoInitiatedTheTransfer == whoInitiated))
                    );
                }

                [Fact]
                public async void GivenEmptyInitiator_AddAnErrorMessageToThePageAndDoNotUpdateTheModel()
                {
                    FeaturesViewModel vm = new FeaturesViewModel
                    {
                        Project = new Project { Urn = "0001" },
                        WhoInitiated = string.Empty
                    };

                    //Validate object with empty Initiator and post to controller
                    var validator = new FeaturesValidator();
                    var results = validator.Validate(vm);
                    results.AddToModelState(_subject.ModelState, null);
                    var response = await _subject.InitiatedPost(vm);
                    
                    //Assert
                    _projectRepository.Verify(r => r.Update(It.IsAny<Project>()), Times.Never);
                    Assert.False(results.IsValid);
                    Assert.Equal("WhoInitiated", results.Errors[0].PropertyName);
                    Assert.Equal("Select who initiated the project", results.Errors[0].ErrorMessage);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    FeaturesViewModel vm = new FeaturesViewModel
                    {
                        Project = new Project { Urn = "errorUrn" },
                        WhoInitiated = TransferFeatures.ProjectInitiators.Dfe.ToString()
                    };
                    var response = await _subject.InitiatedPost(vm);
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }

                [Fact]
                public async void GivenUpdateReturnsError_DisplayErrorPage()
                {
                    _projectRepository.Setup(r => r.Update(It.IsAny<Project>()))
                        .ReturnsAsync(new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                StatusCode = System.Net.HttpStatusCode.NotFound,
                                ErrorMessage = "Project not found"
                            }
                        });

                    var controller = new FeaturesController(_projectRepository.Object);

                    FeaturesViewModel vm = new FeaturesViewModel
                    {
                        Project = new Project { Urn = "0001" },
                        WhoInitiated = TransferFeatures.ProjectInitiators.Dfe.ToString()
                    };
                    var response = await controller.InitiatedPost(vm);
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }

                [Fact]
                public async void GivenReturnToPreviewWithInvalidInput_AssignToTheView()
                {
                    FeaturesViewModel vm = new FeaturesViewModel
                    {
                        Project = new Project { Urn = "0001" },
                        ReturnToPreview = true,
                        WhoInitiated = string.Empty
                    };

                    //Validate object and post to controller
                    var validator = new FeaturesValidator();
                    var results = validator.Validate(vm);
                    results.AddToModelState(_subject.ModelState, null);
                    var response = await _subject.InitiatedPost(vm);

                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<FeaturesViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectsToPreviewPage()
                {
                    FeaturesViewModel vm = new FeaturesViewModel
                    {
                        Project = new Project { Urn = "0001" },
                        ReturnToPreview = true,
                        WhoInitiated = TransferFeatures.ProjectInitiators.Dfe.ToString()
                    };
                    var response = await _subject.InitiatedPost(vm);

                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response, Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new { id = "0001" })
                    );
                }
            }
        }

        public class ReasonTests : FeaturesControllerTests
        {
            public class GetTests : ReasonTests
            {
                [Fact]
                public async void GivenUrn_GetsProjectAndAssignsToTheView()
                {
                    var request = new Func<Task<IActionResult>>(async () => await _subject.Reason("0001"));
                    await AssertProjectIsGottenFromRepositoryAndAssignedToView(request);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.Reason("errorUrn");
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }

                [Fact]
                public async void GivenReturnToPreview_AssignsItToTheViewModel()
                {
                    var response = await _subject.Reason("0001", true);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<FeaturesViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }
            }

            public class PostTests : ReasonTests
            {
                [Fact]
                public async void GivenSubjectToInterventionAndReason_UpdatesTheProject()
                {
                    await _subject.ReasonPost("0001", true, "More detail");
                    _projectRepository.Verify(r => r.Update(It.Is<Project>(project =>
                        project.Urn == "0001" &&
                        project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == true &&
                        project.Features.ReasonForTransfer.InterventionDetails == "More detail")), Times.Once);
                }

                [Fact]
                public async void GivenSubjectToInterventionAndReason_RedirectsToSummaryPage()
                {
                    var result = await _subject.ReasonPost("0001", true, "More detail");
                    var redirect = Assert.IsType<RedirectToActionResult>(result);
                    Assert.Equal("Index", redirect.ActionName);
                }

                [Fact]
                public async void GivenNotSubjectToInterventionAndNoReason_UpdatesTheProject()
                {
                    await _subject.ReasonPost("0001", false, null);
                    _projectRepository.Verify(r => r.Update(It.Is<Project>(project =>
                        project.Urn == "0001" &&
                        project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == false &&
                        project.Features.ReasonForTransfer.InterventionDetails == string.Empty)), Times.Once);
                }

                [Fact]
                public async void GivenNothingSubmitted_DoesNotUpdateTheProject()
                {
                    await _subject.ReasonPost("0001", null, null);
                    _projectRepository.Verify(r => r.Update(It.IsAny<Project>()), Times.Never);
                }

                [Fact]
                public async void GivenNothingSubmitted_SetsAnErrorWithMessageOnTheViewModel()
                {
                    var result = await _subject.ReasonPost("0001", null, null);
                    var viewModel = GetViewModel(result);

                    Assert.True(viewModel.FormErrors.HasErrors);
                    var error = viewModel.FormErrors.Errors[0];
                    Assert.Equal("Select whether or not the transfer is subject to intervention", error.ErrorMessage);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.ReasonPost("errorUrn", false, "test");
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }

                [Fact]
                public async void GivenUpdateReturnsError_DisplayErrorPage()
                {
                    _projectRepository.Setup(r => r.Update(It.IsAny<Project>()))
                        .ReturnsAsync(new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                StatusCode = System.Net.HttpStatusCode.NotFound,
                                ErrorMessage = "Project not found"
                            }
                        });

                    var controller = new FeaturesController(_projectRepository.Object);

                    var response = await controller.ReasonPost("0001", false, "test");
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }

                [Fact]
                public async void GivenReturnToPreviewWithInvalidInput_AssignToTheView()
                {
                    var response = await _subject.ReasonPost("0001", null, null, true);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<FeaturesViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectsToPreviewPage()
                {
                    var response = await _subject.ReasonPost("0001", true, "Meow", true);

                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response, Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new { id = "0001" })
                    );
                }
            }
        }

        public class TypeTests : FeaturesControllerTests
        {
            public class GetTests : TypeTests
            {
                [Fact]
                public async void GivenUrn_GetsProjectAndAssignsToTheView()
                {
                    var request = new Func<Task<IActionResult>>(async () => await _subject.Type("0001"));
                    await AssertProjectIsGottenFromRepositoryAndAssignedToView(request);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.Type("errorUrn");
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }

                [Fact]
                public async void GivenReturnToPreview_AssignsItToTheViewModel()
                {
                    var response = await _subject.Type("0001", true);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<FeaturesViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }
            }

            public class PostTests : TypeTests
            {
                [Theory]
                [InlineData(TransferFeatures.TransferTypes.MatClosure)]
                [InlineData(TransferFeatures.TransferTypes.TrustsMerging)]
                public async void GivenNonOtherType_UpdatesTheProject(TransferFeatures.TransferTypes transferType)
                {
                    await _subject.TypePost("0001", transferType, "");

                    _projectRepository.Verify(
                        r => r.Update(It.Is<Project>(project => project.Features.TypeOfTransfer == transferType)),
                        Times.Once);
                }

                [Fact]
                public async void GivenOtherTypeAndText_UpdatesTheProject()
                {
                    await _subject.TypePost("0001", TransferFeatures.TransferTypes.Other, "Other");

                    _projectRepository.Verify(
                        r => r.Update(It.Is<Project>(project =>
                            project.Features.TypeOfTransfer == TransferFeatures.TransferTypes.Other &&
                            project.Features.OtherTypeOfTransfer == "Other")),
                        Times.Once);
                }

                [Fact]
                public async void GivenNoTransferType_SetErrorOnTheView()
                {
                    var response = await _subject.TypePost("0001", TransferFeatures.TransferTypes.Empty, null);
                    var viewModel = GetViewModel(response);
                    Assert.True(viewModel.FormErrors.HasErrors);
                    Assert.Equal("Select the type of transfer",
                        viewModel.FormErrors.ErrorForField("typeOfTransfer").ErrorMessage);
                }

                [Fact]
                public async void GivenOtherTransferTypeButNoText_SetErrorOnTheView()
                {
                    var response = await _subject.TypePost("0001", TransferFeatures.TransferTypes.Other, null);
                    var viewModel = GetViewModel(response);
                    Assert.True(viewModel.FormErrors.HasErrors);
                    Assert.Equal("Enter the type of transfer",
                        viewModel.FormErrors.ErrorForField("otherType").ErrorMessage);
                }

                [Fact]
                public async void GivenTypeOfTransfer_RedirectsToIndex()
                {
                    var result = await _subject.TypePost("0001", TransferFeatures.TransferTypes.SatClosure, null);
                    var redirect = Assert.IsType<RedirectToActionResult>(result);
                    Assert.Equal("Index", redirect.ActionName);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.TypePost("errorUrn", TransferFeatures.TransferTypes.SatClosure, null);
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }

                [Fact]
                public async void GivenReturnToPreviewWithError_AssignsItToTheViewModel()
                {
                    var response = await _subject.TypePost("0001", TransferFeatures.TransferTypes.Empty, "", true);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<FeaturesViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectsToPreviewPage()
                {
                    var response = await _subject.TypePost("0001", TransferFeatures.TransferTypes.MatClosure, "Meow", true);

                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response, Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new { id = "0001" })
                    );
                }

                [Fact]
                public async void GivenUpdateReturnsError_DisplayErrorPage()
                {
                    _projectRepository.Setup(r => r.Update(It.IsAny<Project>()))
                        .ReturnsAsync(new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                StatusCode = System.Net.HttpStatusCode.NotFound,
                                ErrorMessage = "Project not found"
                            }
                        });

                    var controller = new FeaturesController(_projectRepository.Object);

                    var response = await controller.TypePost("0001", TransferFeatures.TransferTypes.SatClosure, null);
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }
            }
        }

        #region Helpers

        private async Task AssertProjectIsGottenFromRepositoryAndAssignedToView(Func<Task<IActionResult>> request)
        {
            var result = await request();
            _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
            var viewModel = GetViewModel(result);

            Assert.Equal(_foundProject, viewModel.Project);
        }

        private static FeaturesViewModel GetViewModel(IActionResult result)
        {
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<FeaturesViewModel>(viewResult.Model);
            return viewModel;
        }

        #endregion
    }
}