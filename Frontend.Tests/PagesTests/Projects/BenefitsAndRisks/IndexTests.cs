using Frontend.Pages.Projects.BenefitsAndRisks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.BenefitsAndRisks
{
    public class IndexTests : BaseTests
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
    }
}