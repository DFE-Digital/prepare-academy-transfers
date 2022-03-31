using System.Collections.Generic;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Frontend.Pages.Projects.TransferDates;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.TransferDates
{
    public class FirstDiscussedTests : BaseTests
    {
        private readonly FirstDiscussed _subject;

        protected FirstDiscussedTests()
        {
            _subject = new FirstDiscussed(ProjectRepository.Object)
            {
                Urn = ProjectUrn0001
            };
        }

        public class OnGetAsyncTests : FirstDiscussedTests
        {
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnGetAsync();

                ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenExistingProject_AssignsTheProjectToThePageModel()
            {
                FoundProjectFromRepo.Dates = new Data.Models.Projects.TransferDates
                {
                    FirstDiscussed = "15/01/2020",
                    HasFirstDiscussedDate = true
                };
                var response = await _subject.OnGetAsync();

                Assert.IsType<PageResult>(response);
                Assert.Equal(FoundProjectFromRepo.Urn, _subject.Urn);
                Assert.Equal(FoundProjectFromRepo.Dates.FirstDiscussed, _subject.FirstDiscussedViewModel.FirstDiscussed.DateInputAsString());
                Assert.Equal(FoundProjectFromRepo.Dates.HasFirstDiscussedDate, !_subject.FirstDiscussedViewModel.FirstDiscussed.UnknownDate);
            }
        }

        public class OnPostAsyncTests : FirstDiscussedTests
        {
            public OnPostAsyncTests()
            {
                _subject.FirstDiscussedViewModel = new FirstDiscussedViewModel
                {
                    FirstDiscussed = new DateViewModel
                    {
                        Date = new DateInputViewModel { Day = "15", Month = "10", Year = "2021" },
                        UnknownDate = false
                    }
                };
            }
            
            [Fact]
            public async void GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnPostAsync();

                ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async void GivenErrorOnModel_ShouldReturnPage()
            {
                _subject.ModelState.AddModelError(nameof(_subject.FirstDiscussedViewModel.FirstDiscussed.Date), "error");
                var result = await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Data.Models.Project>(project => project.Urn == ProjectUrn0001)), Times.Never);
                
                Assert.IsType<PageResult>(result);
            }
            
            [Fact]
            public async void GivenUrnAndFirstDiscussedDate_UpdatesTheProject()
            {
                await _subject.OnPostAsync();
            
                ProjectRepository.Verify(r =>
                        r.Update(It.Is<Data.Models.Project>(project => project.Dates.FirstDiscussed == _subject.FirstDiscussedViewModel.FirstDiscussed.DateInputAsString()
                        && project.Dates.HasFirstDiscussedDate == !_subject.FirstDiscussedViewModel.FirstDiscussed.UnknownDate)),
                    Times.Once);
            }
            
            [Fact]
            public async void GivenReturnToPreview_RedirectsToPreviewPage()
            {
                _subject.ReturnToPreview = true;

                var response = await _subject.OnPostAsync();
    
                ControllerTestHelpers.AssertResultRedirectsToPage(
                    response, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new {Urn = ProjectUrn0001})
                );
            }
            
            [Fact]
            public async void GivenUrnAndFirstDiscussed_RedirectsBackToTheSummary()
            {
                var result = await _subject.OnPostAsync();

                var routeValues = new RouteValueDictionary(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Urn", ProjectUrn0001)
                });
                ControllerTestHelpers.AssertResultRedirectsToPage(result, $"/Projects/TransferDates/{nameof(Index)}", routeValues);
            }
        }
    }
}