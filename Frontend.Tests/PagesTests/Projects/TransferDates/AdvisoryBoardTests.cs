using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Frontend.Pages.Projects.TransferDates;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.TransferDates
{
    public class AdvisoryBoardTests : BaseTests
    {
        private readonly AdvisoryBoard _subject;

        protected AdvisoryBoardTests()
        {
            _subject = new AdvisoryBoard(ProjectRepository.Object)
            {
                Urn = ProjectUrn0001
            };
        }

        public class OnGetAsyncTests : AdvisoryBoardTests
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
                    Htb = "15/01/2020",
                    HasHtbDate = true
                };
                var response = await _subject.OnGetAsync();

                Assert.IsType<PageResult>(response);
                Assert.Equal(FoundProjectFromRepo.Urn, _subject.Urn);
                Assert.Equal(FoundProjectFromRepo.Dates.Htb, _subject.AdvisoryBoardViewModel.AdvisoryBoardDate.DateInputAsString());
                Assert.Equal(FoundProjectFromRepo.Dates.HasHtbDate, !_subject.AdvisoryBoardViewModel.AdvisoryBoardDate.UnknownDate);
            }
        }

        public class OnPostAsyncTests : AdvisoryBoardTests
        {
            public OnPostAsyncTests()
            {
                _subject.AdvisoryBoardViewModel = new AdvisoryBoardViewModel
                {
                    AdvisoryBoardDate = new DateViewModel
                    {
                        Date = new DateInputViewModel { Day = "15", Month = "10", Year = (System.DateTime.Now.Year + 1).ToString() },
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
                _subject.ModelState.AddModelError(nameof(_subject.AdvisoryBoardViewModel.AdvisoryBoardDate.Date),
                    "error");
                var result = await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Data.Models.Project>(project => project.Urn == ProjectUrn0001)), Times.Never);

                Assert.IsType<PageResult>(result);
            }

            [Fact]
            public async void GivenUrnAndAdvisoryBoardDate_UpdatesTheProject()
            {
                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                        r.Update(It.Is<Data.Models.Project>(project =>
                            project.Dates.Htb == _subject.AdvisoryBoardViewModel.AdvisoryBoardDate.DateInputAsString()
                            && project.Dates.HasHtbDate ==
                            !_subject.AdvisoryBoardViewModel.AdvisoryBoardDate.UnknownDate)),
                    Times.Once);
            }

            [Fact]
            public async void GivenReturnToPreview_RedirectsToPreviewPage()
            {
                _subject.ReturnToPreview = true;

                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(
                    response, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new { id = ProjectUrn0001 })
                );
            }

            [Fact]
            public async void GivenUrnAndAdvisoryBoard_RedirectsBackToTheSummary()
            {
                var result = await _subject.OnPostAsync();

                var routeValues = new RouteValueDictionary(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Urn", ProjectUrn0001)
                });
                ControllerTestHelpers.AssertResultRedirectsToPage(result, $"/Projects/TransferDates/{nameof(Index)}",
                    routeValues);
            }
            
            [Fact]
            public async void GivenAdvisoryBoardDateGreaterThanTargetDate_SetsErrorOnViewModel()
            {
                ProjectRepository.Setup(r => r.GetByUrn(It.IsAny<string>()))
                    .ReturnsAsync(new RepositoryResult<Project>
                    {
                        Result = new Project
                        {
                            Urn = "0002",
                            Dates = new Data.Models.Projects.TransferDates
                            {
                                Target = "01/01/2000"
                            }
                        }
                    });
                    
                _subject.AdvisoryBoardViewModel = new AdvisoryBoardViewModel
                {
                    Urn = "0002",
                    AdvisoryBoardDate = new DateViewModel
                    {
                        Date = new DateInputViewModel
                        {
                            Day = "01",
                            Month = "01",
                            Year = (System.DateTime.Now.Year + 1).ToString()
                        }
                    },
                    ReturnToPreview = true
                };

                var result = await _subject.OnPostAsync();
                
                Assert.IsType<PageResult>(result);
                Assert.Single(_subject.ModelState[$"AdvisoryBoardViewModel.AdvisoryBoardDate.Date.Day"].Errors);
            }
        }
    }
}