using Data.Models;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Pages.Projects;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects
{
    public class PupilNumbersTests : BaseTests
    {
        private readonly PupilNumbers _subject;
        private const string AdditionalInformation =  "Pupil numbers additional information";
        
        public PupilNumbersTests()
        {
            _subject = new PupilNumbers(GetInformationForProject.Object, ProjectRepository.Object)
            {
                Urn = ProjectUrn0001,
                AdditionalInformationViewModel = new AdditionalInformationViewModel()
                {
                   AdditionalInformation = AdditionalInformation
                }
            };
        }

        public class GetTests : PupilNumbersTests
        {
            [Fact]
            public async void GivenProjectId_GetsInformationAboutProject()
            {
                await _subject.OnGetAsync();
                GetInformationForProject.Verify(s => s.Execute(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenExistingAdditionalInformation_AssignsTheInformationToTheViewModel()
            {
                FoundInformationForProject.Project.PupilNumbersAdditionalInformation =
                    AdditionalInformation;
                var response = await _subject.OnGetAsync();

                Assert.Equal(AdditionalInformation,
                    _subject.AdditionalInformationViewModel.AdditionalInformation);
            }
        }

        public class PostTests : PupilNumbersTests
        {
            [Fact]
            public async void GivenAdditionalInformationIsPosted_CorrectlyRedirects()
            {
                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(response, "/Projects/PupilNumbers",
                    new RouteValueDictionary(new {Urn = ProjectUrn0001}));
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectCorrectly()
            {
                await _subject.OnPostAsync();

                ProjectRepository.Verify(r => r.Update(It.Is<Project>(
                    project => project.PupilNumbersAdditionalInformation == AdditionalInformation
                )));
            }

            [Fact]
            public async void GivenReturnToPreview_RedirectToPreviewPage()
            {
                _subject.ReturnToPreview = true;
                var response = await _subject.OnPostAsync();
                ControllerTestHelpers.AssertResultRedirectsToPage(
                    response, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new {Urn = ProjectUrn0001})
                );
            }
        }
    }
}