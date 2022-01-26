using Frontend.Pages.Projects.TransferDates;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.TransferDates
{
    public class IndexTests : BaseTests
    {
        private readonly Index _subject;
       
        public IndexTests()
        {
            _subject = new Index(ProjectRepository.Object);
        }

        [Fact]
        public async void GivenUrn_FetchesProjectFromTheRepository()
        {
            _subject.Urn = PopulatedProjectUrn;
            await _subject.OnGetAsync();

            ProjectRepository.Verify(r => r.GetByUrn(PopulatedProjectUrn), Times.Once);
            Assert.Equal(FoundPopulatedProjectFromRepo.Dates.FirstDiscussed, _subject.FirstDiscussedDate);
            Assert.Equal(FoundPopulatedProjectFromRepo.Dates.Htb, _subject.AdvisoryBoardDate);
            Assert.Equal(FoundPopulatedProjectFromRepo.Dates.Target, _subject.TargetDate);
            Assert.Equal(FoundPopulatedProjectFromRepo.Dates.HasFirstDiscussedDate, _subject.HasFirstDiscussedDate);
            Assert.Equal(FoundPopulatedProjectFromRepo.Dates.HasHtbDate, _subject.HasAdvisoryBoardDate);
            Assert.Equal(FoundPopulatedProjectFromRepo.Dates.HasTargetDateForTransfer, _subject.HasTargetDate);
        }
    }
}