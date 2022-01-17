using Frontend.Pages.Projects.AcademyAndTrustInformation;
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
    }
}