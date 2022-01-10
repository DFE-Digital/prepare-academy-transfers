using Frontend.Pages.Projects.AcademyAndTrustInformation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.AcademyAndTrustInformation
{
    public class IndexTests : PageTests
    {
        private readonly Index _subject;
        public IndexTests()
        {
            _subject = new Index(GetInformationForProject.Object);
        }
        [Fact]
        public async void GivenUrn_FetchesProjectFromTheRepository()
        {
            await _subject.OnGetAsync(ProjectUrn0001);
        
            GetInformationForProject.Verify(r => r.Execute(ProjectUrn0001), Times.Once);
        }
        
        [Fact]
        public async void GivenGetByUrnReturnsError_DisplayErrorPage()
        {
            var response =  await _subject.OnGetAsync(ProjectErrorUrn);
            var viewResult = Assert.IsType<ViewResult>(response);
        
            Assert.Equal(ErrorPageName, viewResult.ViewName);
            Assert.Equal(ErrorMessage, viewResult.Model);
        }
    }
}