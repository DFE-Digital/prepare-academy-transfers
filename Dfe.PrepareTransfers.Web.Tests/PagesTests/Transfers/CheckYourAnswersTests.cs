using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Web.Pages.Transfers;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
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

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Transfers
{
    public class CheckYourAnswersTests
    {
        private readonly PageContext _pageContext;
        private readonly TempDataDictionary _tempData;
        private readonly Mock<IAcademies> _academyRepository;
        private readonly Mock<ITrusts> _trustsRepository;
        private readonly Mock<IProjects> _projectsRepository;
        private readonly Mock<ISession> _session;
        private readonly CheckYourAnswersModel _subject;

        private readonly Trust _outgoingTrust;

        private readonly Trust _incomingTrust = new Trust
        { Ukprn = "9a7be920-eaa0-e911-a83f-000d3a385210", Name = "Test", };

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
            _academyRepository = new Mock<IAcademies>();
            _projectsRepository = new Mock<IProjects>();
            var referenceNumberService = new Mock<IReferenceNumberService>();

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

            _outgoingTrust = new Trust
            {
                Name = "Test",
                Ukprn = "9a7be920-eaa0-e911-a83f-000d3a3852af",
            };

            _subject = new CheckYourAnswersModel(_trustsRepository.Object, _projectsRepository.Object,
                referenceNumberService.Object, _academyRepository.Object)
            {
                PageContext = _pageContext,
                TempData = _tempData,
                OutgoingTrust = _outgoingTrust,
                IncomingTrust = _incomingTrust
            };

            var outgoingTrustIdByteArray = Encoding.UTF8.GetBytes(_outgoingTrust.Ukprn);
            _session.Setup(s => s.TryGetValue("OutgoingTrustId", out outgoingTrustIdByteArray)).Returns(true);

            var incomingTrustIdByteArray = Encoding.UTF8.GetBytes(_incomingTrust.Ukprn);
            _session.Setup(s => s.TryGetValue("IncomingTrustId", out incomingTrustIdByteArray)).Returns(true);

            byte[] proposedTrustNameByteArray = null;
            _session.Setup(s => s.TryGetValue("ProposedTrustName", out proposedTrustNameByteArray)).Returns(true);

            var outgoingAcademyIds = new List<string> { _academyOne.Ukprn, _academyTwo.Ukprn };
            var outgoingAcademyIdsByteArray = Encoding.UTF8.GetBytes(string.Join(",", outgoingAcademyIds));
            _session.Setup(s => s.TryGetValue("OutgoingAcademyIds", out outgoingAcademyIdsByteArray)).Returns(true);

            _trustsRepository.Setup(r => r.GetByUkprn(_outgoingTrust.Ukprn)).ReturnsAsync(
                 _outgoingTrust
                );

            _trustsRepository.Setup(r => r.GetByUkprn(_incomingTrust.Ukprn)).ReturnsAsync(
                _incomingTrust
                );

            _academyRepository.Setup(x => x.GetAcademiesByTrustUkprn(It.IsAny<string>())).Returns(Task.FromResult(new List<Academy>()));
            _academyRepository.Setup(x => x.GetAcademiesByTrustUkprn(It.Is<string>(y => y == _outgoingTrust.Ukprn))).Returns(Task.FromResult(new List<Academy>() { _academyOne, _academyTwo, _academyThree }));
        }

        public class OnGetAsync : CheckYourAnswersTests
        {
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

        public class OnPostAsync : CheckYourAnswersTests
        {
            public OnPostAsync()
            {
                var createdProject = new Project
                {
                    Urn = "Created URN"
                };

                _projectsRepository.Setup(r => r.Create(It.IsAny<Project>()))
                    .ReturnsAsync(new RepositoryResult<Project>
                    {
                        Result = createdProject
                    });
            }

            [Fact]
            public async void GivenSubmittingProject_FetchesProjectInformationFromSession()
            {
                await _subject.OnPostAsync();

                var foundByteArray = It.IsAny<byte[]>();
                _session.Verify(s => s.TryGetValue("OutgoingTrustId", out foundByteArray), Times.Once);
                _session.Verify(s => s.TryGetValue("IncomingTrustId", out foundByteArray), Times.Once);
                _session.Verify(s => s.TryGetValue("OutgoingAcademyIds", out foundByteArray), Times.Once);
                _session.Verify(s => s.TryGetValue("ProposedTrustName", out foundByteArray), Times.Once);
            }

            [Fact]
            public async void GivenSubmittingProjectWithAllValues_InsertsMappedProject()
            {
                await _subject.OnPostAsync();

                _projectsRepository.Verify(
                    r => r.Create(It.Is<Project>(input =>
                        input.TransferringAcademies[0].OutgoingAcademyUkprn == _academyOne.Ukprn &&
                        input.TransferringAcademies[0].IncomingTrustUkprn == _incomingTrust.Ukprn &&
                        input.TransferringAcademies[1].OutgoingAcademyUkprn == _academyTwo.Ukprn &&
                        input.TransferringAcademies[1].IncomingTrustUkprn == _incomingTrust.Ukprn &&
                        input.OutgoingTrustUkprn == _outgoingTrust.Ukprn)),
                    Times.Once);
            }

            [Fact]
            public async void GivenProjectIsInserted_RedirectsToProjectFeaturesWithCreatedId()
            {
                var createdProjectUrn = "12345";
                _projectsRepository.Setup(r => r.Create(It.IsAny<Project>())).ReturnsAsync(
                    new RepositoryResult<Project>
                    {
                        Result = new Project { Urn = "12345" }
                    });

                var response = await _subject.OnPostAsync();

                var responseRedirect = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal($"/Projects/{nameof(Pages.Projects.Index)}", responseRedirect.PageName);
                Assert.Equal(createdProjectUrn, responseRedirect.RouteValues["urn"]);
            }
        }
    }
}
