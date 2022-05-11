using System;
using System.Collections.Generic;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Models.TransferDates;
using Frontend.Pages.Projects.TransferDates;
using Frontend.Tests.Helpers;
using Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using Index = Frontend.Pages.Projects.TransferDates.Index;

namespace Frontend.Tests.PagesTests.Projects.TransferDates
{
    public class TargetTests : BaseTests
    {
        private readonly Target _subject;

        protected TargetTests()
        {
            _subject = new Target(ProjectRepository.Object)
            {
                Urn = ProjectUrn0001
            };
        }

        public class OnGetAsyncTests : TargetTests
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
                    Target = "15/01/2020",
                    HasTargetDateForTransfer = true
                };
                var response = await _subject.OnGetAsync();

                Assert.IsType<PageResult>(response);
                Assert.Equal(FoundProjectFromRepo.Urn, _subject.Urn);
                Assert.Equal(FoundProjectFromRepo.Dates.Target, _subject.TargetDateViewModel.TargetDate.DateInputAsString());
                Assert.Equal(FoundProjectFromRepo.Dates.HasTargetDateForTransfer, !_subject.TargetDateViewModel.TargetDate.UnknownDate);
            }
        }

        public class OnPostAsyncTests : TargetTests
        {
            public OnPostAsyncTests()
            {
                _subject.TargetDateViewModel = new TargetDateViewModel
                {
                    TargetDate = new DateViewModel
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

            [Theory]
            [InlineData(null, null)]
            [InlineData("01", null)]
            [InlineData(null, "2099")]
            public async void WhenMonthAndYearAreNotSet_DoesNotSetDay(string monthValue, string yearValue)
            {
                _subject.TargetDateViewModel.TargetDate.Date.Day = null;
                _subject.TargetDateViewModel.TargetDate.Date.Month = monthValue;
                _subject.TargetDateViewModel.TargetDate.Date.Year = yearValue;

                await _subject.OnPostAsync();

                Assert.Null(_subject.TargetDateViewModel.TargetDate.Date.Day);
            }
            
            [Fact]
            public async void WhenMonthAndYearAreSet_SetsDayToFirstOfMonth()
            {
                _subject.TargetDateViewModel.TargetDate.Date.Month = "01";
                _subject.TargetDateViewModel.TargetDate.Date.Year = "2099";

                await _subject.OnPostAsync();

                Assert.Equal("01", _subject.TargetDateViewModel.TargetDate.Date.Day);
            }

            [Fact]
            public async void GivenUrnAndTargetDate_UpdatesTheProject()
            {
                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                        r.Update(It.Is<Data.Models.Project>(project =>
                            project.Dates.Target == _subject.TargetDateViewModel.TargetDate.DateInputAsString()
                            && project.Dates.HasTargetDateForTransfer ==
                            !_subject.TargetDateViewModel.TargetDate.UnknownDate)),
                    Times.Once);
            }

            [Fact]
            public async void GivenReturnToPreview_RedirectsToPreviewPage()
            {
                _subject.ReturnToPreview = true;

                var response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(
                    response, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new { Urn = ProjectUrn0001 })
                );
            }

            [Fact]
            public async void GivenUrnAndTarget_RedirectsBackToTheSummary()
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
            public async void GivenTargetTransferDateBeforeHtbDate_SetErrorOnTheModel()
            {
                FoundProjectFromRepo.Dates.Htb = DateTime.Now.AddYears(-1).ToShortDate();
                
                var targetDate = DateTime.Now.AddYears(-2);

                _subject.TargetDateViewModel = new TargetDateViewModel()
                {
                    Urn = "0001",
                    TargetDate = new DateViewModel
                    {
                        Date = new DateInputViewModel
                        {
                            Day = targetDate.Day.ToString(),
                            Month = targetDate.Month.ToString(),
                            Year = targetDate.Year.ToString()
                        }
                    }
                };

                var result = await _subject.OnPostAsync();
                
                Assert.IsType<PageResult>(result);
                Assert.Single(_subject.ModelState[$"TargetDateViewModel.TargetDate.Date.Day"].Errors);
            }
        }
    }
}