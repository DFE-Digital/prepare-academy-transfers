using System;
using System.Collections.Generic;
using API.Models.Upstream.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Data;
using Frontend.Controllers;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests
{
    public class ProjectControllerTests
    {
        private readonly ProjectController _subject;
        private readonly Mock<IProjectsRepository> _projectsRepository;

        public ProjectControllerTests()
        {
            _projectsRepository = new Mock<IProjectsRepository>();
            _subject = new ProjectController(_projectsRepository.Object);
        }

        public class IndexTests : ProjectControllerTests
        {
            [Fact]
            public async void GivenAProjectID_PutsTheProjectNameAndTrustNameInTheViewData()
            {
                var projectId = Guid.NewGuid();

                _projectsRepository.Setup(r => r.GetProjectById(projectId)).ReturnsAsync(
                    new RepositoryResult<GetProjectsResponseModel>
                    {
                        Result = new GetProjectsResponseModel
                        {
                            ProjectName = "Some name",
                            ProjectTrusts = new List<GetProjectsTrustResponseModel>
                            {
                                new GetProjectsTrustResponseModel
                                {
                                    TrustName = "Meow Meowington's Trust"
                                }
                            }
                        }
                    });

                await _subject.Index(projectId);

                Assert.Equal("Some name", _subject.ViewData["ProjectName"]);
                Assert.Equal("Meow Meowington's Trust", _subject.ViewData["OutgoingTrustName"]);
            }
        }
    }
}