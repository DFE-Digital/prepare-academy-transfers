using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Data.Models;
using Frontend.Pages.Projects.GeneralInformation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.GeneralInformation
{
    public class IndexTests : BaseTests
    {
        private readonly Index _subject;
        
        public IndexTests()
        {
            _subject = new Index(GetInformationForProject.Object)
            {
                AcademyUkprn = AcademyUkprn
            };
        }
        
        [Fact]
        public async void GivenUrn_FetchesProjectFromTheRepository()
        {
            await _subject.OnGetAsync(ProjectUrn0001);
        
            GetInformationForProject.Verify(r => r.Execute(ProjectUrn0001), Times.Once);
        }
        
        [Fact]
        public async void GivenExistingAcademy_AssignsTheAcademyToTheViewModel()
        {
            var ukprn = "7689";
            var fixture = new Fixture();
            var outgoingAcademy = fixture.Create<Academy>();
            outgoingAcademy.Ukprn = ukprn;
            outgoingAcademy.LastChangedDate = "22-10-2021";
            FoundInformationForProject.OutgoingAcademies = new List<Academy>
            {
                outgoingAcademy
            };

            _subject.AcademyUkprn = ukprn;
            var response = await _subject.OnGetAsync(ProjectUrn0001);

            var expectedGeneralInformation = FoundInformationForProject.OutgoingAcademies.First().GeneralInformation;
            Assert.IsType<PageResult>(response);
            Assert.Equal(outgoingAcademy.Name, _subject.AcademyName);
            Assert.Equal(expectedGeneralInformation.SchoolPhase, _subject.SchoolPhase);
        }
    }
}