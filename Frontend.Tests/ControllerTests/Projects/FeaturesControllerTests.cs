using System;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.Projects;
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
                Features = new TransferFeatures()
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
                await AssertProjectIsGottenFromRepositoryAndAssignedToView<FeaturesViewModel>(request);
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
                    await AssertProjectIsGottenFromRepositoryAndAssignedToView<FeaturesViewModel>(request);
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
                    var viewModel = GetViewModel<FeaturesViewModel>(response);
                    _projectRepository.Verify(r => r.Update(It.IsAny<Project>()), Times.Never);
                    Assert.True(viewModel.HasError);
                    Assert.Equal("Please select who initiated the project", viewModel.Error);
                }
            }
        }

        #region Helpers

        private async Task AssertProjectIsGottenFromRepositoryAndAssignedToView<TViewModel>(
            Func<Task<IActionResult>> request)
            where TViewModel : ProjectViewModel
        {
            var result = await request();
            _projectRepository.Verify(r => r.GetByUrn("0001"), Times.Once);
            var viewModel = GetViewModel<TViewModel>(result);

            Assert.Equal(_foundProject, viewModel.Project);
        }

        private static TViewModel GetViewModel<TViewModel>(IActionResult result) where TViewModel : ProjectViewModel
        {
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<TViewModel>(viewResult.Model);
            return viewModel;
        }

        #endregion
    }
}