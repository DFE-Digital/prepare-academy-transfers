using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Models.Features;
using Frontend.Tests.Helpers;
using Frontend.Validators.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

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
        
        public class ReasonTests : FeaturesControllerTests
        {
            // public class GetTests : ReasonTests
            // {
            //     [Fact]
            //     public async void GivenUrn_GetsProject()
            //     {
            //         var request = new Func<Task<IActionResult>>(async () => await _subject.Reason("0001"));
            //         var result = await request();
            //         _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
            //         var vm = ControllerTestHelpers.AssertViewModelFromResult<FeaturesReasonViewModel>(result);
            //
            //         Assert.Equal(_foundProject.OutgoingAcademyName, vm.OutgoingAcademyName);
            //         Assert.IsType<FeaturesReasonViewModel>(vm);
            //     }
            //
            //     [Fact]
            //     public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            //     {
            //         var response = await _subject.Reason("errorUrn");
            //         var viewResult = Assert.IsType<ViewResult>(response);
            //         var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);
            //
            //         Assert.Equal("ErrorPage", viewResult.ViewName);
            //         Assert.Equal("Project not found", viewModel);
            //     }
            //
            //     [Fact]
            //     public async void GivenReturnToPreview_AssignsItToTheViewModel()
            //     {
            //         var response = await _subject.Reason("0001", true);
            //         var viewModel = ControllerTestHelpers.AssertViewModelFromResult<FeaturesReasonViewModel>(response);
            //
            //         Assert.True(viewModel.ReturnToPreview);
            //     }
            // }

            // public class PostTests : ReasonTests
            // {
            //     [Fact]
            //     public async void GivenSubjectToInterventionAndReason_UpdatesTheProject()
            //     {
            //         var vm = new FeaturesReasonViewModel
            //         {
            //             IsSubjectToIntervention = true,
            //             MoreDetail = "More detail"
            //         };
            //         await _subject.ReasonPost(vm);
            //         _projectRepository.Verify(r => r.Update(It.Is<Project>(project =>
            //             project.Urn == "0001" &&
            //             project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == true &&
            //             project.Features.ReasonForTransfer.InterventionDetails == "More detail")), Times.Once);
            //     }
            //
            //     [Fact]
            //     public async void GivenSubjectToInterventionAndReason_RedirectsToSummaryPage()
            //     {
            //         var urn = "0001";
            //         var vm = new FeaturesReasonViewModel
            //         {
            //             Urn = urn,
            //             IsSubjectToIntervention = true,
            //             MoreDetail = "More detail"
            //         };
            //         var result = await _subject.ReasonPost(vm);
            //         ControllerTestHelpers.AssertResultRedirectsToPage(result, "/Projects/Features/Index",
            //             new RouteValueDictionary(new {urn}));
            //     }
            //
            //     [Fact]
            //     public async void GivenNotSubjectToInterventionAndNoReason_UpdatesTheProject()
            //     {
            //         var vm = new FeaturesReasonViewModel
            //         {
            //             Urn = "0001",
            //             IsSubjectToIntervention = false
            //         };
            //         await _subject.ReasonPost(vm);
            //         _projectRepository.Verify(r => r.Update(It.Is<Project>(project =>
            //             project.Urn == "0001" &&
            //             project.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention == false &&
            //             project.Features.ReasonForTransfer.InterventionDetails == string.Empty)), Times.Once);
            //     }
            //
            //     [Fact]
            //     public async void GivenNothingSubmitted_DoesNotUpdateTheProject()
            //     {
            //         var vm = new FeaturesReasonViewModel
            //         {
            //             Urn = "0001",
            //         };
            //         await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesReasonValidator(), vm, _subject.ModelState);
            //         await _subject.ReasonPost(vm);
            //
            //         _projectRepository.Verify(r => r.Update(It.IsAny<Project>()), Times.Never);
            //     }
            //
            //     [Fact]
            //     public async void GivenNothingSubmitted_ViewModelReturned()
            //     {
            //         var vm = new FeaturesReasonViewModel
            //         {
            //             Urn = "0001",
            //         };
            //
            //         await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesReasonValidator(), vm, _subject.ModelState);
            //         var resultPost = await _subject.ReasonPost(vm);
            //
            //         ControllerTestHelpers.AssertViewModelFromResult<FeaturesReasonViewModel>(resultPost);
            //         Assert.False(_subject.ModelState.IsValid);
            //     }
            //
            //     [Fact]
            //     public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            //     {
            //         var vm = new FeaturesReasonViewModel
            //         {
            //             Urn = "errorUrn",
            //             IsSubjectToIntervention = false,
            //             MoreDetail = "test"
            //         };
            //         var response = await _subject.ReasonPost(vm);
            //         var viewResult = Assert.IsType<ViewResult>(response);
            //         var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);
            //
            //         Assert.Equal("ErrorPage", viewResult.ViewName);
            //         Assert.Equal("Project not found", viewModel);
            //     }
            //
            //     [Fact]
            //     public async void GivenUpdateReturnsError_DisplayErrorPage()
            //     {
            //         _projectRepository.Setup(r => r.Update(It.IsAny<Project>()))
            //             .ReturnsAsync(new RepositoryResult<Project>
            //             {
            //                 Error = new RepositoryResultBase.RepositoryError
            //                 {
            //                     StatusCode = System.Net.HttpStatusCode.NotFound,
            //                     ErrorMessage = "Project not found"
            //                 }
            //             });
            //
            //         var controller = new FeaturesController(_projectRepository.Object);
            //
            //         var vm = new FeaturesReasonViewModel
            //         {
            //             Urn = "0001",
            //             IsSubjectToIntervention = false,
            //             MoreDetail = "test"
            //         };
            //
            //         var response = await controller.ReasonPost(vm);
            //         var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);
            //
            //         var viewResult = Assert.IsType<ViewResult>(response);
            //         Assert.Equal("ErrorPage", viewResult.ViewName);
            //         Assert.Equal("Project not found", viewModel);
            //     }
            //
            //     [Fact]
            //     public async void GivenReturnToPreviewWithInvalidInput_AssignToTheView()
            //     {
            //         var vm = new FeaturesReasonViewModel
            //         {
            //             Urn = "0001",
            //             ReturnToPreview = true
            //         };
            //
            //         await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesReasonValidator(), vm, _subject.ModelState);
            //
            //         var response = await _subject.ReasonPost(vm);
            //
            //         var viewModel = ControllerTestHelpers.AssertViewModelFromResult<FeaturesReasonViewModel>(response);
            //         Assert.True(viewModel.ReturnToPreview);
            //     }
            //
            //     [Fact]
            //     public async void GivenReturnToPreview_RedirectsToPreviewPage()
            //     {
            //         var vm = new FeaturesReasonViewModel
            //         {
            //             Urn = "0001",
            //             IsSubjectToIntervention = true,
            //             MoreDetail = "Meow",
            //             ReturnToPreview = true
            //         };
            //         var response = await _subject.ReasonPost(vm);
            //
            //         ControllerTestHelpers.AssertResultRedirectsToPage(
            //             response, Links.HeadteacherBoard.Preview.PageName,
            //             new RouteValueDictionary(new { id = "0001" })
            //         );
            //     }
            // }
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
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }

                [Fact]
                public async void GivenReturnToPreview_AssignsItToTheViewModel()
                {
                    var response = await _subject.Type("0001", true);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<FeaturesTypeViewModel>(response);

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
                    var vm = new FeaturesTypeViewModel
                    {
                        Urn = "0001",
                        TypeOfTransfer = transferType,
                        OtherType = string.Empty
                    };
                    await _subject.TypePost(vm);

                    _projectRepository.Verify(
                        r => r.Update(It.Is<Project>(project => project.Features.TypeOfTransfer == transferType)),
                        Times.Once);
                }

                [Fact]
                public async void GivenOtherTypeAndText_UpdatesTheProject()
                {
                    var vm = new FeaturesTypeViewModel
                    {
                        Urn = "0001",
                        TypeOfTransfer = TransferFeatures.TransferTypes.Other,
                        OtherType = "Other"
                    };
                    await _subject.TypePost(vm);

                    _projectRepository.Verify(
                        r => r.Update(It.Is<Project>(project =>
                            project.Features.TypeOfTransfer == TransferFeatures.TransferTypes.Other &&
                            project.Features.OtherTypeOfTransfer == "Other")),
                        Times.Once);
                }

                
                [Fact]
                public async void GivenNoTransferType_SetErrorOnTheView()
                {
                    var vm = new FeaturesTypeViewModel
                    {
                        Urn = "0001",
                        TypeOfTransfer = TransferFeatures.TransferTypes.Empty
                    };
                    
                    await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesTypeValidator(), vm, _subject.ModelState);
                    var response = await _subject.TypePost(vm);

                    ControllerTestHelpers.AssertViewModelFromResult<FeaturesTypeViewModel>(response);
                    Assert.False(_subject.ModelState.IsValid);
                }


                [Fact]
                public async void GivenOtherTransferTypeButNoText_SetErrorOnTheView()
                {
                    var vm = new FeaturesTypeViewModel
                    {
                        Urn = "0001",
                        TypeOfTransfer = TransferFeatures.TransferTypes.Other
                    };

                    await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesTypeValidator(), vm, _subject.ModelState);
                    var response = await _subject.TypePost(vm);

                    ControllerTestHelpers.AssertViewModelFromResult<FeaturesTypeViewModel>(response);
                    Assert.False(_subject.ModelState.IsValid);
                }

                [Fact]
                public async void GivenTypeOfTransfer_RedirectsToIndex()
                {
                    var urn = "0001";
                    var vm = new FeaturesTypeViewModel
                    {
                        Urn = urn,
                        TypeOfTransfer = TransferFeatures.TransferTypes.SatClosure
                    };

                    var result = await _subject.TypePost(vm);
                    ControllerTestHelpers.AssertResultRedirectsToPage(result, "/Projects/Features/Index",
                        new RouteValueDictionary(new {urn}));
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var vm = new FeaturesTypeViewModel
                    {
                        Urn = "errorUrn",
                        TypeOfTransfer = TransferFeatures.TransferTypes.SatClosure
                    };

                    var response = await _subject.TypePost(vm);
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }

                [Fact]
                public async void GivenReturnToPreviewWithError_AssignsItToTheViewModel()
                {
                    var vm = new FeaturesTypeViewModel
                    {
                        Urn = "0001",
                        TypeOfTransfer = TransferFeatures.TransferTypes.Empty,
                        ReturnToPreview = true
                    };
                    
                    await ControllerTestHelpers.ValidateAndAddToModelState(new FeaturesTypeValidator(), vm, _subject.ModelState);
                    var response = await _subject.TypePost(vm);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<FeaturesTypeViewModel>(response);
                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectsToPreviewPage()
                {
                    var vm = new FeaturesTypeViewModel
                    {
                        Urn = "0001",
                        TypeOfTransfer = TransferFeatures.TransferTypes.MatClosure,
                        OtherType = "Meow",
                        ReturnToPreview = true
                    };
                    var response = await _subject.TypePost(vm);

                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response, Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new { id = "0001" })
                    );
                }

                [Fact]
                public async void GivenUpdateReturnsError_DisplayErrorPage()
                {
                    var vm = new FeaturesTypeViewModel
                    {
                        Urn = "0001",
                        TypeOfTransfer = TransferFeatures.TransferTypes.SatClosure
                    };

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

                    var response = await controller.TypePost(vm);
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }
            }
        }
    }
}