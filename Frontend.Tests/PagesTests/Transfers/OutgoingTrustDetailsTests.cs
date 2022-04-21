
using Data;
using Data.Models;
using Frontend.Pages.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Linq;
using Xunit;

namespace Frontend.Tests.PagesTests.Transfers
{
    public class OutgoingTrustDetailsTests
    {
        private readonly string _trustId;
        private readonly Trust _foundTrust;
        private readonly Mock<ITrusts> _trustsRepository;
        private readonly OutgoingTrustDetailsModel _subject;

        public OutgoingTrustDetailsTests()
        {
            _trustsRepository = new Mock<ITrusts>();

            _trustId = "9a7be920-eaa0-e911-a83f-000d3a3855a3";
            _foundTrust = new Trust
            {
                Ukprn = _trustId,
                Name = "Example trust"
            };

            _trustsRepository.Setup(r => r.GetByUkprn(_trustId)).ReturnsAsync(
                new RepositoryResult<Trust>
                {
                    Result = _foundTrust
                }
            );

            var httpContext = new DefaultHttpContext();
            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var pageContext = new PageContext()
            {
                ViewData = viewData
            };

            _subject = new OutgoingTrustDetailsModel(_trustsRepository.Object)
            {
                PageContext = pageContext,
                TempData = tempData
            };
        }

        [Fact]
        public async void GivenId_RetrievesTrustAndAssignsToTrustProperty()
        {
            _subject.TrustId = _trustId;

            var response = await _subject.OnGetAsync();

            _trustsRepository.Verify(r => r.GetByUkprn(_trustId), Times.Once);
            Assert.IsType<PageResult>(response);
            Assert.Equal(_foundTrust, _subject.Trust);
        }

        [Fact]
        // Ensure query string gets bound to model when in the format ?query=trust&trustId=1001
        public void BindsPropertyIsPresentWithCorrectOptions()
        {
            var trustSearchModel = new OutgoingTrustDetailsModel(_trustsRepository.Object);
            var queryAttribute = (BindPropertyAttribute)trustSearchModel.GetType()
                .GetProperty("SearchQuery").GetCustomAttributes(typeof(BindPropertyAttribute), false).First();
            var trustIdAttribute = (BindPropertyAttribute)trustSearchModel.GetType()
                .GetProperty("TrustId").GetCustomAttributes(typeof(BindPropertyAttribute), false).First();

            Assert.NotNull(queryAttribute);
            Assert.Equal("query", queryAttribute.Name);
            Assert.True(queryAttribute.SupportsGet);

            Assert.NotNull(trustIdAttribute);
            Assert.Equal("trustId", trustIdAttribute.Name);
            Assert.True(trustIdAttribute.SupportsGet);
        }

        [Fact]
        public async void GivenChangeLink_AssignChangeLinkToTheView()
        {
            await _subject.OnGetAsync(change: true);

            Assert.True((bool)_subject.ViewData["ChangeLink"]);
        }

        [Fact]
        public async void GivenTrustIdIsNotPresent_RedirectToTrustSearchPageWithAnError()
        {
            _subject.SearchQuery = "";

            var response = await _subject.OnGetAsync();

            var redirectResponse = AssertRedirectToPage(response, "/Transfers/TrustSearch");
            Assert.Equal("", redirectResponse.RouteValues["query"]);
            Assert.Equal("Select the outgoing trust", _subject.TempData["ErrorMessage"]);
        }

        private static RedirectToPageResult AssertRedirectToPage(IActionResult response, string pageName)
        {
            var redirectResponse = Assert.IsType<RedirectToPageResult>(response);
            Assert.Equal(pageName, redirectResponse.PageName);
            return redirectResponse;
        }
    }
}
