using Data;
using Data.Models;
using Frontend.Pages.Transfers;
using Frontend.Views.Transfers;
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

namespace Frontend.Tests.PagesTests.Transfers
{
    public class OutgoingTrustAcademiesTests
    {
        private const string AcademyName = "Academy 001";
        private const string AcademyNameTwo = "Academy 002";

        private readonly Mock<ISession> _session;
        private readonly Mock<ITrusts> _trustsRepository;
        private readonly OutgoingTrustAcademiesModel _subject;

        public OutgoingTrustAcademiesTests()
        {
            _session = new Mock<ISession>();
            _trustsRepository = new Mock<ITrusts>();

            const string trustId = "9a7be920-eaa0-e911-a83f-000d3a3852af";
            var trustIdByteArray = Encoding.UTF8.GetBytes(trustId);

            _session.Setup(s => s.TryGetValue("OutgoingTrustId", out trustIdByteArray)).Returns(true);

            _trustsRepository.Setup(r => r.GetByUkprn(trustId)).ReturnsAsync(
                new RepositoryResult<Trust>
                {
                    Result = new Trust
                    {
                        Academies = new List<Academy>
                        {
                            new Academy {Name = AcademyName},
                            new Academy {Name = AcademyNameTwo}
                        }
                    }
                });

            var tempDataProvider = new Mock<ITempDataProvider>();
            var httpContext = new DefaultHttpContext();
            var sessionFeature = new SessionFeature { Session = _session.Object };
            httpContext.Features.Set<ISessionFeature>(sessionFeature);

            var tempDataDictionaryFactory =
                new TempDataDictionaryFactory(tempDataProvider.Object);
            var tempData = tempDataDictionaryFactory.GetTempData(httpContext);
            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var pageContext = new PageContext()
            {
                ViewData = viewData,
                HttpContext = httpContext
            };

            _subject = new OutgoingTrustAcademiesModel(_trustsRepository.Object)
            {
                TempData = tempData,
                PageContext = pageContext,
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
        public async void GivenNoErrorMessage_SetsErrorExistsToFalse()
        {
            var response = await _subject.OnGetAsync();

            Assert.IsType<PageResult>(response);
            Assert.Equal(false, _subject.ViewData["Error.Exists"]);
        }

        [Fact]
        public async void GivenErrorMessage_PutsTheErrorIntoTheViewData()
        {
            _subject.TempData["ErrorMessage"] = "Error message";

            var response = await _subject.OnGetAsync();
            
            Assert.IsType<PageResult>(response);
            Assert.Equal(true, _subject.ViewData["Error.Exists"]);
            Assert.Equal("Error message", _subject.ViewData["Error.Message"]);
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
}
