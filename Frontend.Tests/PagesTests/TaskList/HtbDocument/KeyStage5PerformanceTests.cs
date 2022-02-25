using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Pages.TaskList.KeyStage5Performance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.TaskList.HtbDocument
{
    public class KeyStage5PerformanceTests : BaseTests
    {
        private readonly KeyStage5Performance _subject;

        public KeyStage5PerformanceTests()
        {
            FoundInformationForProject.OutgoingAcademies.First().EducationPerformance  = new EducationPerformance
            {
                KeyStage5Performance = new List<KeyStage5> {new KeyStage5 {Year = "2019"}},
                KeyStage5AdditionalInformation = "some additional info"
            };

            _subject = new KeyStage5Performance(GetInformationForProject.Object, ProjectRepository.Object)
            {
                Urn = ProjectUrn0001,
                AcademyUkprn = AcademyUkprn,
                AdditionalInformationViewModel = new AdditionalInformationViewModel()
            };
        }

        public class OnGetAsyncTests : KeyStage5PerformanceTests
        {
            [Theory]
            [InlineData("1234")]
            [InlineData("4321")]
            public async void OnGet_GetInformationForProjectId(string id)
            {
                _subject.Urn = id;
                await _subject.OnGetAsync();

                GetInformationForProject.Verify(s => s.Execute(id));
            }

            [Fact]
            public async void OnGet_AssignsCorrectValuesToViewModel()
            {
                var result = await _subject.OnGetAsync();

                Assert.IsType<PageResult>(result);
                Assert.Equal(FoundInformationForProject.Project.Urn, _subject.Urn);
                Assert.Equal(FoundInformationForProject.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn).EducationPerformance, _subject.EducationPerformance);
                Assert.Equal(FoundInformationForProject.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn).Ukprn, _subject.AcademyUkprn);
                
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheViewModel()
            {
                await _subject.OnGetAsync();

                Assert.Equal(
                    FoundInformationForProject.OutgoingAcademies.First(a => a.Ukprn == AcademyUkprn)
                        .EducationPerformance.KeyStage5AdditionalInformation,
                    _subject.AdditionalInformationViewModel.AdditionalInformation);
                Assert.Equal(ProjectUrn0001, _subject.AdditionalInformationViewModel.Urn);
            }

            [Fact]
            public async void GivenReturnToPreview_UpdatesTheViewModel()
            {
                _subject.ReturnToPreview = true;
                await _subject.OnGetAsync();

                Assert.True(_subject.ReturnToPreview);
            }
        }

        public class OnPostAsyncTests : KeyStage5PerformanceTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnPostAsync();
                ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectModel()
            {
                const string additionalInfo = "some additional info";
                _subject.AdditionalInformationViewModel.AdditionalInformation = additionalInfo;
                var response = await _subject.OnPostAsync();

                var redirectToPageResponse = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal("KeyStage5Performance", redirectToPageResponse.PageName);
                Assert.Null(redirectToPageResponse.PageHandler);
                Assert.Equal(additionalInfo,
                    FoundProjectFromRepo.TransferringAcademies.First(a => a.OutgoingAcademyUkprn == AcademyUkprn)
                        .KeyStage5PerformanceAdditionalInformation);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectCorrectly()
            {
                const string additionalInfo = "some additional info";
                _subject.AdditionalInformationViewModel.AdditionalInformation = additionalInfo;
                await _subject.OnPostAsync();
                ProjectRepository.Verify(r => r.Update(It.Is<Project>(
                    project => project.TransferringAcademies.First(a => a.OutgoingAcademyUkprn == AcademyUkprn)
                        .KeyStage5PerformanceAdditionalInformation == additionalInfo
                )));
            }

            [Fact]
            public async void GivenReturnToPreview_RedirectsToThePreviewPage()
            {
                _subject.ReturnToPreview = true;
                var response = await _subject.OnPostAsync();

                var redirectResponse = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal(Links.HeadteacherBoard.Preview.PageName, redirectResponse.PageName);
                Assert.Equal(_subject.Urn, redirectResponse.RouteValues["Urn"]);
            }
        }
    }
}