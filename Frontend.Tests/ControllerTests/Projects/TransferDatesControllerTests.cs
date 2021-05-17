using Data;
using Data.Models;
using Frontend.Controllers.Projects;
using Frontend.Models;
using Frontend.Tests.Helpers;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class TransferDatesControllerTests
    {
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
                .ReturnsAsync(new RepositoryResult<Project>() {Result = _foundProject});

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
                        r.Update(It.Is<Project>(project => project.TransferDates.FirstDiscussed == expectedDate)));
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
                    Assert.Equal("Please enter the date the transfer was first discussed",
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
                    Assert.Equal("Please enter a valid date", responseModel.FormErrors.Errors[0].ErrorMessage);
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
                        r.Update(It.Is<Project>(project => project.TransferDates.Target == expectedDate)));
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
                    Assert.Equal("Please enter the target date for the transfer",
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
                    Assert.Equal("Please enter a valid date", responseModel.FormErrors.Errors[0].ErrorMessage);
                }
            }
        }
    }
}