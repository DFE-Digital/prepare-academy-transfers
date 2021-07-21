using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
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

            _projectsRepository.Setup(r => r.Update(It.IsAny<Project>()))
                .ReturnsAsync(new RepositoryResult<Project>());

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

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                _projectsRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                    .ReturnsAsync(new RepositoryResult<Project>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            StatusCode = System.Net.HttpStatusCode.NotFound,
                            ErrorMessage = "Project not found"
                        }
                    });

                var controller = new BenefitsController(_projectsRepository.Object);

                var response = await controller.Index("projectUrn");
                var viewResult = Assert.IsType<ViewResult>(response);
                var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Project not found", viewModel);
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

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    _projectsRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                        .ReturnsAsync(new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                StatusCode = System.Net.HttpStatusCode.NotFound,
                                ErrorMessage = "Project not found"
                            }
                        });

                    var controller = new BenefitsController(_projectsRepository.Object);

                    var response = await controller.IntendedBenefits("projectUrn");
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
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
                            project => project.Benefits.IntendedBenefits.All(intendedBenefits.Contains))));
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
                            project => project.Benefits.IntendedBenefits.All(intendedBenefits.Contains) &&
                                       project.Benefits.OtherIntendedBenefit == "Other benefit")
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

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    _projectsRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                        .ReturnsAsync(new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                StatusCode = System.Net.HttpStatusCode.NotFound,
                                ErrorMessage = "Project not found"
                            }
                        });

                    var controller = new BenefitsController(_projectsRepository.Object);

                    var intendedBenefits = new[]
                    {
                        TransferBenefits.IntendedBenefit.ImprovingSafeguarding, TransferBenefits.IntendedBenefit.Other
                    };

                    var response = await controller.IntendedBenefitsPost("projectUrn", intendedBenefits, "");
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }

                [Fact]
                public async void GivenUpdateReturnsError_DisplayErrorPage()
                {
                    _projectsRepository.Setup(r => r.Update(It.IsAny<Project>()))
                        .ReturnsAsync(new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                StatusCode = System.Net.HttpStatusCode.NotFound,
                                ErrorMessage = "Project not found"
                            }
                        });

                    var controller = new BenefitsController(_projectsRepository.Object);

                    var intendedBenefits = new[]
                    {
                        TransferBenefits.IntendedBenefit.ImprovingSafeguarding, TransferBenefits.IntendedBenefit.CentralFinanceTeamAndSupport
                    };

                    var response = await controller.IntendedBenefitsPost("projectUrn", intendedBenefits, "");
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }
            }
        }

        public class OtherFactorsTests : BenefitsControllerTests
        {
            public class GetTests : OtherFactorsTests
            {
                [Fact]
                public async void GivenUrn_AssignsModelToTheView()
                {
                    var result = await _subject.OtherFactors("0001");
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<BenefitsViewModel>(result);

                    Assert.Equal(_foundProject.Urn, viewModel.Project.Urn);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    _projectsRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                        .ReturnsAsync(new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                StatusCode = System.Net.HttpStatusCode.NotFound,
                                ErrorMessage = "Project not found"
                            }
                        });

                    var controller = new BenefitsController(_projectsRepository.Object);

                    var response = await controller.OtherFactors("projectUrn");
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }
                
                [Fact]
                public async void GivenUrnAndNoOtherFactors_CreateErrorOnTheView()
                {
                    var otherFactors = new List<TransferBenefits.OtherFactor>();
                    var response = await _subject.OtherFactorsPost("0001", otherFactors, "", "", "");
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<BenefitsViewModel>(response);
                    Assert.True(viewModel.FormErrors.HasErrors);
                    Assert.True(viewModel.FormErrors.HasErrorForField("otherFactors"));
                }
            }

            public class PostTests : OtherFactorsTests
            {
                [Fact]
                public async void GivenUrnAndAllOtherFactors_UpdateTheProject()
                {
                    var otherFactorDescriptions = new List<TransferBenefits.OtherFactor>
                    {
                        TransferBenefits.OtherFactor.HighProfile,
                        TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues,
                        TransferBenefits.OtherFactor.FinanceAndDebtConcerns
                    };

                    Func<Project, bool> assertOtherFactorsEqual = project =>
                    {
                        var projectOtherFactors = project.Benefits.OtherFactors;
                        var highProfile = projectOtherFactors[TransferBenefits.OtherFactor.HighProfile];
                        var complexIssues =
                            projectOtherFactors[TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues];
                        var finance = projectOtherFactors[TransferBenefits.OtherFactor.FinanceAndDebtConcerns];

                        return highProfile == "High profile" &&
                               complexIssues == "Complex issues" &&
                               finance == "Finance concerns";
                    };

                    await _subject.OtherFactorsPost("0001", otherFactorDescriptions,
                        "High profile", "Complex issues", "Finance concerns");

                    _projectsRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => assertOtherFactorsEqual(project)))
                    );
                }

                [Fact]
                public async void GivenUrnAndOtherFactors_RedirectsToTheSummaryPage()
                {
                    var otherFactorDescriptions = new List<TransferBenefits.OtherFactor>
                    {
                        TransferBenefits.OtherFactor.HighProfile,
                        TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues,
                        TransferBenefits.OtherFactor.FinanceAndDebtConcerns
                    };


                    var response = await _subject.OtherFactorsPost("0001", otherFactorDescriptions,
                        "High profile", "Complex issues", "Finance concerns");
                    ControllerTestHelpers.AssertResultRedirectsToAction(response, "Index");
                }

                [Fact]
                public async void GivenUrnAndHighProfile_UpdatesTheProjectCorrectly()
                {
                    var otherFactors = new List<TransferBenefits.OtherFactor>
                        {TransferBenefits.OtherFactor.HighProfile};

                    await _subject.OtherFactorsPost("0001", otherFactors, "High profile", "", "");
                    _projectsRepository.Verify(r => r.Update(It.Is<Project>(
                        project => project.Benefits.OtherFactors.Keys.Count == 1 &&
                                   project.Benefits.OtherFactors[TransferBenefits.OtherFactor.HighProfile] ==
                                   "High profile"
                    )));
                }

                [Fact]
                public async void GivenUrnAndComplexLandIssues_UpdatesTheProjectCorrectly()
                {
                    var otherFactors = new List<TransferBenefits.OtherFactor>
                        {TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues};

                    await _subject.OtherFactorsPost("0001", otherFactors, "", "Complex issues", "");
                    _projectsRepository.Verify(r => r.Update(It.Is<Project>(
                        project => project.Benefits.OtherFactors.Keys.Count == 1 &&
                                   project.Benefits.OtherFactors[
                                       TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues] ==
                                   "Complex issues"
                    )));
                }

                [Fact]
                public async void GivenUrnAndFinanceAndDebtConcerns_UpdatesTheProjectCorrectly()
                {
                    var otherFactors = new List<TransferBenefits.OtherFactor>
                        {TransferBenefits.OtherFactor.FinanceAndDebtConcerns};

                    await _subject.OtherFactorsPost("0001", otherFactors, "", "", "Finance concerns");
                    _projectsRepository.Verify(r => r.Update(It.Is<Project>(
                        project => project.Benefits.OtherFactors.Keys.Count == 1 &&
                                   project.Benefits.OtherFactors[
                                       TransferBenefits.OtherFactor.FinanceAndDebtConcerns] ==
                                   "Finance concerns"
                    )));
                }

                [Fact]
                public async void GivenUrnAndOtherFactorsWithNoDescription_UpdatesTheProjectCorrectly()
                {
                    var otherFactors = new List<TransferBenefits.OtherFactor>
                    {
                        TransferBenefits.OtherFactor.HighProfile,
                        TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues,
                        TransferBenefits.OtherFactor.FinanceAndDebtConcerns
                    };

                    await _subject.OtherFactorsPost("0001", otherFactors, "", "", "");
                    _projectsRepository.Verify(r => r.Update(It.Is<Project>(
                        project => project.Benefits.OtherFactors.Keys.Count == 3 &&
                                   project.Benefits.OtherFactors.ContainsKey(TransferBenefits.OtherFactor
                                       .HighProfile) &&
                                   project.Benefits.OtherFactors.ContainsKey(TransferBenefits.OtherFactor
                                       .ComplexLandAndBuildingIssues) &&
                                   project.Benefits.OtherFactors.ContainsKey(TransferBenefits.OtherFactor
                                       .FinanceAndDebtConcerns)
                    )));
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    _projectsRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                        .ReturnsAsync(new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                StatusCode = System.Net.HttpStatusCode.NotFound,
                                ErrorMessage = "Project not found"
                            }
                        });

                    var controller = new BenefitsController(_projectsRepository.Object);

                    var otherFactors = new List<TransferBenefits.OtherFactor>
                        {TransferBenefits.OtherFactor.HighProfile};

                    var response = await controller.OtherFactorsPost("projectUrn", otherFactors, "", "", "");
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }

                [Fact]
                public async void GivenUpdateReturnsError_DisplayErrorPage()
                {
                    _projectsRepository.Setup(r => r.Update(It.IsAny<Project>()))
                        .ReturnsAsync(new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                StatusCode = System.Net.HttpStatusCode.NotFound,
                                ErrorMessage = "Project not found"
                            }
                        });

                    var controller = new BenefitsController(_projectsRepository.Object);

                    var otherFactors = new List<TransferBenefits.OtherFactor>
                        {TransferBenefits.OtherFactor.HighProfile};

                    var response = await controller.OtherFactorsPost("projectUrn", otherFactors, "", "", "");
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }
            }
        }
    }
}