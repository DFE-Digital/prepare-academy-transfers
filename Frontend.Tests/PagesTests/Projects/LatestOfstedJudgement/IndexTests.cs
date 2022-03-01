using System.Collections.Generic;
using System.Linq;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Pages.Projects.LatestOfstedJudgement;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.LatestOfstedJudgement
{
    public class IndexTests : BaseTests
    {
        private readonly Index _subject;

        public IndexTests()
        {
            _subject = new Index(GetInformationForProject.Object, ProjectRepository.Object)
            {
                Urn = ProjectUrn0001,
                AcademyUkprn = AcademyUkprn,
                AdditionalInformationViewModel = new AdditionalInformationViewModel
                {
                    AdditionalInformation = "some additional information"
                }
            };
        }

        public class OnGetAsyncTests : IndexTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnGetAsync();

                GetInformationForProject.Verify(r => r.Execute(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenUrn_AssignsModelToThePage()
            {
                var response = await _subject.OnGetAsync();

                Assert.IsType<PageResult>(response);
                Assert.Equal(ProjectUrn0001, _subject.Urn);
                Assert.Equal(AcademyUrn, _subject.OutgoingAcademyUrn);
            }
        }

        public class OnPostAsyncTests : IndexTests
        {
            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProject()
            {
                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                        r.Update(It.Is<Data.Models.Project>(project =>
                            project.TransferringAcademies.First(a => a.OutgoingAcademyUkprn == _subject.AcademyUkprn)
                                .LatestOfstedReportAdditionalInformation
                            == _subject.AdditionalInformationViewModel.AdditionalInformation)),
                    Times.Once);
            }

            [Fact]
            public async void GivenAdditionalInformation_UpdatesTheProjectModel()
            {
                var response = await _subject.OnPostAsync();

                var routeValues = new RouteValueDictionary(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Urn", ProjectUrn0001)
                });
                ControllerTestHelpers.AssertResultRedirectsToPage(response,
                    $"/Projects/LatestOfstedJudgement/{nameof(Index)}", routeValues);
                Assert.Equal(_subject.AdditionalInformationViewModel.AdditionalInformation, FoundProjectFromRepo
                    .TransferringAcademies
                    .First(a => a.OutgoingAcademyUkprn == _subject.AcademyUkprn)
                    .LatestOfstedReportAdditionalInformation);
            }

            [Fact]
            public async void GivenReturnToPreview_RedirectsToPreviewPage()
            {
                _subject.ReturnToPreview = true;
                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(
                    response,
                    Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new {Urn = ProjectUrn0001})
                );
            }
        }
    }
}