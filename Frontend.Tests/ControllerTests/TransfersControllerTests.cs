using Data;
using Data.Models;
using Frontend.Controllers;
using Frontend.Views.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Session;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frontend.Services.Interfaces;
using Xunit;

namespace Frontend.Tests.ControllerTests
{
    public class TransfersControllerTests
    {
        private readonly Mock<IAcademies> _academiesRepository;
        private readonly Mock<IProjects> _projectsRepository;
        private readonly Mock<ITrusts> _trustsRepository;

        private readonly TransfersController _subject;
        private readonly Mock<ISession> _session;

        public TransfersControllerTests()
        {
            _academiesRepository = new Mock<IAcademies>();
            _projectsRepository = new Mock<IProjects>();
            _trustsRepository = new Mock<ITrusts>();
            _session = new Mock<ISession>();
            var referenceNumberService = new Mock<IReferenceNumberService>();

            var tempDataProvider = new Mock<ITempDataProvider>();
            var httpContext = new DefaultHttpContext();
            var sessionFeature = new SessionFeature {Session = _session.Object};
            httpContext.Features.Set<ISessionFeature>(sessionFeature);

            var tempDataDictionaryFactory =
                new TempDataDictionaryFactory(tempDataProvider.Object);
            var tempData = tempDataDictionaryFactory.GetTempData(httpContext);

            _subject = new TransfersController(_projectsRepository.Object,
                _trustsRepository.Object, referenceNumberService.Object) {TempData = tempData, ControllerContext = {HttpContext = httpContext}};
        }

        public class ConfirmIncomingTrustTests : TransfersControllerTests
        {
            [Fact]
            public async void GivenTrustId_StoresTheTrustInTheSessionAndRedirects()
            {
                const string trustId = "9a7be920-eaa0-e911-a83f-000d3a3852af";
                var response = await _subject.ConfirmIncomingTrust(trustId);

                _session.Verify(s => s.Set(
                    "IncomingTrustId",
                    It.Is<byte[]>(input =>
                        Encoding.UTF8.GetString(input) == trustId
                    )));
                AssertRedirectToAction(response, "CheckYourAnswers");
            }
            
            [Fact]
            public async void GivenNoTrustId_RedirectBackToSearchIncomingTrustPageWithError()
            {
                var result = await _subject.ConfirmIncomingTrust(null);

                var resultRedirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("SearchIncomingTrust", resultRedirect.ActionName);
                Assert.Equal("Select an incoming trust", _subject.TempData["ErrorMessage"]);
            }
        }

        public class CheckYourAnswersTests : TransfersControllerTests
        {
            private readonly Trust _outgoingTrust;

            private readonly Trust _incomingTrust = new Trust
                {Ukprn = "9a7be920-eaa0-e911-a83f-000d3a385210"};

            private readonly Academy _academyOne = new Academy
                {Ukprn = "9a7be920-eaa0-e911-a83f-000d3a385211"};

            private readonly Academy _academyTwo = new Academy
                {Ukprn = "9a7be920-eaa0-e911-a83f-000d3a385212"};

            private readonly Academy _academyThree = new Academy
                {Ukprn = "9a7be920-eaa0-e911-a83f-000d3a385213"};

            public CheckYourAnswersTests()
            {
                _outgoingTrust = new Trust
                {
                    Ukprn = "9a7be920-eaa0-e911-a83f-000d3a3852af", Academies = new List<Academy>
                    {
                        _academyOne, _academyTwo, _academyThree
                    }
                };

                var outgoingTrustIdByteArray = Encoding.UTF8.GetBytes(_outgoingTrust.Ukprn);
                _session.Setup(s => s.TryGetValue("OutgoingTrustId", out outgoingTrustIdByteArray)).Returns(true);

                var incomingTrustIdByteArray = Encoding.UTF8.GetBytes(_incomingTrust.Ukprn);
                _session.Setup(s => s.TryGetValue("IncomingTrustId", out incomingTrustIdByteArray)).Returns(true);

                var outgoingAcademyIds = new List<string> {_academyOne.Ukprn, _academyTwo.Ukprn};
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

                var response = await _subject.CheckYourAnswers();
                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<CheckYourAnswers>(viewResponse.Model);
                Assert.Null(viewModel.IncomingTrust);
            }

            [Fact]
            public async void GivenAllInformationInSession_CallsTheAPIsWithTheStoredIDs()
            {
                await _subject.CheckYourAnswers();
                _trustsRepository.Verify(r => r.GetByUkprn(_outgoingTrust.Ukprn), Times.Once);
                _trustsRepository.Verify(r => r.GetByUkprn(_incomingTrust.Ukprn), Times.Once);
            }

