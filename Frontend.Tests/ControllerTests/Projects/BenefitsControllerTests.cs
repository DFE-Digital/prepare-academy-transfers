using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Tests.Helpers;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class BenefitsControllerTests
    {
        private readonly BenefitsController _subject;
        private readonly Mock<IProjects> _projectsRepository;
        private readonly Project _foundProject;

        public BenefitsControllerTests()
        {
            _foundProject = new Project
            {
                Urn = "0001"
            };

            _projectsRepository = new Mock<IProjects>();

            _projectsRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResult<Project> {Result = _foundProject});

            _subject = new BenefitsController(_projectsRepository.Object);
        }

        public class IndexTests : BenefitsControllerTests
        {
            [Fact]
            public async void GivenUrn_AssignsModelToTheView()
            {
                var result = await _subject.Index("0001");
                var viewModel = ControllerTestHelpers.GetViewModelFromResult<BenefitsViewModel>(result);

                Assert.Equal(_foundProject.Urn, viewModel.Project.Urn);
            }
        }

        public class IntendedBenefitsTests : BenefitsControllerTests
        {
            public class GetTests : IntendedBenefitsTests
            {
                [Fact]
                public async void GivenUrn_AssignsModelToTheView()
                {
                    var result = await _subject.IntendedBenefits("0001");
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<BenefitsViewModel>(result);

                    Assert.Equal(_foundProject.Urn, viewModel.Project.Urn);
                }
            }

            public class PostTests : IntendedBenefitsTests
            {
                [Fact]
                public async void GivenUrnAndIntendedBenefits_UpdatesTheProject()
                {
                    var intendedBenefits = new List<TransferBenefits.IntendedBenefit>
                    {
                        TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
                        TransferBenefits.IntendedBenefit.StrengtheningGovernance
                    };

                    await _subject.IntendedBenefitsPost("0001", intendedBenefits.ToArray(), "");

                    _projectsRepository.Verify(r =>
                        r.Update(It.Is<Project>(
                            project => project.TransferBenefits.IntendedBenefits.All(intendedBenefits.Contains))));
                }

                [Fact]
                public async void GivenOtherBenefitAndItsDetails_UpdatesTheProject()
                {
                    var intendedBenefits = new List<TransferBenefits.IntendedBenefit>
                    {
                        TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
                        TransferBenefits.IntendedBenefit.StrengtheningGovernance,
                        TransferBenefits.IntendedBenefit.Other
                    };

                    await _subject.IntendedBenefitsPost("0001", intendedBenefits.ToArray(), "Other benefit");
                    _projectsRepository.Verify(r =>
                        r.Update(It.Is<Project>(
                            project => project.TransferBenefits.IntendedBenefits.All(intendedBenefits.Contains) &&
                                       project.TransferBenefits.OtherIntendedBenefit == "Other benefit")
                        )
                    );
                }

                [Fact]
                public async void GivenUrnAndIntendedBenefits_RedirectsToTheSummaryPage()
                {
                    var intendedBenefits = new List<TransferBenefits.IntendedBenefit>
                    {
                        TransferBenefits.IntendedBenefit.ImprovingSafeguarding,
                        TransferBenefits.IntendedBenefit.StrengtheningGovernance
                    };

                    var response = await _subject.IntendedBenefitsPost("0001", intendedBenefits.ToArray(), "");

                    ControllerTestHelpers.AssertResultRedirectsToAction(response, "Index");
                }

                [Fact]
                public async void GivenUrnAndNoBenefits_CreateErrorOnTheView()
                {
                    var intendedBenefits = new TransferBenefits.IntendedBenefit[] { };
                    var response = await _subject.IntendedBenefitsPost("0001", intendedBenefits, "");
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<BenefitsViewModel>(response);
                    Assert.True(viewModel.FormErrors.HasErrors);
                    Assert.True(viewModel.FormErrors.HasErrorForField("intendedBenefits"));
                }

                [Fact]
                public async void GivenOtherBenefitButNoDescription_CreateErrorOnTheView()
                {
                    var intendedBenefits = new[] {TransferBenefits.IntendedBenefit.Other};
                    var response = await _subject.IntendedBenefitsPost("0001", intendedBenefits, "");
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<BenefitsViewModel>(response);
                    Assert.True(viewModel.FormErrors.HasErrors);
                    Assert.True(viewModel.FormErrors.HasErrorForField("otherBenefit"));
                }

                [Fact]
                public async void GivenManyBenefitsIncludingOtherButNoDescription_CreateErrorOnTheView()
                {
                    var intendedBenefits = new[]
                    {
                        TransferBenefits.IntendedBenefit.ImprovingSafeguarding, TransferBenefits.IntendedBenefit.Other
                    };
                    var response = await _subject.IntendedBenefitsPost("0001", intendedBenefits, "");
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<BenefitsViewModel>(response);
                    Assert.True(viewModel.FormErrors.HasErrors);
                    Assert.True(viewModel.FormErrors.HasErrorForField("otherBenefit"));
                }
            }
        }
    }
}