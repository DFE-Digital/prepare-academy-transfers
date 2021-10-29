using Data;
using Data.Models;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class TransferDatesControllerTests
    {
        private const string _errorWithGetByUrn = "errorUrn";
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
                .ReturnsAsync(new RepositoryResult<Project> {Result = _foundProject});
            _projectsRepository.Setup(s => s.GetByUrn(_errorWithGetByUrn))
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
                var viewModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(result);

                Assert.Equal(_foundProject.Urn, viewModel.Project.Urn);
            }

            [Fact]
            public async void GivenGetByUrnReturnsError_DisplayErrorPage()
            {
                var response = await _subject.Index(_errorWithGetByUrn);
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
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(result);

                    Assert.Equal(_foundProject.Urn, viewModel.Project.Urn);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.FirstDiscussed(_errorWithGetByUrn);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }

                [Fact]
                public async void GivenReturnToPreview_AssignsItToTheViewModel()
                {
                    var response = await _subject.FirstDiscussed("0001", true);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

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
                    await _subject.FirstDiscussedPost("0001", day, month, year);

                    _projectsRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Dates.FirstDiscussed == expectedDate)));
                }

                [Fact]
                public async void GivenUrnAndFullDate_RedirectsToTheSummaryPage()
                {
                    var response = await _subject.FirstDiscussedPost("0001", "01", "01", "2020");
                    ControllerTestHelpers.AssertResultRedirectsToAction(response, "Index");
                }

                [Theory]
                [InlineData(null, "1", "2020")]
                [InlineData("1", null, "2020")]
                [InlineData("1", "1", null)]
                public async void GivenPartsOfDateMissing_SetErrorOnTheModel(string day, string month, string year)
                {
                    var response = await _subject.FirstDiscussedPost("0001", day, month, year);
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("Enter a valid date",
                        responseModel.FormErrors.Errors[0].ErrorMessage);
                }

                [Theory]
                [InlineData("0", "1", "2020")]
                [InlineData("32", "1", "2020")]
                [InlineData("1", "0", "2020")]
                [InlineData("1", "13", "2020")]
                [InlineData("1", "13", "0")]
                public async void GivenInvalidDate_SetErrorOnTheModel(string day, string month, string year)
                {
                    var response = await _subject.FirstDiscussedPost("0001", day, month, year);
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("Enter a valid date", responseModel.FormErrors.Errors[0].ErrorMessage);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.FirstDiscussedPost(_errorWithGetByUrn, "01", "01", "2020");
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

                    var response = await _subject.FirstDiscussedPost("0001", "01", "01", "2020");
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Update error", viewResult.Model);
                }

                [Fact]
                public async void GivenInvalidInputAndReturnToPreview_AssignItToTheViewModel()
                {
                    var response = await _subject.FirstDiscussedPost("0001", "01", null, null, returnToPreview:true);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectToPreviewPage()
                {
                    var response = await _subject.FirstDiscussedPost("0001", "01", "01", "2020", returnToPreview:true);
                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response,
                        Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new {id = "0001"})
                    );
                }

                [Fact]
                public async void GivenNoDateAndUnknownIsFalse_SetsErrorOnViewModel()
                {
                    var response = await _subject.FirstDiscussedPost("0001", null, null, null, dateUnknown:false);
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("You must enter the date or confirm that you don't know it", responseModel.FormErrors.Errors[0].ErrorMessage);
                }

                [Fact]
                public async void GivenValidDateAndUnknownIsTrue_SetsErrorOnViewModel()
                {
                    var response = await _subject.FirstDiscussedPost("0001", "25", "10", "2021", dateUnknown: true);
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("You must either enter the date or select 'I do not know this'", responseModel.FormErrors.Errors[0].ErrorMessage);
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
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(result);

                    Assert.Equal(_foundProject.Urn, viewModel.Project.Urn);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.TargetDate(_errorWithGetByUrn);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }

                [Fact]
                public async void GivenReturnToPreview_AssignsItToTheViewModel()
                {
                    var response = await _subject.TargetDate("0001", true);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

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
                    await _subject.TargetDatePost("0001", day, month, year);

                    _projectsRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Dates.Target == expectedDate)));
                }

                [Fact]
                public async void GivenUrnAndFullDate_RedirectsToTheSummaryPage()
                {
                    var response = await _subject.TargetDatePost("0001", "01", "01", "2020");
                    ControllerTestHelpers.AssertResultRedirectsToAction(response, "Index");
                }

                [Theory]
                [InlineData(null, "1", "2020")]
                [InlineData("1", null, "2020")]
                [InlineData("1", "1", null)]
                public async void GivenPartsOfDateMissing_SetErrorOnTheModel(string day, string month, string year)
                {
                    var response = await _subject.TargetDatePost("0001", day, month, year);
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("Enter a valid date",
                        responseModel.FormErrors.Errors[0].ErrorMessage);
                }

                [Theory]
                [InlineData("0", "1", "2020")]
                [InlineData("32", "1", "2020")]
                [InlineData("1", "0", "2020")]
                [InlineData("1", "13", "2020")]
                [InlineData("1", "13", "0")]
                public async void GivenInvalidDate_SetErrorOnTheModel(string day, string month, string year)
                {
                    var response = await _subject.TargetDatePost("0001", day, month, year);
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("Enter a valid date", responseModel.FormErrors.Errors[0].ErrorMessage);
                }
                
                [Fact]
                public async void GivenTargetTransferDateBeforeHtbDate_SetErrorOnTheModel()
                {
                    _foundProject.Dates.Htb = "12/10/2020";
                    
                    var response = await _subject.TargetDatePost("0001", "11", "10", "2020");
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("The target transfer date must be on or after the Advisory Board date", responseModel.FormErrors.Errors[0].ErrorMessage);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.TargetDatePost(_errorWithGetByUrn, "01", "01", "2020");
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

                    var response = await _subject.TargetDatePost("0001", "01", "01", "2020");
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Update error", viewResult.Model);
                }

                [Fact]
                public async void GivenInvalidInputAndReturnToPreview_AssignItToTheViewModel()
                {
                    var response = await _subject.TargetDatePost("0001", "45", null, "2020", true);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectToPreviewPage()
                {
                    var response = await _subject.TargetDatePost("0001", "01", "01", "2020", true);
                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response,
                        Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new {id = "0001"})
                    );
                }
                
                [Fact]
                public async void GivenNoDateAndUnknownIsFalse_SetsErrorOnViewModel()
                {
                    var response = await _subject.TargetDatePost("0001", null, null, null, dateUnknown:false);
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("You must enter the date or confirm that you don't know it", responseModel.FormErrors.Errors[0].ErrorMessage);
                }

                [Fact]
                public async void GivenValidDateAndUnknownIsTrue_SetsErrorOnViewModel()
                {
                    var response = await _subject.TargetDatePost("0001", "25", "10", "2021", dateUnknown: true);
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("You must either enter the date or select 'I do not know this'", responseModel.FormErrors.Errors[0].ErrorMessage);
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
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(result);

                    Assert.Equal(_foundProject.Urn, viewModel.Project.Urn);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.HtbDate(_errorWithGetByUrn);
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Error", viewResult.Model);
                }

                [Fact]
                public async void GivenReturnToPreview_AssignsItToTheViewModel()
                {
                    var response = await _subject.HtbDate("0001", true);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

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
                    await _subject.HtbDatePost("0001", day, month, year);

                    _projectsRepository.Verify(r =>
                        r.Update(It.Is<Project>(project => project.Dates.Htb == expectedDate)));
                }

                [Fact]
                public async void GivenUrnAndFullDate_RedirectsToTheSummaryPage()
                {
                    var response = await _subject.HtbDatePost("0001", "01", "01", "2020");
                    ControllerTestHelpers.AssertResultRedirectsToAction(response, "Index");
                }

                [Theory]
                [InlineData(null, "1", "2020")]
                [InlineData("1", null, "2020")]
                [InlineData("1", "1", null)]
                public async void GivenPartsOfDateMissing_SetErrorOnTheModel(string day, string month, string year)
                {
                    var response = await _subject.HtbDatePost("0001", day, month, year);
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("Enter a valid date", responseModel.FormErrors.Errors[0].ErrorMessage);
                }

                [Theory]
                [InlineData("0", "1", "2020")]
                [InlineData("32", "1", "2020")]
                [InlineData("1", "0", "2020")]
                [InlineData("1", "13", "2020")]
                [InlineData("1", "13", "0")]
                public async void GivenInvalidDate_SetErrorOnTheModel(string day, string month, string year)
                {
                    var response = await _subject.HtbDatePost("0001", day, month, year);
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("Enter a valid date", responseModel.FormErrors.Errors[0].ErrorMessage);
                }
                
                [Fact]
                public async void GivenABDateBeforeTransferDate_SetErrorOnTheModel()
                {
                    _foundProject.Dates.Target = "12/10/2020";
                    
                    var response = await _subject.HtbDatePost("0001", "13", "10", "2020");
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("The Advisory Board date must be on or before the target date for the transfer", responseModel.FormErrors.Errors[0].ErrorMessage);
                }

                [Fact]
                public async void GivenGetByUrnReturnsError_DisplayErrorPage()
                {
                    var response = await _subject.HtbDatePost(_errorWithGetByUrn, "01", "01", "2020");
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

                    var response = await _subject.HtbDatePost("0001", "01", "01", "2020");
                    var viewResult = Assert.IsType<ViewResult>(response);

                    Assert.Equal("ErrorPage", viewResult.ViewName);
                    Assert.Equal("Update error", viewResult.Model);
                }

                [Fact]
                public async void GivenInvalidInputAndReturnToPreview_AssignItToTheViewModel()
                {
                    var response = await _subject.HtbDatePost("0001", null, "01", null, true);
                    var viewModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(viewModel.ReturnToPreview);
                }

                [Fact]
                public async void GivenReturnToPreview_RedirectToPreviewPage()
                {
                    var response = await _subject.HtbDatePost("0001", "01", "01", "2020", true);
                    ControllerTestHelpers.AssertResultRedirectsToPage(
                        response,
                        Links.HeadteacherBoard.Preview.PageName,
                        new RouteValueDictionary(new {id = "0001"})
                    );
                }
                
                [Fact]
                public async void GivenNoDateAndUnknownIsFalse_SetsErrorOnViewModel()
                {
                    var response = await _subject.HtbDatePost("0001", null, null, null, dateUnknown:false);
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("You must enter the date or confirm that you don't know it", responseModel.FormErrors.Errors[0].ErrorMessage);
                }

                [Fact]
                public async void GivenValidDateAndUnknownIsTrue_SetsErrorOnViewModel()
                {
                    var response = await _subject.HtbDatePost("0001", "25", "10", "2021", dateUnknown: true);
                    var responseModel = ControllerTestHelpers.GetViewModelFromResult<TransferDatesViewModel>(response);

                    Assert.True(responseModel.FormErrors.HasErrors);
                    Assert.Equal("You must either enter the date or select 'I do not know this'", responseModel.FormErrors.Errors[0].ErrorMessage);
                }
            }
        }
    }
}