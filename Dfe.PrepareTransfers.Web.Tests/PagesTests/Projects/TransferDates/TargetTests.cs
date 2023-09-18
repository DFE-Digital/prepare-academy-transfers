using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Dfe.PrepareTransfers.Web.Models.TransferDates;
using Dfe.PrepareTransfers.Web.Pages.Projects.TransferDates;
using Dfe.PrepareTransfers.Web.Tests.Dfe.PrepareTransfers.Helpers;
using Dfe.PrepareTransfers.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using Index = Dfe.PrepareTransfers.Web.Pages.Projects.TransferDates.Index;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.TransferDates
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
            public async Task GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnGetAsync();

                ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
            }

            [Fact]
            public async Task GivenExistingProject_AssignsTheProjectToThePageModel()
            {
                FoundProjectFromRepo.Dates = new Data.Models.Projects.TransferDates
                {
                    Target = "15/01/2020",
                    HasTargetDateForTransfer = true
                };
                IActionResult response = await _subject.OnGetAsync();

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
                        Date = new DateInputViewModel { Day = "15", Month = "10", Year = (DateTime.Now.Year + 1).ToString() },
                        UnknownDate = false
                    }
                };
            }

            [Fact]
            public async Task GivenUrn_FetchesProjectFromTheRepository()
            {
                await _subject.OnPostAsync();

                ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
            }
            
            [Fact]
            public void TargetDateViewModel_SkipsAutomaticValidation()
            {
               var attribute = (CustomizeValidatorAttribute)typeof(Target).GetProperty("TargetDateViewModel")
                  ?.GetCustomAttributes(typeof(CustomizeValidatorAttribute), false)
                  .First();

                Assert.NotNull(attribute);
                Assert.True(attribute.Skip);
            }

            [Fact]
            public async Task WhenMonthAndYearAreNotSet_DoesNotSetDay()
            {
                _subject.TargetDateViewModel.TargetDate.Date.Day = null;
                _subject.TargetDateViewModel.TargetDate.Date.Month = null;
                _subject.TargetDateViewModel.TargetDate.Date.Year = null;

                await _subject.OnPostAsync();

                Assert.Null(_subject.TargetDateViewModel.TargetDate.Date.Day);
            }
            
            [Theory]
            [InlineData("01", null)]
            [InlineData(null, "2099")]
            [InlineData("01", "2099")]
            public async Task WhenMonthOrYearIsSet_SetsDayToFirstOfMonth(string monthValue, string yearValue)
            {
                _subject.TargetDateViewModel.TargetDate.Date.Month = monthValue;
                _subject.TargetDateViewModel.TargetDate.Date.Year = yearValue;

                await _subject.OnPostAsync();

                Assert.Equal("01", _subject.TargetDateViewModel.TargetDate.Date.Day);
            }

            [Fact]
            public async Task GivenUrnAndTargetDate_UpdatesTheProject()
            {
                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                        r.UpdateDates(It.Is<Data.Models.Project>(project =>
                            project.Dates.Target == _subject.TargetDateViewModel.TargetDate.DateInputAsString()
                            && project.Dates.HasTargetDateForTransfer ==
                            !_subject.TargetDateViewModel.TargetDate.UnknownDate)),
                    Times.Once);
            }

            [Fact]
            public async Task GivenReturnToPreview_RedirectsToPreviewPage()
            {
                _subject.ReturnToPreview = true;

                IActionResult response = await _subject.OnPostAsync();

                ControllerTestHelpers.AssertResultRedirectsToPage(
                    response, Links.HeadteacherBoard.Preview.PageName,
                    new RouteValueDictionary(new { Urn = ProjectUrn0001 })
                );
            }

            [Fact]
            public async Task GivenUrnAndTarget_RedirectsBackToTheSummary()
            {
                IActionResult result = await _subject.OnPostAsync();

                var routeValues = new RouteValueDictionary(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Urn", ProjectUrn0001)
                });
                ControllerTestHelpers.AssertResultRedirectsToPage(result, $"/Projects/TransferDates/{nameof(Index)}",
                    routeValues);
            }
            
            [Fact]
            public async Task GivenTargetTransferDateBeforeHtbDate_SetErrorOnTheModel()
            {
                FoundProjectFromRepo.Dates.Htb = DateTime.Now.AddYears(-1).ToShortDate();
                
                DateTime targetDate = DateTime.Now.AddYears(-2);

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

                IActionResult result = await _subject.OnPostAsync();
                
                Assert.IsType<PageResult>(result);
                _subject.ModelState.Values.First().ValidationState.Should().Be(ModelValidationState.Invalid);
            }
        }
    }
}