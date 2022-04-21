using Data;
using Data.Models;
using Frontend.Pages.Transfers;
using Frontend.Views.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Frontend.Tests.PagesTests.Transfers
{
    public class TrustSearchTests
    {
        private readonly TrustSearchModel _subject;
        private readonly Mock<ITrusts> _trustsRepository;

        public TrustSearchTests()
        {
            _trustsRepository = new Mock<ITrusts>();

            var httpContext = new DefaultHttpContext();
            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var pageContext = new PageContext()
            {
                ViewData = viewData
            };

            _subject = new TrustSearchModel(_trustsRepository.Object)
            {
                PageContext = pageContext,
                TempData = tempData
            };
        }

        [Fact]
        public async Task GivenErrorMessage_AddsErrorToModelState()
        {
            _trustsRepository.Setup(r => r.SearchTrusts("test", ""))
                .ReturnsAsync(
                    new RepositoryResult<List<TrustSearchResult>>
                    {
                        Result = new List<TrustSearchResult>
                        {
                            new TrustSearchResult { Academies = new List<TrustSearchAcademy> { new TrustSearchAcademy()} }
                        }
                    });

            _subject.TempData["ErrorMessage"] = "This is an error message";
            _subject.SearchQuery = "test";

            await _subject.OnGetAsync();

            Assert.Equal("This is an error message", _subject.ModelState["Trusts"].Errors.First().ErrorMessage);
        }

        [Fact]
        public async void GivenSearchingByEmptyString_RedirectToTrustNamePageWithAnError()
        {
            _subject.SearchQuery = "";

            var response = await _subject.OnGetAsync();

            AssertRedirectToPage(response, "/Transfers/TrustName");
            Assert.Equal("Enter the outgoing trust name", _subject.TempData["ErrorMessage"]);
        }

        [Fact]
        public async void GivenSearchReturnsNoTrusts_RedirectToTrustNamePageWithAnError()
        {
            _trustsRepository.Setup(r => r.SearchTrusts("Meow", ""))
                .ReturnsAsync(
                    new RepositoryResult<List<TrustSearchResult>> { Result = new List<TrustSearchResult>() });
            _subject.SearchQuery = "Meow";

            var response = await _subject.OnGetAsync();

            var redirectResponse = AssertRedirectToPage(response, "/Transfers/TrustName");
            Assert.Equal("Meow", redirectResponse.RouteValues["query"]);
            Assert.Equal("We could not find any trusts matching your search criteria", _subject.TempData["ErrorMessage"]);
        }

        [Fact]
        public async void GivenSearchReturnsTrustWithNoAcademies_RedirectToTrustNamePageWithAnError()
        {
            _trustsRepository.Setup(r => r.SearchTrusts("Meow", ""))
                .ReturnsAsync(
                    new RepositoryResult<List<TrustSearchResult>>
                    {
                        Result = new List<TrustSearchResult> { new TrustSearchResult
                        {
                            TrustName = "Meow",
                            Ukprn = "test",
                            Academies = new List<TrustSearchAcademy>()
                        } }
                    });
            _subject.SearchQuery = "Meow";

            var response = await _subject.OnGetAsync();

            var redirectResponse = AssertRedirectToPage(response, "/Transfers/TrustName");
            Assert.Equal("Meow", redirectResponse.RouteValues["query"]);
            Assert.Equal("We could not find any trusts matching your search criteria", _subject.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task GivenSearchingByString_SearchesForTrustsAndAssignsToModel()
        {
            const string searchQuery = "Trust name";
            var trusts = new List<TrustSearchResult>
            {
                new TrustSearchResult { Ukprn = "1234", Academies = new List<TrustSearchAcademy> { new TrustSearchAcademy()} },
                new TrustSearchResult { Ukprn = "4321", Academies = new List<TrustSearchAcademy> { new TrustSearchAcademy()} }
            };
            _trustsRepository.Setup(r => r.SearchTrusts(searchQuery, "")).ReturnsAsync(
                new RepositoryResult<List<TrustSearchResult>>
                {
                    Result = trusts
                }
            );
            _subject.SearchQuery = searchQuery;

            var result = await _subject.OnGetAsync();

            _trustsRepository.Verify(r => r.SearchTrusts(searchQuery, ""));
            Assert.Equal(trusts, _subject.Trusts);
        }

        [Fact]
        // Ensure query string gets bound to model when in the format ?query=search-term
        public void BindsPropertyIsPresentWithCorrectOptions()
        {
            var trustSearchModel = new TrustSearchModel(_trustsRepository.Object);
            var attribute = (BindPropertyAttribute)trustSearchModel.GetType()
                .GetProperty("SearchQuery").GetCustomAttributes(typeof(BindPropertyAttribute), false).First();

            Assert.NotNull(attribute);
            Assert.Equal("query", attribute.Name);
            Assert.True(attribute.SupportsGet);
        }

        [Fact]
        public async Task GivenChangeLink_SetChangeLinkinViewData()
        {
            await _subject.OnGetAsync(change: true);

            Assert.True((bool)_subject.ViewData["ChangeLink"]);
        }

        private static RedirectToPageResult AssertRedirectToPage(IActionResult response, string pageName)
        {
            var redirectResponse = Assert.IsType<RedirectToPageResult>(response);
            Assert.Equal(pageName, redirectResponse.PageName);
            return redirectResponse;
        }
    }
}
