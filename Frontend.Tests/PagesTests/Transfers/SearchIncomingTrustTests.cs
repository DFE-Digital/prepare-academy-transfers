using Data;
using Data.Models;
using Frontend.Pages.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Session;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Frontend.Tests.PagesTests.Transfers
{
    public class SearchIncomingTrustTests
    {
        private const string TrustId = "9a7be920-eaa0-e911-a83f-000d3a3855a3";

        private readonly SearchIncomingTrustModel _subject;
        private readonly Mock<ITrusts> _trustsRepository;
        private readonly Mock<ISession> _session;

        public SearchIncomingTrustTests()
        {
            _trustsRepository = new Mock<ITrusts>();
            _session = new Mock<ISession>();

            var trustIdByteArray = Encoding.UTF8.GetBytes(TrustId);
            _session.Setup(s => s.TryGetValue("OutgoingTrustId", out trustIdByteArray)).Returns(true);

            var sessionFeature = new SessionFeature { Session = _session.Object };
            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<ISessionFeature>(sessionFeature);

            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var pageContext = new PageContext()
            {
                ViewData = viewData,
                HttpContext = httpContext
            };

            _subject = new SearchIncomingTrustModel(_trustsRepository.Object)
            {
                PageContext = pageContext,
                TempData = tempData
            };
        }

        public class OnGetAsync : SearchIncomingTrustTests
        {
            [Fact]
            public async void GivenSearchingByEmptyString_RedirectToTrustNamePageWithAnError()
            {
                _subject.SearchQuery = "";

                var response = await _subject.OnGetAsync();

                AssertRedirectToPage(response, "/Transfers/IncomingTrust");
                Assert.Equal("Enter the incoming trust name", _subject.TempData["ErrorMessage"]);
            }

            [Fact]
            public async void GivenNoSearchResultsForString_RedirectToIncomingTrustPageWithError()
            {
                _trustsRepository.Setup(r => r.SearchTrusts("Trust name", TrustId)).ReturnsAsync(
                    new RepositoryResult<List<TrustSearchResult>>
                    {
                        Result = new List<TrustSearchResult>()
                    });
                _subject.SearchQuery = "Trust name";

                var response = await _subject.OnGetAsync();

                var redirectResponse = AssertRedirectToPage(response, "/Transfers/IncomingTrust");
                Assert.Equal("Trust name", redirectResponse.RouteValues["query"]);
                Assert.Equal("We could not find any trusts matching your search criteria", _subject.TempData["ErrorMessage"]);
            }

            [Fact]
            // Ensure query string gets bound to model when in the format ?query=search-term
            public void BindsPropertyIsPresentWithCorrectOptions()
            {
                var trustSearchModel = new SearchIncomingTrustModel(_trustsRepository.Object);
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

            [Fact]
            public async Task GivenSearchingByString_SearchesForTrustsAndAssignsToModel()
            {
                const string searchQuery = "Trust name";
                var trusts = new List<TrustSearchResult>
                {
                    new TrustSearchResult { Ukprn = "1234", Academies = new List<TrustSearchAcademy> { new TrustSearchAcademy()} },
                    new TrustSearchResult { Ukprn = "4321", Academies = new List<TrustSearchAcademy> { new TrustSearchAcademy()} }
                };
                _trustsRepository.Setup(r => r.SearchTrusts(searchQuery, TrustId)).ReturnsAsync(
                    new RepositoryResult<List<TrustSearchResult>>
                    {
                        Result = trusts
                    }
                );
                _subject.SearchQuery = searchQuery;

                var result = await _subject.OnGetAsync();

                _trustsRepository.Verify(r => r.SearchTrusts(searchQuery, TrustId));
                Assert.Equal(trusts, _subject.Trusts);
            }
        }

        public class OnPostAsync : SearchIncomingTrustTests
        {
            [Fact]
            public async void GivenTrustId_StoresTheTrustInTheSessionAndRedirects()
            {
                _subject.SelectedTrustId = TrustId;

                var response = await _subject.OnPostAsync();

                _session.Verify(s => s.Set(
                    "IncomingTrustId",
                    It.Is<byte[]>(input =>
                        Encoding.UTF8.GetString(input) == TrustId
                    )));
                AssertRedirectToAction(response, "CheckYourAnswers");
            }

            [Fact]
            public async void WhenSelectedAcademyIdsIsNull_UpdatesTheModelStateAndReturnsPageResult()
            {
                const string searchQuery = "Trust name";
                var trusts = new List<TrustSearchResult>
                {
                    new TrustSearchResult { Ukprn = "1234", Academies = new List<TrustSearchAcademy> { new TrustSearchAcademy()} },
                    new TrustSearchResult { Ukprn = "4321", Academies = new List<TrustSearchAcademy> { new TrustSearchAcademy()} }
                };
                _trustsRepository.Setup(r => r.SearchTrusts(searchQuery, TrustId)).ReturnsAsync(
                    new RepositoryResult<List<TrustSearchResult>>
                    {
                        Result = trusts
                    }
                );
                await _subject.OnGetAsync();
                _subject.SelectedTrustId = null;

                var result = await _subject.OnPostAsync();

                Assert.False(_subject.ModelState.IsValid);
            }
        }

        private static RedirectToPageResult AssertRedirectToPage(IActionResult response, string pageName)
        {
            var redirectResponse = Assert.IsType<RedirectToPageResult>(response);
            Assert.Equal(pageName, redirectResponse.PageName);
            return redirectResponse;
        }

        private static RedirectToActionResult AssertRedirectToAction(IActionResult response, string actionName)
        {
            var redirectResponse = Assert.IsType<RedirectToActionResult>(response);
            Assert.Equal(actionName, redirectResponse.ActionName);
            return redirectResponse;
        }
    }
}