            [Fact]
            public async void GivenAllInformationInSession_CreatesTheViewModelCorrectly()
            {
                var response = await _subject.CheckYourAnswers();

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<CheckYourAnswers>(viewResponse.Model);

                Assert.Equal(_outgoingTrust.Ukprn, viewModel.OutgoingTrust.Ukprn);
                Assert.Equal(_incomingTrust.Ukprn, viewModel.IncomingTrust.Ukprn);

                var expectedAcademyIds = new List<string> {_academyOne.Ukprn, _academyTwo.Ukprn};
                var viewAcademyIds = viewModel.OutgoingAcademies.Select(academy => academy.Ukprn);

                Assert.Equal(expectedAcademyIds, viewAcademyIds);
            }
        }

        public class SubmitProjectTests : TransfersControllerTests
        {
            private readonly Trust _outgoingTrust = new Trust
                {Ukprn = "9a7be920-eaa0-e911-a83f-000d3a3852af"};

            private readonly Trust _incomingTrust = new Trust
                {Ukprn = "9a7be920-eaa0-e911-a83f-000d3a385210"};

            private readonly Academy _academyOne = new Academy
                {Ukprn = "9a7be920-eaa0-e911-a83f-000d3a385211"};

            private readonly Academy _academyTwo = new Academy
                {Ukprn = "9a7be920-eaa0-e911-a83f-000d3a385212"};

            public SubmitProjectTests()
            {
                var outgoingTrustIdByteArray = Encoding.UTF8.GetBytes(_outgoingTrust.Ukprn);
                _session.Setup(s => s.TryGetValue("OutgoingTrustId", out outgoingTrustIdByteArray)).Returns(true);

                var incomingTrustIdByteArray = Encoding.UTF8.GetBytes(_incomingTrust.Ukprn);
                _session.Setup(s => s.TryGetValue("IncomingTrustId", out incomingTrustIdByteArray)).Returns(true);

                var outgoingAcademyIds = new List<string> {_academyOne.Ukprn, _academyTwo.Ukprn};
                var outgoingAcademyIdsByteArray = Encoding.UTF8.GetBytes(string.Join(",", outgoingAcademyIds));
                _session.Setup(s => s.TryGetValue("OutgoingAcademyIds", out outgoingAcademyIdsByteArray)).Returns(true);

                var createdProject = new Project
                {
                    Urn = "Created URN"
                };

                _projectsRepository.Setup(r => r.Create(It.IsAny<Project>()))
                    .ReturnsAsync(new RepositoryResult<Project> {Result = createdProject});
            }

            [Fact]
            public async void GivenSubmittingProject_FetchesProjectInformationFromSession()
            {
                await _subject.SubmitProject();

                var foundByteArray = It.IsAny<byte[]>();
                _session.Verify(s => s.TryGetValue("OutgoingTrustId", out foundByteArray), Times.Once);
                _session.Verify(s => s.TryGetValue("IncomingTrustId", out foundByteArray), Times.Once);
                _session.Verify(s => s.TryGetValue("OutgoingAcademyIds", out foundByteArray), Times.Once);
            }

            [Fact]
            public async void GivenSubmittingProjectWithAllValues_InsertsMappedProject()
            {
                await _subject.SubmitProject();

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
                        Result = new Project {Urn = "12345"}
                    });

                var response = await _subject.SubmitProject();

                var responseRedirect = Assert.IsType<RedirectToPageResult>(response);
                Assert.Equal($"/Projects/{nameof(Pages.Projects.Index)}", responseRedirect.PageName);
                Assert.Equal(createdProjectUrn, responseRedirect.RouteValues["urn"]);
            }
        }

        #region HelperMethods

        private static void AssertTrustsAreAssignedToTheView(IActionResult result, string trustId, string trustTwoId)
        {
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<TrustSearch>(viewResult.ViewData.Model);

            Assert.Equal(trustId, viewModel.Trusts[0].Ukprn);
            Assert.Equal(trustTwoId, viewModel.Trusts[1].Ukprn);
        }

        private static RedirectToActionResult AssertRedirectToAction(IActionResult response, string actionName)
        {
            var redirectResponse = Assert.IsType<RedirectToActionResult>(response);
            Assert.Equal(actionName, redirectResponse.ActionName);
            return redirectResponse;
        }
        
        private static RedirectToPageResult AssertRedirectToPage(IActionResult response, string pageName)
        {
            var redirectResponse = Assert.IsType<RedirectToPageResult>(response);
            Assert.Equal(pageName, redirectResponse.PageName);
            return redirectResponse;
        }

        #endregion
    }
}