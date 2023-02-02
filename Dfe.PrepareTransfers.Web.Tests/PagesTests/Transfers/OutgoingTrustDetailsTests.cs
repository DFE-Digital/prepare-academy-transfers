
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Web.Pages.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Session;
using Moq;
using System.Linq;
using System.Text;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Transfers
{
    public class OutgoingTrustDetailsTests
    {
        protected const string trustId = "9a7be920-eaa0-e911-a83f-000d3a3855a3";
        protected readonly Mock<ITrusts> trustsRepository;
        protected readonly Mock<ISession> session;
        protected readonly OutgoingTrustDetailsModel subject;

        public OutgoingTrustDetailsTests()
        {
            trustsRepository = new Mock<ITrusts>();

            session = new Mock<ISession>();

            var trustIdByteArray = Encoding.UTF8.GetBytes(trustId);
            session.Setup(s => s.TryGetValue("OutgoingTrustId", out trustIdByteArray)).Returns(true);

            var sessionFeature = new SessionFeature { Session = session.Object };
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

            subject = new OutgoingTrustDetailsModel(trustsRepository.Object)
            {
                PageContext = pageContext,
                TempData = tempData
            };
        }

        public class Onget : OutgoingTrustDetailsTests
        {
            private readonly Trust _foundTrust;

            public Onget()
            {
                _foundTrust = new Trust
                {
                    Ukprn = trustId,
                    Name = "Example trust"
                };

                trustsRepository.Setup(r => r.GetByUkprn(trustId)).ReturnsAsync(
                    new RepositoryResult<Trust>
                    {
                        Result = _foundTrust
                    }
                );

            }

            [Fact]
            public async void GivenId_RetrievesTrustAndAssignsToTrustProperty()
            {
                subject.TrustId = trustId;

                var response = await subject.OnGetAsync();

                trustsRepository.Verify(r => r.GetByUkprn(trustId), Times.Once);
                Assert.IsType<PageResult>(response);
                Assert.Equal(_foundTrust, subject.Trust);
            }

            [Fact]
            // Ensure query string gets bound to model when in the format ?query=trust&trustId=1001
            public void BindsPropertyIsPresentWithCorrectOptions()
            {
                var trustSearchModel = new OutgoingTrustDetailsModel(trustsRepository.Object);
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
                await subject.OnGetAsync(change: true);

                Assert.True((bool)subject.ViewData["ChangeLink"]);
            }

            [Fact]
            public async void GivenTrustIdIsNotPresent_RedirectToTrustSearchPageWithAnError()
            {
                subject.SearchQuery = "";

                var response = await subject.OnGetAsync();

                var redirectResponse = AssertRedirectToPage(response, "/Transfers/TrustSearch");
                Assert.Equal("", redirectResponse.RouteValues["query"]);
                Assert.Equal("Select a trust", subject.TempData["ErrorMessage"]);
            }
        }

        public class OnPost : OutgoingTrustDetailsTests
        {
            [Fact]
            public void GivenTrustId_StoresTheTrustInTheSessionAndRedirects()
            {
                subject.TrustId = trustId;

                var response = subject.OnPost();

                session.Verify(s => s.Set(
                    "OutgoingTrustId",
                    It.Is<byte[]>(input =>
                        Encoding.UTF8.GetString(input) == trustId
                    )));
                AssertRedirectToPage(response, "/Transfers/OutgoingTrustAcademies");
            }

            [Fact]
            public void GivenTrustId_ClearExistingInformationInTheSession()
            {
                subject.TrustId = trustId;

                var response = subject.OnPost();

                session.Verify(s => s.Remove("IncomingTrustId"));
                session.Verify(s => s.Remove("OutgoingAcademyIds"));
            }
        }

        protected static RedirectToPageResult AssertRedirectToPage(IActionResult response, string pageName)
        {
            var redirectResponse = Assert.IsType<RedirectToPageResult>(response);
            Assert.Equal(pageName, redirectResponse.PageName);
            return redirectResponse;
        }
    }
}
