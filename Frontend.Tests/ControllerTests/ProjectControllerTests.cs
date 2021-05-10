using System;
using Data;
using Data.Models;
using Frontend.Controllers;
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
                var projectId = Guid.NewGuid();

                _projectsRepository.Setup(r => r.GetByUrn(projectId.ToString())).ReturnsAsync(
                    new RepositoryResult<Project>
                    {
                        Result = new Project
                        {
                            Name = "Some name",
                            OutgoingTrustName = "Meow Meowington's Trust"
                        }
                    });

                await _subject.Index(projectId);

                Assert.Equal("Some name", _subject.ViewData["ProjectName"]);
                Assert.Equal("Meow Meowington's Trust", _subject.ViewData["OutgoingTrustName"]);
            }
        }
    }
}