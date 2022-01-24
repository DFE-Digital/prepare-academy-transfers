using System.Collections.Generic;
using AutoFixture;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Pages.Projects.TransferDates;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.TransferDates
{
    public class IndexTests : BaseTests
    {
        private readonly Index _subject;
        private readonly Project _foundPopulatedProjectFromRepo;
        private const string PopulatedProjectUrn = "01234";

        public IndexTests()
        {
            var fixture = new Fixture();
            
            var populatedTransferringAcademy = fixture.Build<TransferringAcademies>()
                .With(a => a.OutgoingAcademyName, OutgoingAcademyName)
                .With(a => a.OutgoingAcademyUrn, AcademyUrn)
                .Create();
            
            _foundPopulatedProjectFromRepo = fixture.Build<Project>()
                .With(p => p.Urn, PopulatedProjectUrn)
                .With(p => p.TransferringAcademies, new List<TransferringAcademies> {populatedTransferringAcademy})
                .Create();
            
            ProjectRepository.Setup(s => s.GetByUrn(PopulatedProjectUrn)).ReturnsAsync(
                new RepositoryResult<Project>
                {
                    Result = _foundPopulatedProjectFromRepo
                });
            
            _subject = new Index(ProjectRepository.Object);
        }

        [Fact]
        public async void GivenUrn_FetchesProjectFromTheRepository()
        {
            _subject.Urn = PopulatedProjectUrn;
            await _subject.OnGetAsync();

            ProjectRepository.Verify(r => r.GetByUrn(PopulatedProjectUrn), Times.Once);
            Assert.Equal(_foundPopulatedProjectFromRepo.Dates.FirstDiscussed, _subject.FirstDiscussedDate);
            Assert.Equal(_foundPopulatedProjectFromRepo.Dates.Htb, _subject.HtbDate);
            Assert.Equal(_foundPopulatedProjectFromRepo.Dates.Target, _subject.TargetDate);
            Assert.Equal(_foundPopulatedProjectFromRepo.Dates.HasFirstDiscussedDate, _subject.HasFirstDiscussedDate);
            Assert.Equal(_foundPopulatedProjectFromRepo.Dates.HasHtbDate, _subject.HasHtbDate);
            Assert.Equal(_foundPopulatedProjectFromRepo.Dates.HasTargetDateForTransfer, _subject.HasTargetDate);
        }
    }
}