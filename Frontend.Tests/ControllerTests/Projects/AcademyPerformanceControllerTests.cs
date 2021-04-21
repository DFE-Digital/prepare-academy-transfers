using System;
using API.Models.Upstream.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Frontend.Controllers.Projects;
using Frontend.Models.AcademyPerformance;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests.Projects
{
    public class AcademyPerformanceControllerTests
    {
        private readonly AcademyPerformanceController _subject;
        private readonly Mock<IProjectsRepository> _projectRepository;

        public AcademyPerformanceControllerTests()
        {
            _projectRepository = new Mock<IProjectsRepository>();
            _subject = new AcademyPerformanceController(_projectRepository.Object);
        }
        public class IndexTests : AcademyPerformanceControllerTests
        {
            [Fact]
            public async void GivenExistingProject_AssignsTheProjectToTheViewModel()
            {
                var projectId = Guid.NewGuid();
                var foundProject = new GetProjectsResponseModel()
                {
                    ProjectId = projectId
                };
                
                _projectRepository.Setup(r => r.GetProjectById(projectId)).ReturnsAsync(new RepositoryResult<GetProjectsResponseModel>()
                {
                    Result = foundProject
                });

                var response = await _subject.Index(projectId);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<IndexViewModel>(viewResponse.Model);
                
                Assert.Equal(foundProject, viewModel.Project);
            }
        }
    }
}