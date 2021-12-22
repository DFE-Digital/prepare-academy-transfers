using AutoFixture;
using Data.Models;
using Frontend.Pages.Projects.GeneralInformation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.GeneralInformation
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
        public async void GivenExistingProject_AssignsTheProjectToTheViewModel()
        {
            var fixture = new Fixture();
            var outgoingAcademy = fixture.Create<Academy>();
            FoundInformationForProject.OutgoingAcademy = outgoingAcademy;
                
            var response = await _subject.OnGetAsync(ProjectUrn0001);

            var expectedGeneralInformation = outgoingAcademy.GeneralInformation;
            Assert.IsType<PageResult>(response);
            Assert.Equal(ProjectUrn0001, _subject.Urn);
            Assert.Equal(expectedGeneralInformation.SchoolPhase, _subject.SchoolPhase);
        }
        
        [Fact]
        public async void GivenGetByUrnReturnsError_DisplayErrorPage()
        {
            var response =  await _subject.OnGetAsync(ProjectErrorUrn);
            var viewResult = Assert.IsType<ViewResult>(response);
        
            Assert.Equal("ErrorPage", viewResult.ViewName);
            Assert.Equal("Error", viewResult.Model);
        }
    }
}