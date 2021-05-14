using System;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.Projects;
using DocumentFormat.OpenXml.Presentation;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
                }
            };

            _projectRepository.Setup(r => r.GetByUrn("0001")).ReturnsAsync(new RepositoryResult<Project>()
            {
                Result = _foundProject
            });
        }

        public class IndexTests : FeaturesControllerTests
        {
            [Fact]
            public async void GivenUrn_GetsProjectFromRepositoryAndAssignsToTheView()
            {
                var request = new Func<Task<IActionResult>>(async () => await _subject.Index("0001"));
                await AssertProjectIsGottenFromRepositoryAndAssignedToView(request);
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
            }

            public class PostTests : InitiatedTests
            {
                [Fact]
                public async void GivenUrnAndInitiator_GetsProjectFromRepository()
                {
                    await _subject.InitiatedPost("0001", TransferFeatures.ProjectInitiators.Dfe);
                    _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
                }

                [Fact]
                public async void GivenUrnAndInitiator_RedirectsProjectToFeaturesSummary()
                {
                    var request = await _subject.InitiatedPost("0001", TransferFeatures.ProjectInitiators.Dfe);

                    var redirectResponse = Assert.IsType<RedirectToActionResult>(request);
                    Assert.Equal("Index", redirectResponse.ActionName);
                }

                [Theory]
                [InlineData(TransferFeatures.ProjectInitiators.Dfe)]
                [InlineData(TransferFeatures.ProjectInitiators.OutgoingTrust)]
                public async void GivenUrnAndWhoInitiated_AssignsTheCorrectEnumAndUpdatesTheProject(
                    TransferFeatures.ProjectInitiators whoInitiated)
                {
                    await _subject.InitiatedPost("0001", whoInitiated);
                    _projectRepository.Verify(
                        r => r.Update(
                            It.Is<Project>(project => project.Features.WhoInitiatedTheTransfer == whoInitiated))
                    );
                }

                [Fact]
                public async void GivenEmptyInitiator_AddAnErrorMessageToThePageAndDoNotUpdateTheModel()
                {
                    var response = await _subject.InitiatedPost("0001", TransferFeatures.ProjectInitiators.Empty);
                    var viewModel = GetViewModel(response);
                    _projectRepository.Verify(r => r.Update(It.IsAny<Project>()), Times.Never);
                    Assert.True(viewModel.FormErrors.HasErrors);
                    var error = viewModel.FormErrors.Errors[0];
                    Assert.Equal("Please select who initiated the project", error.ErrorMessage);
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
                        project.Features.ReasonForTransfer.InterventionDetails == null)), Times.Once);
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
                    Assert.Equal("Please select the type of transfer",
                        viewModel.FormErrors.ErrorForField("typeOfTransfer").ErrorMessage);
                }
                
                [Fact]
                public async void GivenOtherTransferTypeButNoText_SetErrorOnTheView()
                {
                    var response = await _subject.TypePost("0001", TransferFeatures.TransferTypes.Other, null);
                    var viewModel = GetViewModel(response);
                    Assert.True(viewModel.FormErrors.HasErrors);
                    Assert.Equal("Please enter the type of transfer",
                        viewModel.FormErrors.ErrorForField("otherType").ErrorMessage);
                }

                [Fact]
                public async void GivenTypeOfTransfer_RedirectsToIndex()
                {
                    var result = await _subject.TypePost("0001", TransferFeatures.TransferTypes.SatClosure, null);
                    var redirect = Assert.IsType<RedirectToActionResult>(result);
                    Assert.Equal("Index", redirect.ActionName);
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