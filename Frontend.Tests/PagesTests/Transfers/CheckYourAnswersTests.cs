using Data;
using Data.Models;
using Frontend.Pages.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Session;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Frontend.Tests.PagesTests.Transfers
{
    public class CheckYourAnswersTests
    {
        private readonly PageContext _pageContext;
        private readonly TempDataDictionary _tempData;
        private readonly Mock<ITrusts> _trustsRepository;
        private readonly Mock<ISession> _session;
        private readonly CheckYourAnswersModel _subject;

        private readonly Trust _outgoingTrust;

        private readonly Trust _incomingTrust = new Trust
        { Ukprn = "9a7be920-eaa0-e911-a83f-000d3a385210" };

        private readonly Academy _academyOne = new Academy
        { Ukprn = "9a7be920-eaa0-e911-a83f-000d3a385211" };

        private readonly Academy _academyTwo = new Academy
        { Ukprn = "9a7be920-eaa0-e911-a83f-000d3a385212" };

        private readonly Academy _academyThree = new Academy
        { Ukprn = "9a7be920-eaa0-e911-a83f-000d3a385213" };

        public CheckYourAnswersTests()
        {
            _session = new Mock<ISession>();
            _trustsRepository = new Mock<ITrusts>();

            var httpContext = new DefaultHttpContext();
            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);

            var sessionFeature = new SessionFeature { Session = _session.Object };
            httpContext.Features.Set<ISessionFeature>(sessionFeature);

            _tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _pageContext = new PageContext()
            {
                ViewData = viewData,
                HttpContext = httpContext
            };

            _subject = new CheckYourAnswersModel(_trustsRepository.Object)
            {
                PageContext = _pageContext,
                TempData = _tempData
            };
            _outgoingTrust = new Trust
            {
                Ukprn = "9a7be920-eaa0-e911-a83f-000d3a3852af",
                Academies = new List<Academy>
                    {
                        _academyOne, _academyTwo, _academyThree
                    }
            };

            var outgoingTrustIdByteArray = Encoding.UTF8.GetBytes(_outgoingTrust.Ukprn);
            _session.Setup(s => s.TryGetValue("OutgoingTrustId", out outgoingTrustIdByteArray)).Returns(true);

            var incomingTrustIdByteArray = Encoding.UTF8.GetBytes(_incomingTrust.Ukprn);
            _session.Setup(s => s.TryGetValue("IncomingTrustId", out incomingTrustIdByteArray)).Returns(true);

            var outgoingAcademyIds = new List<string> { _academyOne.Ukprn, _academyTwo.Ukprn };
            var outgoingAcademyIdsByteArray = Encoding.UTF8.GetBytes(string.Join(",", outgoingAcademyIds));
            _session.Setup(s => s.TryGetValue("OutgoingAcademyIds", out outgoingAcademyIdsByteArray)).Returns(true);

            _trustsRepository.Setup(r => r.GetByUkprn(_outgoingTrust.Ukprn)).ReturnsAsync(
                new RepositoryResult<Trust>
                {
                    Result = _outgoingTrust
                });

            _trustsRepository.Setup(r => r.GetByUkprn(_incomingTrust.Ukprn)).ReturnsAsync(
                new RepositoryResult<Trust>
                {
                    Result = _incomingTrust
                });
        }

        [Fact]
        public async void GivenIncomingTrustNotSelected_RendersTheViewCorrectly()
        {
            byte[] incomingTrustIdByteArray = null;
            _session.Setup(s => s.TryGetValue("IncomingTrustId", out incomingTrustIdByteArray)).Returns(true);

            var response = await _subject.OnGetAsync();

            var viewResponse = Assert.IsType<PageResult>(response);
            Assert.Null(_subject.IncomingTrust);
        }

        [Fact]
        public async void GivenAllInformationInSession_CallsTheAPIsWithTheStoredIDs()
        {
            await _subject.OnGetAsync();

            _trustsRepository.Verify(r => r.GetByUkprn(_outgoingTrust.Ukprn), Times.Once);
            _trustsRepository.Verify(r => r.GetByUkprn(_incomingTrust.Ukprn), Times.Once);
        }

        [Fact]
        public async void GivenAllInformationInSession_SetsPropertiesCorrectly()
        {
            var response = await _subject.OnGetAsync();
            var expectedAcademyIds = new List<string> { _academyOne.Ukprn, _academyTwo.Ukprn };
            var AcademyIds = _subject.OutgoingAcademies.Select(academy => academy.Ukprn);

            Assert.IsType<PageResult>(response);
            Assert.Equal(_outgoingTrust.Ukprn, _subject.OutgoingTrust.Ukprn);
            Assert.Equal(_incomingTrust.Ukprn, _subject.IncomingTrust.Ukprn);
            Assert.Equal(expectedAcademyIds, AcademyIds);
        }
    }
}
