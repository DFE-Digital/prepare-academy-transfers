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

        public class TrustNameTests : TransfersControllerTests
        {
            [Fact]
            public void GivenErrorMessageExists_SetErrorInViewData()
            {
                _subject.TempData["ErrorMessage"] = "This is an error message";
                _subject.TrustName();

                Assert.Equal(true, _subject.ViewData["Error.Exists"]);
                Assert.Equal("This is an error message", _subject.ViewData["Error.Message"]);
            }

            [Fact]
            public void GivenExistingQuery_SetQueryInViewData()
            {
                _subject.TrustName("Meow");

                Assert.Equal("Meow", _subject.ViewData["Query"]);
            }

            [Fact]
            public void GivenChangeLink_SetChangeLinkinViewData()
            {
                _subject.TrustName("Meow", true);

                Assert.Equal(true, _subject.ViewData["ChangeLink"]);
            }
        }

        public class TrustSearchTests : TransfersControllerTests
        {
            [Fact]
            public void GivenErrorMessageExists_SetErrorInViewData()
            {
                _trustsRepository.Setup(r => r.SearchTrusts("test", ""))
                    .ReturnsAsync(
                        new RepositoryResult<List<TrustSearchResult>> {Result = new List<TrustSearchResult>
                        {
                            new TrustSearchResult {Academies = new List<TrustSearchAcademy> { new TrustSearchAcademy()}}
                        }});
                
                _subject.TempData["ErrorMessage"] = "This is an error message";
                _subject.TrustSearch("test");

                Assert.Equal(true, _subject.ViewData["Error.Exists"]);
                Assert.Equal("This is an error message", _subject.ViewData["Error.Message"]);
            }
            
            [Fact]
            public async void GivenSearchingByEmptyString_RedirectToTrustNamePageWithAnError()
            {
                var response = await _subject.TrustSearch("");
                AssertRedirectToAction(response, "TrustName");
                Assert.Equal("Enter the outgoing trust name", _subject.TempData["ErrorMessage"]);
            }

            [Fact]
            public async void GivenSearchReturnsNoTrusts_RedirectToTrustNamePageWithAnError()
            {
                _trustsRepository.Setup(r => r.SearchTrusts("Meow", ""))
                    .ReturnsAsync(
                        new RepositoryResult<List<TrustSearchResult>> {Result = new List<TrustSearchResult>()});
                var response = await _subject.TrustSearch("Meow");
        
                var redirectResponse = AssertRedirectToAction(response, "TrustName");
                Assert.Equal("Meow", redirectResponse.RouteValues["query"]);
                Assert.Equal("We could not find any trusts matching your search criteria", _subject.TempData["ErrorMessage"]);
            }

            [Fact]
            public async void GivenSearchReturnsTrustWithNoAcademies_RedirectToTrustNamePageWithAnError()
            {
                _trustsRepository.Setup(r => r.SearchTrusts("Meow", ""))
                    .ReturnsAsync(
                        new RepositoryResult<List<TrustSearchResult>> {Result = new List<TrustSearchResult> { new TrustSearchResult
                        {
                            TrustName = "Meow",
                            Ukprn = "test",
                            Academies = new List<TrustSearchAcademy>()
                        } }});
                var response = await _subject.TrustSearch("Meow");

                var redirectResponse = AssertRedirectToAction(response, "TrustName");
                Assert.Equal("Meow", redirectResponse.RouteValues["query"]);
                Assert.Equal("We could not find any trusts matching your search criteria", _subject.TempData["ErrorMessage"]);
            }

            [Fact]
            public async void GivenSearchingByString_SetsResponsesOnTheView()
            {
                const string trustId = "1234";
                const string trustTwoId = "4321";

                _trustsRepository.Setup(r => r.SearchTrusts("Trust name", "")).ReturnsAsync(
                    new RepositoryResult<List<TrustSearchResult>>
                    {
                        Result = new List<TrustSearchResult>
                        {
                            new TrustSearchResult {Ukprn = trustId, Academies = new List<TrustSearchAcademy> { new TrustSearchAcademy()}},
                            new TrustSearchResult {Ukprn = trustTwoId, Academies = new List<TrustSearchAcademy> { new TrustSearchAcademy()}}
                        }
                    }
                );

                var result = await _subject.TrustSearch("Trust name");

                AssertTrustsAreAssignedToTheView(result, trustId, trustTwoId);
                _trustsRepository.Verify(r => r.SearchTrusts("Trust name", ""));
            }

            [Fact]
            public async void GivenSearchingByString_SetsQueryOnTheView()
            {
                const string trustId = "1234";
                const string trustTwoId = "4321";

                _trustsRepository.Setup(r => r.SearchTrusts("Trust name", "")).ReturnsAsync(
                    new RepositoryResult<List<TrustSearchResult>>
                    {
                        Result = new List<TrustSearchResult>
                        {
                            new TrustSearchResult {Ukprn = trustId, Academies = new List<TrustSearchAcademy> { new TrustSearchAcademy()}},
                            new TrustSearchResult {Ukprn = trustTwoId, Academies = new List<TrustSearchAcademy> { new TrustSearchAcademy()}}
                        }
                    }
                );

                await _subject.TrustSearch("Trust name");
                Assert.Equal("Trust name", _subject.ViewData["Query"]);
            }

            [Fact]
            public async void GivenChangeLink_SetChangeLinkinViewData()
            {
                const string trustId = "1234";
                const string trustTwoId = "4321";

                _trustsRepository.Setup(r => r.SearchTrusts("Trust name", "")).ReturnsAsync(
                    new RepositoryResult<List<TrustSearchResult>>
                    {
                        Result = new List<TrustSearchResult>
                        {
                            new TrustSearchResult {Ukprn = trustId, Academies = new List<TrustSearchAcademy>()},
                            new TrustSearchResult {Ukprn = trustTwoId, Academies = new List<TrustSearchAcademy>()}
                        }
                    }
                );

                await _subject.TrustSearch("Trust name", true);

                Assert.Equal(true, _subject.ViewData["ChangeLink"]);
            }
        }

        public class OutgoingTrustDetailsTests : TransfersControllerTests
        {
            private readonly string _trustId;
            private readonly Trust _foundTrust;

            public OutgoingTrustDetailsTests()
            {
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
            }

            [Fact]
            public async void GivenId_LookupTrustFromAPIAndAssignToView()
            {
                var response = await _subject.OutgoingTrustDetails(_trustId);

                _trustsRepository.Verify(r => r.GetByUkprn(_trustId), Times.Once);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<OutgoingTrustDetails>(viewResponse.Model);

                Assert.Equal(_foundTrust, viewModel.Trust);
            }

            [Fact]
            public async void GivenIdAndQuery_AssignQueryToTheView()
            {
                await _subject.OutgoingTrustDetails(_trustId, "Trust name");
                Assert.Equal("Trust name", _subject.ViewData["Query"]);
            }

            [Fact]
            public async void GivenChangeLink_AssignChangeLinkToTheView()
            {
                await _subject.OutgoingTrustDetails(_trustId, "Trust name", true);
                Assert.Equal(true, _subject.ViewData["ChangeLink"]);
            }
        }

        public class ConfirmOutgoingTrustTests : TransfersControllerTests
        {
            [Fact]
            public void GivenTrustId_StoresTheTrustInTheSessionAndRedirects()
            {
                const string trustId = "9a7be920-eaa0-e911-a83f-000d3a3852af";
                var response = _subject.ConfirmOutgoingTrust(trustId);

                _session.Verify(s => s.Set(
                    "OutgoingTrustId",
                    It.Is<byte[]>(input =>
                        Encoding.UTF8.GetString(input) == trustId
                    )));

                AssertRedirectToPage(response, "/Transfers/OutgoingTrustAcademies");
            }

            [Fact]
            public void GivenTrustId_ClearExistingInformationInTheSession()
            {
                var trustId = "9a7be920-eaa0-e911-a83f-000d3a3852af";
                _subject.ConfirmOutgoingTrust(trustId);

                _session.Verify(s => s.Remove("IncomingTrustId"));
                _session.Verify(s => s.Remove("OutgoingAcademyIds"));
            }
        }

        public class IncomingTrust : TransfersControllerTests
        {
            private readonly Academy _outgoingAcademy;

            public IncomingTrust()
            {
                _outgoingAcademy = new Academy
                {
                    Ukprn = "9a7be920-eaa0-e911-a83f-000d3a3852af",
                    Name = "test name"
                };

                var outgoingAcademyIdByteArray = Encoding.UTF8.GetBytes(_outgoingAcademy.Ukprn);
                _session.Setup(s => s.TryGetValue("OutgoingAcademyIds", out outgoingAcademyIdByteArray)).Returns(true);

                _academiesRepository.Setup(r => r.GetAcademyByUkprn(_outgoingAcademy.Ukprn)).ReturnsAsync(
                    new RepositoryResult<Academy>
                    {
                        Result = _outgoingAcademy
                    });
            }

            [Fact]
            public void GivenErrorMessageExists_SetErrorInViewData()
            {
                _subject.TempData["ErrorMessage"] = "This is an error message";
                _subject.IncomingTrust();

                Assert.Equal(true, _subject.ViewData["Error.Exists"]);
                Assert.Equal("This is an error message", _subject.ViewData["Error.Message"]);
            }

            [Fact]
            public void GivenExistingQuery_SetQueryInViewData()
            {
                _subject.IncomingTrust("Meow");

                Assert.Equal("Meow", _subject.ViewData["Query"]);
            }

            [Fact]
            public void GivenChangeLink_SetChangeLinkinViewData()
            {
                _subject.IncomingTrust("Meow", true);

                Assert.Equal(true, _subject.ViewData["ChangeLink"]);
            }
        }

        public class SearchIncomingTrusts : TransfersControllerTests
        {
            [Fact]
            public async void GivenSearchingByEmptyString_RedirectToTrustNamePageWithAnError()
            {
                var response = await _subject.SearchIncomingTrust("");
                AssertRedirectToAction(response, "IncomingTrust");
                Assert.Equal("Enter the incoming trust name", _subject.TempData["ErrorMessage"]);
            }

            [Fact]
            public async void GivenNoSearchResultsForString_RedirectToIncomingTrustPageWithError()
            {
                _trustsRepository.Setup(r => r.SearchTrusts("Trust name", null)).ReturnsAsync(
                    new RepositoryResult<List<TrustSearchResult>>
                    {
                        Result = new List<TrustSearchResult>()
                    });

                var response = await _subject.SearchIncomingTrust("Trust name");
                var redirectResponse = AssertRedirectToAction(response, "IncomingTrust");
                Assert.Equal("Trust name", redirectResponse.RouteValues["query"]);
                Assert.Equal("We could not find any trusts matching your search criteria", _subject.TempData["ErrorMessage"]);
            }
            
            [Fact]
            public async void GivenErrorMessageExists_SetErrorInViewData()
            {
                _trustsRepository.Setup(r => r.SearchTrusts("test", null))
                    .ReturnsAsync(
                        new RepositoryResult<List<TrustSearchResult>> {Result = new List<TrustSearchResult> { new TrustSearchResult() }});
                
                _subject.TempData["ErrorMessage"] = "This is an error message";
                await _subject.SearchIncomingTrust("test");

                Assert.Equal(true, _subject.ViewData["Error.Exists"]);
                Assert.Equal("This is an error message", _subject.ViewData["Error.Message"]);
            }
            
            [Fact]
            public async void GivenQuery_PutsQueryIntoTheViewData()
            {
                _trustsRepository.Setup(r => r.SearchTrusts("test", null))
                    .ReturnsAsync(
                        new RepositoryResult<List<TrustSearchResult>> {Result = new List<TrustSearchResult> { new TrustSearchResult() }});
                
                var response = await _subject.SearchIncomingTrust("test");
                var viewResponse = Assert.IsType<ViewResult>(response);

                Assert.Equal("test", viewResponse.ViewData["Query"]);
            }

            [Fact]
            public async void GivenChangeLink_PutsChangeLinkIntoTheViewData()
            {
                _trustsRepository.Setup(r => r.SearchTrusts("test", null))
                    .ReturnsAsync(
                        new RepositoryResult<List<TrustSearchResult>> {Result = new List<TrustSearchResult> { new TrustSearchResult() }});
                
                var response = await _subject.SearchIncomingTrust("test", true);
                var viewResponse = Assert.IsType<ViewResult>(response);

                Assert.Equal(true, viewResponse.ViewData["ChangeLink"]);
            }

            [Fact]
            public async void GivenSearchingByString_SetsMappedResponsesOnTheView()
            {
                var trustId = "1234";
                var trustTwoId = "4321";

                _trustsRepository.Setup(r => r.SearchTrusts(It.IsAny<string>(), null)).ReturnsAsync(
                    new RepositoryResult<List<TrustSearchResult>>
                    {
                        Result = new List<TrustSearchResult>
                        {
                            new TrustSearchResult {Ukprn = trustId},
                            new TrustSearchResult {Ukprn = trustTwoId}
                        }
                    });

                var result = await _subject.SearchIncomingTrust("Trust name");

                AssertTrustsAreAssignedToTheView(result, trustId, trustTwoId);
                _trustsRepository.Verify(r => r.SearchTrusts("Trust name", null));
            }

            [Fact]
            public async void GivenChangeLink_SetsChangeLinkOnTheView()
            {
                const string trustId = "1234";
                const string trustTwoId = "4321";

                _trustsRepository.Setup(r => r.SearchTrusts("Trust name", null)).ReturnsAsync(
                    new RepositoryResult<List<TrustSearchResult>>
                    {
                        Result = new List<TrustSearchResult>
                        {
                            new TrustSearchResult {Ukprn = trustId},
                            new TrustSearchResult {Ukprn = trustTwoId}
                        }
                    }
                );

                await _subject.SearchIncomingTrust("Trust name", true);

                Assert.Equal(true, _subject.ViewData["ChangeLink"]);
            }
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