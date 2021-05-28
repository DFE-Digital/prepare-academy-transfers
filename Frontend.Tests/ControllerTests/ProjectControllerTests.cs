using Data;
using Data.Models;
using Frontend.Controllers;
using Frontend.Models;
using Frontend.Tests.Helpers;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests
{
    public class ProjectControllerTests
    {
        private readonly ProjectController _subject;
        private readonly Mock<IProjects> _projectsRepository;

        public ProjectControllerTests()
        {
            _projectsRepository = new Mock<IProjects>();
            _subject = new ProjectController(_projectsRepository.Object);
        }

        public class IndexTests : ProjectControllerTests
        {
            [Fact]
            public async void GivenAProjectID_PutsTheProjectNameAndTrustNameInTheViewData()
            {
                const string projectId = "1234";

                _projectsRepository.Setup(r => r.GetByUrn(projectId)).ReturnsAsync(
                    new RepositoryResult<Project>
                    {
                        Result = new Project
                        {
                            Name = "Some name",
                            OutgoingTrustName = "Meow Meowington's Trust"
                        }
                    });

                var actionResult = await _subject.Index(projectId);
                var viewModel = ControllerTestHelpers.GetViewModelFromResult<ProjectTaskListViewModel>(actionResult);

                Assert.Equal("Some name", viewModel.Project.Name);
                Assert.Equal("Meow Meowington's Trust", viewModel.Project.OutgoingTrustName);
            }
        }
    }
}