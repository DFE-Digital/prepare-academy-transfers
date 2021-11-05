using System;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.Projects;
using FluentValidation;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Tests.Helpers;
using Frontend.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Frontend.Models.Features;

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
                },
                TransferringAcademies = new System.Collections.Generic.List<TransferringAcademies>
                {
                    new TransferringAcademies{OutgoingAcademyName="Outgoing Academy"}
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
                await request();
                _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
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
                    await request();
                    _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
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
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<FeaturesInitiatedViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }
            }

            public class PostTests : InitiatedTests
            {
                [Fact]
                public async void GivenUrnAndInitiator_GetsProjectFromRepository()
                {
                    FeaturesInitiatedViewModel vm = new FeaturesInitiatedViewModel
                    {
                        Urn = "0001",
                        WhoInitiated = TransferFeatures.ProjectInitiators.Dfe
                    };
                    await _subject.InitiatedPost(vm);
                    _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
                }

                [Fact]
                public async void GivenUrnAndInitiator_RedirectsProjectToFeaturesSummary()
                {
                    FeaturesInitiatedViewModel vm = new FeaturesInitiatedViewModel
                    {
                        Urn = "0001",
                        WhoInitiated = TransferFeatures.ProjectInitiators.Dfe
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
                    FeaturesInitiatedViewModel vm = new FeaturesInitiatedViewModel
                    {
                        Urn = "0001",
                        WhoInitiated = whoInitiated
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
                    FeaturesInitiatedViewModel vm = new FeaturesInitiatedViewModel
                    {
                        Urn = "0001",
                        WhoInitiated = TransferFeatures.ProjectInitiators.Empty
                    };

                    var results = await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesInitiatedValidator(), vm, _subject.ModelState);
                    var response = await _subject.InitiatedPost(vm);

                    //Assert
                    _projectRepository.Verify(r => r.Update(It.IsAny<Project>()), Times.Never);

                    Assert.False(results.IsValid);
                    Assert.Equal("WhoInitiated", results.Errors[0].PropertyName);
                    Assert.Equal("Select who initiated the project", results.Errors[0].ErrorMessage);
                    ControllerTestHelpers.GetViewModelFromResult<FeaturesInitiatedViewModel>(response);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    FeaturesInitiatedViewModel vm = new FeaturesInitiatedViewModel
                    {
                        Urn = "errorUrn",
                        WhoInitiated = TransferFeatures.ProjectInitiators.Dfe
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

                    FeaturesInitiatedViewModel vm = new FeaturesInitiatedViewModel
                    {
                        Urn = "0001",
                        WhoInitiated = TransferFeatures.ProjectInitiators.Dfe
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
                    FeaturesInitiatedViewModel vm = new FeaturesInitiatedViewModel
                    {
                        Urn = "0001",
                        ReturnToPreview = true,
                        WhoInitiated = TransferFeatures.ProjectInitiators.Empty
                    };

                    await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesInitiatedValidator(), vm, _subject.ModelState);
                    var response = await _subject.InitiatedPost(vm);

                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<FeaturesInitiatedViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectsToPreviewPage()
                {
                    FeaturesInitiatedViewModel vm = new FeaturesInitiatedViewModel
                    {
                        Urn = "0001",
                        ReturnToPreview = true,
                        WhoInitiated = TransferFeatures.ProjectInitiators.Dfe
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
                    var result = await request();
                    _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
                    var vm = ControllerTestHelpers.GetViewModelFromResult<FeaturesReasonViewModel>(result);

                    Assert.Equal(_foundProject.OutgoingAcademyName, vm.OutgoingAcademyName);
                    Assert.IsType<FeaturesReasonViewModel>(vm);
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
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<FeaturesReasonViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }
            }

            public class PostTests : ReasonTests
            {
                [Fact]
                public async void GivenSubjectToInterventionAndReason_UpdatesTheProject()
                {
                    var vm = new FeaturesReasonViewModel
                    {
                        Urn = "0001",
                        IsSubjectToIntervention = true,
                        MoreDetail = "More detail"
                    };
                    await _subject.ReasonPost(vm);
                    _projectRepository.Verify(r => r.Update(It.Is<Project>(project =>
                        project.Urn == "0001" &&
                        project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == true &&
                        project.Features.ReasonForTransfer.InterventionDetails == "More detail")), Times.Once);
                }

                [Fact]
                public async void GivenSubjectToInterventionAndReason_RedirectsToSummaryPage()
                {
                    var vm = new FeaturesReasonViewModel
                    {
                        Urn = "0001",
                        IsSubjectToIntervention = true,
                        MoreDetail = "More detail"
                    };
                    var result = await _subject.ReasonPost(vm);
                    var redirect = Assert.IsType<RedirectToActionResult>(result);
                    Assert.Equal("Index", redirect.ActionName);
                }

                [Fact]
                public async void GivenNotSubjectToInterventionAndNoReason_UpdatesTheProject()
                {
                    var vm = new FeaturesReasonViewModel
                    {
                        Urn = "0001",
                        IsSubjectToIntervention = false
                    };
                    await _subject.ReasonPost(vm);
                    _projectRepository.Verify(r => r.Update(It.Is<Project>(project =>
                        project.Urn == "0001" &&
                        project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == false &&
                        project.Features.ReasonForTransfer.InterventionDetails == string.Empty)), Times.Once);
                }

                [Fact]
                public async void GivenNothingSubmitted_DoesNotUpdateTheProject()
                {
                    var vm = new FeaturesReasonViewModel
                    {
                        Urn = "0001",
                    };
                    await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesReasonValidator(), vm, _subject.ModelState);
                    await _subject.ReasonPost(vm);

                    _projectRepository.Verify(r => r.Update(It.IsAny<Project>()), Times.Never);
                }

                [Fact]
                public async void GivenNothingSubmitted_SetsAnErrorWithMessageOnTheViewModel()
                {
                    var vm = new FeaturesReasonViewModel
                    {
                        Urn = "0001",
                    };

                    var results = await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesReasonValidator(), vm, _subject.ModelState);
                    var resultPost = await _subject.ReasonPost(vm);

                    Assert.False(results.IsValid);
                    Assert.Equal(nameof(vm.IsSubjectToIntervention), results.Errors[0].PropertyName);
                    Assert.Equal("Select whether or not the transfer is subject to intervention", results.Errors[0].ErrorMessage);
                    ControllerTestHelpers.GetViewModelFromResult<FeaturesReasonViewModel>(resultPost);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var vm = new FeaturesReasonViewModel
                    {
                        Urn = "errorUrn",
                        IsSubjectToIntervention = false,
                        MoreDetail = "test"
                    };
                    var response = await _subject.ReasonPost(vm);
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

                    var vm = new FeaturesReasonViewModel
                    {
                        Urn = "0001",
                        IsSubjectToIntervention = false,
                        MoreDetail = "test"
                    };

                    var response = await controller.ReasonPost(vm);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    var viewResult = Assert.IsType<ViewResult>(response);
                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }

                [Fact]
                public async void GivenReturnToPreviewWithInvalidInput_AssignToTheView()
                {
                    var vm = new FeaturesReasonViewModel
                    {
                        Urn = "0001",
                        ReturnToPreview = true
                    };

                    await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesReasonValidator(), vm, _subject.ModelState);

                    var response = await _subject.ReasonPost(vm);

                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<FeaturesReasonViewModel>(response);
                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectsToPreviewPage()
                {
                    var vm = new FeaturesReasonViewModel
                    {
                        Urn = "0001",
                        IsSubjectToIntervention = true,
                        MoreDetail = "Meow",
                        ReturnToPreview = true
                    };
                    var response = await _subject.ReasonPost(vm);

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
                    await request();
                    _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
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

                //TODO:
                //[Fact]
                //public async void GivenNoTransferType_SetErrorOnTheView()
                //{
                //    var response = await _subject.TypePost("0001", TransferFeatures.TransferTypes.Empty, null);
                //    var viewModel = GetViewModel(response);
                //    Assert.True(viewModel.FormErrors.HasErrors);
                //    Assert.Equal("Select the type of transfer",
                //        viewModel.FormErrors.ErrorForField("typeOfTransfer").ErrorMessage);
                //}

                //[Fact]
                //public async void GivenOtherTransferTypeButNoText_SetErrorOnTheView()
                //{
                //    var response = await _subject.TypePost("0001", TransferFeatures.TransferTypes.Other, null);
                //    var viewModel = GetViewModel(response);
                //    Assert.True(viewModel.FormErrors.HasErrors);
                //    Assert.Equal("Enter the type of transfer",
                //        viewModel.FormErrors.ErrorForField("otherType").ErrorMessage);
                //}

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
    }
}