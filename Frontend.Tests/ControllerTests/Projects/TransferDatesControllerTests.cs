using System.Linq;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Frontend.Tests.Helpers;
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
                Urn = "0001"
            };

            _projectsRepository = new Mock<IProjects>();

            _projectsRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResult<Project> { Result = _foundProject });
            _projectsRepository.Setup(s => s.GetByUrn(ErrorWithGetByUrn))
                .ReturnsAsync(
                    new RepositoryResult<Project>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            ErrorMessage = "Error"
                        }
                    });

            _projectsRepository.Setup(r => r.Update(It.IsAny<Project>()))
                .ReturnsAsync(new RepositoryResult<Project>());

            _subject = new TransferDatesController(_projectsRepository.Object);
        }

        public class IndexTests : TransferDatesControllerTests
        {
            [Fact]
            public async void GivenUrn_AssignsModelToTheView()
            {
                var result = await _subject.Index("0001");
                var viewModel = ControllerTestHelpers.AssertViewModelFromResult<TransferDatesViewModel>(result);

                Assert.Equal(_foundProject.Urn, viewModel.Project.Urn);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var response = await _subject.Index(ErrorWithGetByUrn);
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }
        }

        public class FirstDiscussedTests : TransferDatesControllerTests
        {
            public class GetTests : FirstDiscussedTests
            {
                [Fact]
                public async void GivenUrn_AssignsModelToTheView()
                {
                    var result = await _subject.FirstDiscussed("0001");
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<FirstDiscussedViewModel>(result);

                    Assert.Equal(_foundProject.Urn, viewModel.Urn);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.FirstDiscussed(ErrorWithGetByUrn);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }

                [Fact]
                public async void GivenReturnToPreview_AssignsItToTheViewModel()
                {
                    var response = await _subject.FirstDiscussed("0001", true);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<FirstDiscussedViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }
            }
            
            public class PostTests : FirstDiscussedTests
            {
                [Theory]
                [InlineData("01", "01", "2020", "01/01/2020")]
                [InlineData("20", "02", "2021", "20/02/2021")]
                [InlineData("2", "2", "2021", "02/02/2021")]
                public async void GivenUrnAndFullDate_UpdatesTheProjectWithTheCorrectDate(string day, string month,
                    string year, string expectedDate)
                {
                    var vm = new FirstDiscussedViewModel
                    {
                        Urn = "0001",
                        FirstDiscussed = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = day,
                                Month = month,
                                Year = year
                            }
                        }
                    };
                    
                    await _subject.FirstDiscussedPost(vm);
            
                    _projectsRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Dates.FirstDiscussed == expectedDate)));
                }
            
                [Fact]
                public async void GivenUrnAndFullDate_RedirectsToTheSummaryPage()
                {
                    var vm = new FirstDiscussedViewModel
                    {
                        Urn = "0001",
                        FirstDiscussed = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = "01",
                                Month = "02",
                                Year = "2020"
                            }
                        }
                    };
                    
                    var response = await _subject.FirstDiscussedPost(vm);
                    ControllerTestHelpers.AssertResultRedirectsToAction(response, "Index");
                }
            
                [Fact]
                public async void GivenDateError_SetErrorOnTheModel()
                {
                    var vm = new FirstDiscussedViewModel
                    {
                        Urn = "0001",
                        FirstDiscussed = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = "1",
                                Month = "1",
                                Year = "2020"
                            }
                        }
                    };
                    _subject.ModelState.AddModelError(nameof(vm.FirstDiscussed), "error");

                    var response = await _subject.FirstDiscussedPost(vm);
                    var responseModel = ControllerTestHelpers.AssertViewModelFromResult<FirstDiscussedViewModel>(response);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var vm = new FirstDiscussedViewModel
                    {
                        Urn = ErrorWithGetByUrn,
                        FirstDiscussed = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = "1",
                                Month = "1",
                                Year = "2020"
                            }
                        }
                    };
                    var response = await _subject.FirstDiscussedPost(vm);
                    var viewResult = Assert.IsType<ViewResult>(response);
            
                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }
            
                [Fact]
                public async void GivenUpdatenReturnsError_DisplayErrorPage()
                {
                    var vm = new FirstDiscussedViewModel
                    {
                        Urn = "0001",
                        FirstDiscussed = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = "1",
                                Month = "1",
                                Year = "2020"
                            }
                        }
                    };
                    _projectsRepository.Setup(s => s.Update(It.IsAny<Project>())).ReturnsAsync(
                        new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                ErrorMessage = "Update error"
                            }
                        });
            
                    var response = await _subject.FirstDiscussedPost(vm);
                    var viewResult = Assert.IsType<ViewResult>(response);
            
                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Update error", viewResult.Model);
                }
            
                [Fact]
                public async void GivenModelErrorAndReturnToPreview_AssignItToTheViewModel()
                {
                    var vm = new FirstDiscussedViewModel
                    {
                        Urn = "0001",
                        FirstDiscussed = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = "01",
                                Month = "01",
                                Year = "2000"
                            }
                        },
                        ReturnToPreview = true
                    };
                    _subject.ModelState.AddModelError(nameof(vm.FirstDiscussed), "error");
                    var response = await _subject.FirstDiscussedPost(vm);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<FirstDiscussedViewModel>(response);
            
                    Assert.True(viewModel.ReturnToPreview);
                }
            
                [Fact]
                public async void GivenReturnToPreview_RedirectToPreviewPage()
                {
                    var vm = new FirstDiscussedViewModel
                    {
                        Urn = "0001",
                        FirstDiscussed = new DateViewModel
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
                    var response = await _subject.FirstDiscussedPost(vm);
                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response,
                        Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new { id = "0001" })
                    );
                }
            }
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
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.TargetDate(ErrorWithGetByUrn);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
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
                [Theory]
                [InlineData("01", "01", "2020", "01/01/2020")]
                [InlineData("20", "02", "2021", "20/02/2021")]
                [InlineData("2", "2", "2021", "02/02/2021")]
                public async void GivenUrnAndFullDate_UpdatesTheProjectWithTheCorrectDate(string day, string month,
                    string year, string expectedDate)
                {
                    var vm = new TargetDateViewModel
                    {
                        Urn = "0001",
                        TargetDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = day,
                                Month = month,
                                Year = year
                            }
                        }
                    };
                    
                    await _subject.TargetDatePost(vm);

                    _projectsRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Dates.Target == expectedDate)));
                }

                [Fact]
                public async void GivenUrnAndFullDate_RedirectsToTheSummaryPage()
                {
                    var vm = new TargetDateViewModel
                    {
                        Urn = "0001",
                        TargetDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = "01",
                                Month = "01",
                                Year = "2020"
                            }
                        }
                    };
                    
                    var response = await _subject.TargetDatePost(vm);
                    ControllerTestHelpers.AssertResultRedirectsToAction(response, "Index");
                }
                
                [Fact]
                public async void GivenTargetTransferDateBeforeHtbDate_SetErrorOnTheModel()
                {
                    _foundProject.Dates.Htb = "12/10/2020";

                    var vm = new TargetDateViewModel
                    {
                        Urn = "0001",
                        TargetDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = "11",
                                Month = "01",
                                Year = "2020"
                            }
                        }
                    };
                    
                    var response = await _subject.TargetDatePost(vm);
                    var responseModel = ControllerTestHelpers.AssertViewModelFromResult<TargetDateViewModel>(response);

                    Assert.False(_subject.ModelState.IsValid);
                    Assert.Equal(1, _subject.ModelState.ErrorCount);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var vm = new TargetDateViewModel
                    {
                        Urn = ErrorWithGetByUrn,
                        TargetDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = "11",
                                Month = "01",
                                Year = "2020"
                            }
                        }
                    };
                    
                    var response = await _subject.TargetDatePost(vm);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }
                
                [Fact]
                public async void GivenInvalidInputAndReturnToPreview_AssignItToTheViewModel()
                {
                    var vm = new TargetDateViewModel
                    {
                        Urn = "0001",
                        TargetDate = new DateViewModel
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
                    _subject.ModelState.AddModelError(nameof(vm.TargetDate), "error");
                    var response = await _subject.TargetDatePost(vm);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<TargetDateViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectToPreviewPage()
                {
                    var vm = new TargetDateViewModel
                    {
                        Urn = "0001",
                        TargetDate = new DateViewModel
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
                                    Htb = "01/01/2022"
                                }
                            }
                        });
                    
                    var vm = new TargetDateViewModel
                    {
                        Urn = "0002",
                        TargetDate = new DateViewModel
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
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.HtbDate(ErrorWithGetByUrn);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
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
                [Theory]
                [InlineData("01", "01", "2020", "01/01/2020")]
                [InlineData("20", "02", "2021", "20/02/2021")]
                [InlineData("2", "2", "2021", "02/02/2021")]
                public async void GivenUrnAndFullDate_UpdatesTheProjectWithTheCorrectDate(string day, string month,
                    string year, string expectedDate)
                {
                    var vm = new HtbDateViewModel
                    {
                        Urn = "0001",
                        HtbDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = day,
                                Month = month,
                                Year = year
                            }
                        }
                    };
                    
                    await _subject.HtbDatePost(vm);

                    _projectsRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Dates.Htb == expectedDate)));
                }

                [Fact]
                public async void GivenUrnAndFullDate_RedirectsToTheSummaryPage()
                {
                    var vm = new HtbDateViewModel
                    {
                        Urn = "0001",
                        HtbDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = "01",
                                Month = "01",
                                Year = "2020"
                            }
                        }
                    };
                    var response = await _subject.HtbDatePost(vm);
                    ControllerTestHelpers.AssertResultRedirectsToAction(response, "Index");
                }

                [Fact]
                public async void GivenABDateBeforeTransferDate_SetErrorOnTheModel()
                {
                    _foundProject.Dates.Target = "12/10/2020";

                    var vm = new HtbDateViewModel
                    {
                        Urn = "0001",
                        HtbDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = "13",
                                Month = "10",
                                Year = "2020"
                            }
                        }
                    };
                    
                    var response = await _subject.HtbDatePost(vm);
                    var responseModel = ControllerTestHelpers.AssertViewModelFromResult<HtbDateViewModel>(response);

                    Assert.False(_subject.ModelState.IsValid);
                    Assert.Equal(1, _subject.ModelState.ErrorCount);
                    Assert.Equal("The Advisory Board date must be on or before the target date for the transfer", 
                        _subject.ModelState["HtbDate.Date.Day"].Errors.First().ErrorMessage);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var vm = new HtbDateViewModel
                    {
                        Urn = ErrorWithGetByUrn,
                        HtbDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = "13",
                                Month = "10",
                                Year = "2020"
                            }
                        }
                    };
                    
                    var response = await _subject.HtbDatePost(vm);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }

                [Fact]
                public async void GivenUpdatenReturnsError_DisplayErrorPage()
                {
                    _projectsRepository.Setup(s => s.Update(It.IsAny<Project>())).ReturnsAsync(
                        new RepositoryResult<Project>
                        {
                            Error = new RepositoryResultBase.RepositoryError
                            {
                                ErrorMessage = "Update error"
                            }
                        });

                    var vm = new HtbDateViewModel
                    {
                        Urn = "0001",
                        HtbDate = new DateViewModel
                        {
                            Date = new DateInputViewModel
                            {
                                Day = "01",
                                Month = "01",
                                Year = "2000"
                            }
                        }
                    };
                    
                    var response = await _subject.HtbDatePost(vm);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Update error", viewResult.Model);
                }

                [Fact]
                public async void GivenInvalidInputAndReturnToPreview_AssignItToTheViewModel()
                {
                    var vm = new HtbDateViewModel
                    {
                        Urn = "0001",
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
                    _subject.ModelState.AddModelError(nameof(vm.HtbDate), "error");
                    var response = await _subject.HtbDatePost(vm);
                    var viewModel = ControllerTestHelpers.AssertViewModelFromResult<HtbDateViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectToPreviewPage()
                {
                    var vm = new HtbDateViewModel
                    {
                        Urn = "0001",
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