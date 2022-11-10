using System.Collections.Generic;
using Data;
using Data.Models;
using Frontend.Pages.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Home
{
    public class IndexTests : BaseTests
    {
        private readonly Mock<ILogger<Index>> _logger = new Mock<ILogger<Index>>();
        private readonly Index _subject;
        private readonly Mock<IUrlHelper> _urlHelper = new Mock<IUrlHelper>();
        
        public IndexTests()
        {
            _subject = new Index(ProjectRepository.Object, _logger.Object);
            var foundProjects = new List<ProjectSearchResult>
            {
                new ProjectSearchResult {Urn = "1"},
                new ProjectSearchResult {Urn = "2"},
                new ProjectSearchResult {Urn = "3"},
                new ProjectSearchResult {Urn = "4"},
                new ProjectSearchResult {Urn = "5"},
                new ProjectSearchResult {Urn = "6"}
            };
            ProjectRepository.Setup(r => r.GetProjects(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new RepositoryResult<List<ProjectSearchResult>> {Result = foundProjects});
            _subject.Url = _urlHelper.Object;
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void GivenPage_GetsProjectForThatPage(int page)
        {
            _subject.CurrentPage = page;
            await _subject.OnGetAsync();

            ProjectRepository.Verify(r => r.GetProjects(page, default, 10), Times.Once());
        }

        [Fact]
        public async void OnceLoggedInRedirectToReturnUrl()
        {
            //Arrange
            _subject.ReturnUrl = "path/to/return/to";
            _urlHelper.Setup(s => s.IsLocalUrl(_subject.ReturnUrl)).Returns(true);
            _subject.Url = _urlHelper.Object;
            
            //Act
            var result = await _subject.OnGetAsync();

            //Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(_subject.ReturnUrl, redirectResult.Url);
        }
    }
}