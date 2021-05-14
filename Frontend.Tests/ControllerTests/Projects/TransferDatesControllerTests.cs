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
            }
        }
    }
}