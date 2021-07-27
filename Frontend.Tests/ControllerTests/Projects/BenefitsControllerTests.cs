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
            }

            public class PostTests : OtherFactorsTests
            {
                [Fact]
                public async void GivenUrnAndAllOtherFactors_UpdateTheProject()
                {
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
                    
                    var otherFactors = new List<BenefitsViewModel.OtherFactorsViewModel>
                    {
                        new BenefitsViewModel.OtherFactorsViewModel
                        {
                            OtherFactor = TransferBenefits.OtherFactor.HighProfile,
                            Checked = true,
                            Description = "High profile"
                        },
                        new BenefitsViewModel.OtherFactorsViewModel
                        {
                            OtherFactor = TransferBenefits.OtherFactor.FinanceAndDebtConcerns,
                            Checked = true,
                            Description = "Finance concerns"
                        },
                        new BenefitsViewModel.OtherFactorsViewModel
                        {
                            OtherFactor = TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues,
                            Checked = true,
                            Description = "Complex issues"
                        },
                    };

                    await _subject.OtherFactorsPost("0001",  otherFactors);

                    _projectsRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => assertOtherFactorsEqual(project)))
                    );
                }

                [Fact]
                public async void GivenUrnAndOtherFactors_RedirectsToTheSummaryPage()
                {
                    var otherFactors = 
                        Enum.GetValues(typeof(TransferBenefits.OtherFactor))
                            .Cast<TransferBenefits.OtherFactor>()
                            .Where(otherFactor => otherFactor != TransferBenefits.OtherFactor.Empty)
                            .Select(otherFactor => new BenefitsViewModel.OtherFactorsViewModel
                            {
                                OtherFactor = otherFactor, Checked = true, Description = "test description"
                            }).ToList();
                    
                    var response = await _subject.OtherFactorsPost("0001", otherFactors);
                    ControllerTestHelpers.AssertResultRedirectsToAction(response, "Index");
                }

                [Theory]
                [InlineData("HighProfile")]
                [InlineData("ComplexLandAndBuildingIssues")]
                [InlineData("FinanceAndDebtConcerns")]
                public async void GivenUrnAndOtherFactor_UpdatesTheProjectCorrectly(string otherFactorString)
                {
                    var otherFactors = new List<BenefitsViewModel.OtherFactorsViewModel>
                    {
                        new BenefitsViewModel.OtherFactorsViewModel
                        {
                            OtherFactor = (TransferBenefits.OtherFactor)Enum.Parse(typeof(TransferBenefits.OtherFactor), otherFactorString),
                            Checked = true,
                            Description = "test Description"
                        }
                    };
                    
                    await _subject.OtherFactorsPost("0001", otherFactors);
                    _projectsRepository.Verify(r => r.Update(It.Is<Project>(
                        project => project.Benefits.OtherFactors.Keys.Count == 1 &&
                                   project.Benefits.OtherFactors[otherFactors[0].OtherFactor] ==
                                   "test Description"
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

                    var otherFactors = new List<BenefitsViewModel.OtherFactorsViewModel>();

                    var response = await controller.OtherFactorsPost("projectUrn", otherFactors);
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

                    var otherFactors = new List<BenefitsViewModel.OtherFactorsViewModel>();

                    var response = await controller.OtherFactorsPost("projectUrn", otherFactors);
                    var viewResult = Assert.IsType<ViewResult>(response);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<string>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Project not found", viewModel);
                }
                
                [Theory]
                [InlineData("HighProfile")]
                [InlineData("ComplexLandAndBuildingIssues")]
                [InlineData("FinanceAndDebtConcerns")]
                public async void GivenUrnAndOtherFactorsWithNoDescription_CreateErrorOnTheView(string otherFactorString)
                {
                    var otherFactors = new List<BenefitsViewModel.OtherFactorsViewModel>
                    {
                        new BenefitsViewModel.OtherFactorsViewModel
                        {
                            OtherFactor = (TransferBenefits.OtherFactor)Enum.Parse(typeof(TransferBenefits.OtherFactor), otherFactorString),
                            Checked = true,
                            Description = ""
                        }
                    };

                    var response = await _subject.OtherFactorsPost("0001", otherFactors);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<BenefitsViewModel>(response);
                    Assert.True(viewModel.FormErrors.HasErrors);
                    Assert.True(viewModel.FormErrors.HasErrorForField(otherFactorString));
                }
            }
        }
    }
}