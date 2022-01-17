using System.Collections.Generic;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Frontend.Models;
using Frontend.Pages.TaskList.KeyStage5Performance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.TaskList.HtbDocument
{
    public class KeyStage5PerformanceTests : PageTests
    {
        private readonly KeyStage5Performance _subject;
        public KeyStage5PerformanceTests()
        {
            FoundInformationForProject.EducationPerformance = new EducationPerformance
            {
                KeyStage5Performance = new List<KeyStage5> {new KeyStage5 {Year = "2019"}},

            };

            _subject = new KeyStage5Performance(GetInformationForProject.Object, ProjectRepository.Object);
        }

        public class OnGetAsyncTests : KeyStage5PerformanceTests
        {
            [Theory]
            [InlineData("1234")]
            [InlineData("4321")]
            public async void OnGet_GetInformationForProjectId(string id)
            {
                await _subject.OnGetAsync(id);

                GetInformationForProject.Verify(s => s.Execute(id));
            }

            [Fact]
            public async void OnGet_AssignsCorrectValuesToViewModel()
            {
                var result = await _subject.OnGetAsync("1234");

                Assert.IsType<PageResult>(result);
                Assert.Equal(FoundInformationForProject.Project.Urn, _subject.ProjectUrn);
                Assert.Equal(FoundInformationForProject.EducationPerformance, _subject.EducationPerformance);
                Assert.Equal(FoundInformationForProject.OutgoingAcademy.Urn, _subject.OutgoingAcademyUrn);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheViewModel()
            {
                const string additionalInformation = "some additional info";
                FoundInformationForProject.Project.KeyStage5PerformanceAdditionalInformation = additionalInformation;

                await _subject.OnGetAsync("1234");

                Assert.Equal(additionalInformation, _subject.AdditionalInformation.AdditionalInformation);
                Assert.Equal(FoundInformationForProject.Project.Urn, _subject.AdditionalInformation.Urn);
            }

            [Fact]
            public async void GivenReturnToPreview_UpdatesTheViewModel()
            {
                await _subject.OnGetAsync("123", false, true);

                Assert.True(_subject.ReturnToPreview);
            }
        }

        public class OnPostAsyncTests : KeyStage5PerformanceTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnPostAsync("1234", string.Empty, false);

                ProjectRepository.Verify(r => r.GetByUrn("1234"), Times.Once);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectModel()
            {
                const string additionalInformation = "some additional info";

                var response = await _subject.OnPostAsync("1234", additionalInformation, false);

                var redirectToPageResponse = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal("KeyStage5Performance", redirectToPageResponse.PageName);
                Assert.Equal("OnGetAsync", redirectToPageResponse.PageHandler);
                Assert.Equal(additionalInformation, FoundProjectFromRepo.KeyStage5PerformanceAdditionalInformation);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectCorrectly()
            {
                const string additionalInfo = "test info";

                await _subject.OnPostAsync("1234", additionalInfo, false);
                ProjectRepository.Verify(r => r.Update(It.Is<Project>(
                    project => project.KeyStage5PerformanceAdditionalInformation == additionalInfo
                )));
            }

            [Fact]
            public async void GivenReturnToPreview_RedirectsToThePreviewPage()
            {
                var response = await _subject.OnPostAsync("1234", "", true);

                var redirectResponse = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal(Links.HeadteacherBoard.Preview.PageName, redirectResponse.PageName);
                Assert.Equal("1234", redirectResponse.RouteValues["id"]);
            }
        }
    }
}