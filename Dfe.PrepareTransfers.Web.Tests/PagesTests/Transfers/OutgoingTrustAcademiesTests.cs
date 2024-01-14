using Dfe.Academies.Contracts.V4.Trusts;
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
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Transfers
{
    public class OutgoingTrustAcademiesTests
    {
        protected readonly PageContext pageContext;

        private const string AcademyName = "Academy 001";
        private const string AcademyNameTwo = "Academy 002";

        private readonly Mock<ISession> _session;
        private readonly Mock<ITrusts> _trustsRepository;
        private readonly Mock<IAcademies> _academyRepository;

        public OutgoingTrustAcademiesTests()
        {
            _session = new Mock<ISession>();
            _trustsRepository = new Mock<ITrusts>();
            _academyRepository = new Mock<IAcademies>();

            const string trustId = "9a7be920-eaa0-e911-a83f-000d3a3852af";
            var trustIdByteArray = Encoding.UTF8.GetBytes(trustId);

            _session.Setup(s => s.TryGetValue("OutgoingTrustId", out trustIdByteArray)).Returns(true);

            var trustUkprn = "1234";

            _trustsRepository.Setup(r => r.GetByUkprn(trustId)).ReturnsAsync(
                new Trust
                {
                    Ukprn = trustUkprn

                });
            _academyRepository.Setup(r => r.GetAcademiesByTrustUkprn(trustUkprn)).ReturnsAsync(
                new List<Academy>
                {

                        new Academy {Name = AcademyName},
                        new Academy {Name = AcademyNameTwo}

                });
            var httpContext = new DefaultHttpContext();
            var sessionFeature = new SessionFeature { Session = _session.Object };
            httpContext.Features.Set<ISessionFeature>(sessionFeature);

            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            pageContext = new PageContext()
            {
                ViewData = viewData,
                HttpContext = httpContext
            };
        }

        public class OnGetAsync : OutgoingTrustAcademiesTests
        {
            private readonly OutgoingTrustAcademiesModel _subject;

            public OnGetAsync()
            {
                _subject = new OutgoingTrustAcademiesModel(_trustsRepository.Object, _academyRepository.Object)
                {
                    PageContext = pageContext
                };
            }

            [Fact]
            public async void GivenTrustIdInSession_FetchesTheAcademiesForThatTrust()
            {
                var response = await _subject.OnGetAsync();

                Assert.IsType<PageResult>(response);
                Assert.Equal(AcademyName, _subject.Academies[0].Name);
                Assert.Equal(AcademyNameTwo, _subject.Academies[1].Name);
            }

            [Fact]
            public async void GivenChangeLink_PutsChangeLinkIntoTheViewData()
            {
                var response = await _subject.OnGetAsync(change: true);

                Assert.IsType<PageResult>(response);
                Assert.Equal(true, _subject.ViewData["ChangeLink"]);
            }

            [Fact]
            public async void GivenOutgoingAcademiesExistInSession_PutsOutgoingAcademyIdIntoTheViewData()
            {
                const string outgoingAcademyId = "AcademyId";
                var outgoingAcademyIds = new List<string> { outgoingAcademyId };
                var outgoingAcademyIdsByteArray = Encoding.UTF8.GetBytes(string.Join(",", outgoingAcademyIds));
                _session.Setup(s => s.TryGetValue("OutgoingAcademyIds", out outgoingAcademyIdsByteArray)).Returns(true);

                var response = await _subject.OnGetAsync();

                Assert.IsType<PageResult>(response);
                Assert.Equal(outgoingAcademyId, _subject.ViewData["OutgoingAcademyId"]);
            }
        }

        public class OnPostAsync : OutgoingTrustAcademiesTests
        {
            [Fact]
            public async void WhenSelectedAcademyIdsIsNull_UpdatesTheModelStateAndReturnPageResult()
            {
                var subject = new OutgoingTrustAcademiesModel(_trustsRepository.Object, _academyRepository.Object)
                {
                    SelectedAcademyIds = null,
                    PageContext = pageContext
                };

                var result = await subject.OnPostAsync();

                Assert.IsType<PageResult>(result);
                Assert.False(subject.ModelState.IsValid);
            }

            [Fact]
            public async void GivenAcademyId_StoresItInTheSessionAndRedirectsToIncomingTrust()
            {
                const string academyId = "9a7be920-eaa0-e911-a83f-000d3a3852af";
                var subject = new OutgoingTrustAcademiesModel(_trustsRepository.Object, _academyRepository.Object)
                {
                    SelectedAcademyIds = new List<string> { academyId },
                    PageContext = pageContext
                };

                var result = await subject.OnPostAsync();

                var resultRedirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("IncomingTrust", resultRedirect.ActionName);
                Assert.Equal("Transfers", resultRedirect.ControllerName);
                _session.Verify(s => s.Set(
                    "OutgoingAcademyIds",
                    It.Is<byte[]>(input =>
                        Encoding.UTF8.GetString(input) == academyId
                )));
            }

            [Fact]
            public async void GivenChangeLink_StoresItInTheSessionAndRedirectsToCheckYourAnswers()
            {
                const string academyId = "9a7be920-eaa0-e911-a83f-000d3a3852af";
                var subject = new OutgoingTrustAcademiesModel(_trustsRepository.Object, _academyRepository.Object)
                {
                    SelectedAcademyIds = new List<string> { academyId },
                    PageContext = pageContext
                };

                var result = await subject.OnPostAsync(change: true);

                var resultRedirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("CheckYourAnswers", resultRedirect.ActionName);
                Assert.Equal("Transfers", resultRedirect.ControllerName);
                _session.Verify(s => s.Set(
                    "OutgoingAcademyIds",
                    It.Is<byte[]>(input =>
                        Encoding.UTF8.GetString(input) == academyId
                )));
            }
        }
    }
}
