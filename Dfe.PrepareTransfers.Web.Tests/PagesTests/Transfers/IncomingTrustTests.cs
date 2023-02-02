using Dfe.PrepareTransfers.Web.Pages.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Linq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Transfers
{
    public class IncomingTrustTests
    {
        private readonly PageContext _pageContext;
        private readonly TempDataDictionary _tempData;
        private readonly IncomingTrustModel _subject;

        public IncomingTrustTests()
        {
            var httpContext = new DefaultHttpContext();
            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            _tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _pageContext = new PageContext()
            {
                ViewData = viewData
            };

            _subject = new IncomingTrustModel()
            {
                PageContext = _pageContext,
                TempData = _tempData
            };
        }

        [Fact]
        public void GivenErrorMessage_AddsErrorToModelState()
        {
            _subject.TempData["ErrorMessage"] = "This is an error message";

            _subject.OnGet();

            Assert.Equal("This is an error message", _subject.ModelState["SearchQuery"].Errors.First().ErrorMessage);
        }

        [Fact]
        // Ensure query string gets bound to model when in the format ?query=search-term
        public void BindsPropertyIsPresentWithCorrectOptions()
        {
            var trustNameModel = new IncomingTrustModel();
            var attribute = (BindPropertyAttribute)trustNameModel.GetType()
                .GetProperty("SearchQuery").GetCustomAttributes(typeof(BindPropertyAttribute), false).First();

            Assert.NotNull(attribute);
            Assert.Equal("query", attribute.Name);
            Assert.True(attribute.SupportsGet);
        }

        [Fact]
        public void GivenChangeLink_SetChangeLinkinViewData()
        {
            _subject.OnGet(change: true);

            Assert.Equal(true, _subject.ViewData["ChangeLink"]);
        }
    }
}
