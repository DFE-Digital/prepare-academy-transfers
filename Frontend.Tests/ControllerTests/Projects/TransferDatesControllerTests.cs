using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Frontend.Tests.Helpers;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class TransferDatesControllerTests
    {
        private const string ErrorWithGetByUrn = "errorUrn";
        private readonly TransferDatesController _subject;
        private readonly Mock<IProjects> _projectsRepository;
        private readonly Project _foundProject;

        public TransferDatesControllerTests()
        {
            _foundProject = new Project()
            {
                Urn = "0001",
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies
                    {
                        OutgoingAcademyUrn = "0002"
                    }
                }
            };

            _projectsRepository = new Mock<IProjects>();

            _projectsRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResult<Project> { Result = _foundProject });

            _projectsRepository.Setup(r => r.Update(It.IsAny<Project>()))
                .ReturnsAsync(new RepositoryResult<Project>());

            _subject = new TransferDatesController(_projectsRepository.Object);
        }

        public class TargetDateTests : TransferDatesControllerTests
        {
            public class GetTests : TargetDateTests
            {
                [Fact]
                public async void GivenUrn_AssignsModelToTheView()
                {
                    var result = await _subject.TargetDate("0001");
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<TargetDateViewModel>(result);

                    Assert.Equal(_foundProject.Urn, viewModel.Urn);
                }

                [Fact]
                public async void GivenReturnToPreview_AssignsItToTheViewModel()
                {
                    var response = await _subject.TargetDate("0001", true);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<TargetDateViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }
            }

            public class PostTests : TargetDateTests
            {
                [Fact]
                public async void GivenUrnAndFullDate_UpdatesTheProjectWithTheCorrectDate()
                {
                    var targetDate = DateTime.Today;
                    var vm = new TargetDateViewModel
                    {
                        Urn = "0001",
                        TargetDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = targetDate.Day.ToString(),
                                Month =  targetDate.Month.ToString(),
                                Year =  targetDate.Year.ToString(),
                            }
                        }
                    };
                    
                    await _subject.TargetDatePost(vm);

                    _projectsRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Dates.Target == targetDate.ToShortDate())));
                }

                [Fact]
                public async void GivenUrnAndFullDate_RedirectsToTheSummaryPage()
                {
                    var targetDate = DateTime.Today;
                    var vm = new TargetDateViewModel
                    {
                        Urn = "0001",
                        TargetDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = targetDate.Day.ToString(),
                                Month = targetDate.Month.ToString(),
                                Year = targetDate.Year.ToString()
                            }
                        }
                    };
                    
                    var response = await _subject.TargetDatePost(vm);
                    ControllerTestHelpers.AssertResultRedirectsToPage(response, "/Projects/TransferDates/Index", 
                        new RouteValueDictionary(new { Urn = "0001" }));
                }
                
                [Fact]
                public async void GivenTargetTransferDateBeforeHtbDate_SetErrorOnTheModel()
                {
                    _foundProject.Dates.Htb = DateTime.Now.AddYears(-1).ToShortDate();

                    var targetDate = DateTime.Now.AddYears(-2);
                    var vm = new TargetDateViewModel
                    {
                        Urn = "0001",
                        TargetDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = targetDate.Day.ToString(),
                                Month = targetDate.Month.ToString(),
                                Year = targetDate.Year.ToString()
                            }
                        }
                    };
                    
                    var response = await _subject.TargetDatePost(vm);
                    var responseModel = ControllerTestHelpers.AssertViewModelFromResult<TargetDateViewModel>(response);

                    Assert.False(_subject.ModelState.IsValid);
                    Assert.Equal(1, _subject.ModelState.ErrorCount);
                }

                [Fact]
                public async void GivenInvalidInputAndReturnToPreview_AssignItToTheViewModel()
                {
                    var targetDate = DateTime.Now.AddYears(-1);
                    var vm = new TargetDateViewModel
                    {
                        Urn = "0001",
                        TargetDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = targetDate.Day.ToString(),
                                Month = targetDate.Month.ToString(),
                                Year = targetDate.Year.ToString()
                            }
                        },
                        ReturnToPreview = true
                    };
                    _subject.ModelState.AddModelError(nameof(vm.TargetDate), "error");
                    var response = await _subject.TargetDatePost(vm);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<TargetDateViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectToPreviewPage()
                {
                    var targetDate = DateTime.Today;
                    var vm = new TargetDateViewModel
                    {
                        Urn = "0001",
                        TargetDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = targetDate.Day.ToString(),
                                Month = targetDate.Month.ToString(),
                                Year = targetDate.Year.ToString(),
                            }
                        },
                        ReturnToPreview = true
                    };
                    var response = await _subject.TargetDatePost(vm);
                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response,
                        Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new { id = "0001" })
                    );
                }
                
                [Fact]
                public async void GivenHtbDateGreaterThanTargetDate_SetsErrorOnViewModel()
                {
                    _projectsRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                        .ReturnsAsync(new RepositoryResult<Project>
                        {
                            Result = new Project
                            {
                                Urn = "0002",
                                Dates = new TransferDates
                                {
                                    Htb = DateTime.Now.AddYears(1).ToShortDate()
                                }
                            }
                        });

                    var targetDate = DateTime.Now.AddYears(-1);
                    var vm = new TargetDateViewModel
                    {
                        Urn = "0002",
                        TargetDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = targetDate.Day.ToString(),
                                Month = targetDate.Month.ToString(),
                                Year = targetDate.Year.ToString(),
                            }
                        },
                        ReturnToPreview = true
                    };
                    Assert.True(await _subject.TargetDatePost(vm) is ViewResult result 
                                && result.ViewData.ModelState["TargetDate.Date.Day"].Errors.Any());
                }
            }
        }

        public class HtbDateTests : TransferDatesControllerTests
        {
            public class GetTests : HtbDateTests
            {
                [Fact]
                public async void GivenUrn_AssignsModelToTheView()
                {
                    var result = await _subject.HtbDate("0001");
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<HtbDateViewModel>(result);

                    Assert.Equal(_foundProject.Urn, viewModel.Urn);
                }

                [Fact]
                public async void GivenReturnToPreview_AssignsItToTheViewModel()
                {
                    var response = await _subject.HtbDate("0001", true);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<HtbDateViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }
            }

            public class PostTests : HtbDateTests
            {
                [Fact]
                public async void GivenUrnAndFullDate_UpdatesTheProjectWithTheCorrectDate()
                {
                    var expectedDate = DateTime.Now.AddYears(1);
                    var vm = new HtbDateViewModel
                    {
                        Urn = "0001",
                        HtbDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = expectedDate.Day.ToString(),
                                Month = expectedDate.Month.ToString(),
                                Year = expectedDate.Year.ToString()
                            }
                        }
                    };
                    
                    await _subject.HtbDatePost(vm);

                    _projectsRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Dates.Htb == expectedDate.ToShortDate())));
                }

                [Fact]
                public async void GivenUrnAndFullDate_RedirectsToTheSummaryPage()
                {
                    var htbDate = DateTime.Now;
                    var vm = new HtbDateViewModel
                    {
                        Urn = "0001",
                        HtbDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = htbDate.Day.ToString(),
                                Month = htbDate.Month.ToString(),
                                Year = htbDate.Year.ToString()
                            }
                        }
                    };
                    var response = await _subject.HtbDatePost(vm);
                    ControllerTestHelpers.AssertResultRedirectsToPage(response, "/Projects/TransferDates/Index", 
                        new RouteValueDictionary(new { Urn = "0001" }));
                }

                [Fact]
                public async void GivenABDateAfterTransferDate_SetErrorOnTheModel()
                {
                    var htbDate = DateTime.Now.AddDays(3);
                    var targetDate = DateTime.Now.AddDays(2);
                    _foundProject.Dates.Target = targetDate.ToShortDate();

                    var vm = new HtbDateViewModel
                    {
                        Urn = "0001",
                        HtbDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = htbDate.Day.ToString(),
                                Month = htbDate.Month.ToString(),
                                Year = htbDate.Year.ToString()
                            }
                        }
                    };
                    
                    var response = await _subject.HtbDatePost(vm);
                    var responseModel = ControllerTestHelpers.AssertViewModelFromResult<HtbDateViewModel>(response);

                    Assert.False(_subject.ModelState.IsValid);
                    Assert.Equal(1, _subject.ModelState.ErrorCount);
                    Assert.Single(_subject.ModelState["HtbDate.Date.Day"].Errors);
                }

                [Fact]
                public async void GivenInvalidInputAndReturnToPreview_AssignItToTheViewModel()
                {
                    var htbDate = DateTime.Today;
                    var vm = new HtbDateViewModel
                    {
                        Urn = "0001",
                        HtbDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = htbDate.Day.ToString(),
                                Month = htbDate.Month.ToString(),
                                Year = htbDate.Year.ToString()
                            }
                        },
                        ReturnToPreview = true
                    };
                    _subject.ModelState.AddModelError(nameof(vm.HtbDate), "error");
                    var response = await _subject.HtbDatePost(vm);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<HtbDateViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectToPreviewPage()
                {
                    var htbDate = DateTime.Today;
                    var vm = new HtbDateViewModel
                    {
                        Urn = "0001",
                        HtbDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = htbDate.Day.ToString(),
                                Month = htbDate.Month.ToString(),
                                Year = htbDate.Year.ToString()
                            }
                        },
                        ReturnToPreview = true
                    };
                    var response = await _subject.HtbDatePost(vm);
                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response,
                        Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new { id = "0001" })
                    );
                }

                [Fact]
                public async void GivenHtbDateGreaterThanTargetDate_SetsErrorOnViewModel()
                {
                    _projectsRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                        .ReturnsAsync(new RepositoryResult<Project>
                        {
                            Result = new Project
                            {
                                Urn = "0002",
                                Dates = new TransferDates
                                {
                                    Target = "01/01/2000"
                                }
                            }
                        });
                    
                    var vm = new HtbDateViewModel
                    {
                        Urn = "0002",
                        HtbDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = "01",
                                Month = "01",
                                Year = "2020"
                            }
                        },
                        ReturnToPreview = true
                    };
                    Assert.True(await _subject.HtbDatePost(vm) is ViewResult result 
                                && result.ViewData.ModelState["HtbDate.Date.Day"].Errors.Any());
                }
            }
        }
    }
}