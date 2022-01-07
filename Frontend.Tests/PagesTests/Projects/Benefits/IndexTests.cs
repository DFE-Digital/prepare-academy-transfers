using Frontend.Models.Benefits;
using Frontend.Pages.Projects.Benefits;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.Benefits
{
    public class IndexTests : PageTests
    {
        private readonly Index _subject;
        public IndexTests()
        {
            _subject = new Index(ProjectRepository.Object)
            {
                Urn = ProjectUrn0001
            };
        }
        
        [Fact]
        public async void GivenUrn_FetchesProjectFromTheRepository()
        {
            await _subject.OnGetAsync();
            
            ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
        }
       
        [Fact]
        public async void GivenUrn_AssignsModelToThePage()
        {
            var response = await _subject.OnGetAsync();
            
            Assert.IsType<PageResult>(response);
            Assert.Equal(ProjectUrn0001,_subject.Urn);
            Assert.Equal(ProjectUrn0001,_subject.BenefitsSummaryViewModel.Urn);
        }
        
        [Fact]
        public async void GivenGetByUrnReturnsError_DisplayErrorPage()
        {
            _subject.Urn = ProjectErrorUrn;
            var response = await _subject.OnGetAsync();
            
            var viewResult = Assert.IsType<ViewResult>(response);
            Assert.Equal(ErrorPageName, viewResult.ViewName);
            Assert.Equal(ErrorMessage, viewResult.Model);
         }
    }
}