using System.Collections.Generic;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Frontend.Pages.Home;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Home
{
    public class IndexTests : PageTests
    {
        private readonly Mock<ILogger<Index>> _logger = new Mock<ILogger<Index>>();
        private readonly Index _subject;

        public IndexTests()
        {
            _subject = new Index(ProjectRepository.Object, _logger.Object);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void GivenPage_GetsProjectForThatPage(int page)
        {
            var foundProjects = new List<ProjectSearchResult>() { new ProjectSearchResult() { Urn = "1" } };

            ProjectRepository.Setup(r => r.GetProjects(page))
                .ReturnsAsync(new RepositoryResult<List<ProjectSearchResult>> { Result = foundProjects });

            _subject.CurrentPage = page;
            await _subject.OnGetAsync();

            ProjectRepository.Verify(r => r.GetProjects(page), Times.Once());
            Assert.Equal(page, _subject.CurrentPage);
            Assert.Equal(foundProjects, _subject.Projects);
        }

        [Fact]
        public async void GivenGetProjectsReturnsError_DisplayErrorPage()
        {
            ProjectRepository.Setup(r => r.GetProjects(It.IsAny<int>())).ReturnsAsync(
                new RepositoryResult<List<ProjectSearchResult>>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        ErrorMessage = ErrorMessage
                    }
                });
        
            var response = await _subject.OnGetAsync();
            var viewResult = Assert.IsType<ViewResult>(response);
        
            Assert.Equal(ErrorPageName, viewResult.ViewName);
            Assert.Equal(ErrorMessage, viewResult.Model);
        }
    }
}