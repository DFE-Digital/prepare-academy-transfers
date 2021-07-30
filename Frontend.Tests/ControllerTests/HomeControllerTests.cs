using Data;
using Data.Models;
using Frontend.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Frontend.Tests.Helpers;
using Frontend.Views.Home;
using Xunit;

namespace Frontend.Tests.ControllerTests
{
    public class HomeControllerTests
    {
        private readonly HomeController _subject;
        private readonly Mock<IConfiguration> _configuration = new Mock<IConfiguration>();
        private readonly Mock<IProjects> _projectsRepository = new Mock<IProjects>();
        private readonly Mock<ILogger<HomeController>> _logger = new Mock<ILogger<HomeController>>();

        public HomeControllerTests()
        {
            _subject = new HomeController(_configuration.Object, _projectsRepository.Object, _logger.Object);
        }

        public class IndexTests : HomeControllerTests
        {
            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            public async void GivenPage_GetsProjectForThatPage(int page)
            {
                var foundProjects = new List<ProjectSearchResult>() {new ProjectSearchResult() {Urn = "1"}};

                _projectsRepository.Setup(r => r.GetProjects(page))
                    .ReturnsAsync(new RepositoryResult<List<ProjectSearchResult>> {Result = foundProjects});
                
                var response = await _subject.Index(page);
                var viewModel = ControllerTestHelpers.GetViewModelFromResult<Index>(response);

                _projectsRepository.Verify(r => r.GetProjects(page), Times.Once());
                Assert.Equal(page, viewModel.Page);
                Assert.Equal(foundProjects, viewModel.Projects);
            }


            [Fact]
            public async void GivenGetProjectsReturnsError_DisplayErrorPage()
            {
                _projectsRepository.Setup(r => r.GetProjects(It.IsAny<int>())).ReturnsAsync(
                    new RepositoryResult<List<ProjectSearchResult>>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            ErrorMessage = "Error"
                        }
                    });

                var response = await _subject.Index();
                var viewResult = Assert.IsType<ViewResult>(response);

                Assert.Equal("ErrorPage", viewResult.ViewName);
                Assert.Equal("Error", viewResult.Model);
            }
        }
    }
}