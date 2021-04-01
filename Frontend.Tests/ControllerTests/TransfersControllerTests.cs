using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using API.Models.Upstream.Request;
using API.Models.Upstream.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Frontend.Controllers;
using Frontend.Views.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Session;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests
{
    public class TransfersControllerTests
    {
        private readonly Mock<ITrustsRepository> _trustRepository;
        private readonly Mock<IAcademiesRepository> _academiesRepository;
        private readonly Mock<IProjectsRepository> _projectsRepository;

        private readonly TransfersController _subject;
        private readonly Mock<ISession> _session;

        public TransfersControllerTests()
        {
            _trustRepository = new Mock<ITrustsRepository>();
            _academiesRepository = new Mock<IAcademiesRepository>();
            _projectsRepository = new Mock<IProjectsRepository>();
            _session = new Mock<ISession>();

            var tempDataProvider = new Mock<ITempDataProvider>();
            var httpContext = new DefaultHttpContext();
            var sessionFeature = new SessionFeature {Session = _session.Object};
            httpContext.Features.Set<ISessionFeature>(sessionFeature);

            var tempDataDictionaryFactory =
                new TempDataDictionaryFactory(tempDataProvider.Object);
            var tempData = tempDataDictionaryFactory.GetTempData(httpContext);

            _subject = new TransfersController(
                _trustRepository.Object,
                _academiesRepository.Object,
                _projectsRepository.Object
            ) {TempData = tempData, ControllerContext = {HttpContext = httpContext}};
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
            public async void GivenSearchingByEmptyString_RedirectToTrustNamePageWithAnError()
            {
                var response = await _subject.TrustSearch("");
                AssertRedirectToAction(response, "TrustName");
                Assert.Equal("Please enter a search term", _subject.TempData["ErrorMessage"]);
            }

            [Fact]
            public async void GivenSearchReturnsNoTrusts_RedirectToTrustNamePageWithAnError()
            {
                _trustRepository.Setup(r => r.SearchTrusts("Meow"))
                    .ReturnsAsync(new RepositoryResult<List<GetTrustsModel>> {Result = new List<GetTrustsModel>()});
                var response = await _subject.TrustSearch("Meow");

                var redirectResponse = AssertRedirectToAction(response, "TrustName");
                Assert.Equal("Meow", redirectResponse.RouteValues["query"]);
                Assert.Equal("No results found", _subject.TempData["ErrorMessage"]);
            }

            [Fact]
            public async void GivenRepositoryReturnsAnError_RedirectToTrustNamePageWithAnError()
            {
                _trustRepository.Setup(r => r.SearchTrusts("Trust name")).ReturnsAsync(
                    new RepositoryResult<List<GetTrustsModel>>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            ErrorMessage = "TRAMS error message",
                            StatusCode = HttpStatusCode.InternalServerError
                        }
                    }
                );

                var response = await _subject.TrustSearch("Trust name");
                var redirectResponse = AssertRedirectToAction(response, "TrustName");
                Assert.Equal("Trust name", redirectResponse.RouteValues["query"]);
                Assert.Equal("TRAMS error message", _subject.TempData["ErrorMessage"]);
            }

            [Fact]
            public async void GivenSearchingByString_SetsResponsesOnTheView()
            {
                var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");
                var trustTwoId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672faf");

                _trustRepository.Setup(r => r.SearchTrusts("Trust name")).ReturnsAsync(
                    new RepositoryResult<List<GetTrustsModel>>
                    {
                        Result = new List<GetTrustsModel>
                        {
                            new GetTrustsModel {Id = trustId},
                            new GetTrustsModel {Id = trustTwoId}
                        }
                    }
                );

                var result = await _subject.TrustSearch("Trust name");

                AssertTrustsAreAssignedToTheView(result, trustId, trustTwoId);
                AssertTrustRepositoryIsCalledCorrectly();
            }
            
            [Fact]
            public async void GivenSearchingByString_SetsQueryOnTheView()
            {
                var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");
                var trustTwoId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672faf");

                _trustRepository.Setup(r => r.SearchTrusts("Trust name")).ReturnsAsync(
                    new RepositoryResult<List<GetTrustsModel>>
                    {
                        Result = new List<GetTrustsModel>
                        {
                            new GetTrustsModel {Id = trustId},
                            new GetTrustsModel {Id = trustTwoId}
                        }
                    }
                );

                await _subject.TrustSearch("Trust name");
                Assert.Equal("Trust name", _subject.ViewData["Query"]);
            }
            
            [Fact]
            public async void GivenChangeLink_SetChangeLinkinViewData()
            {
                var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");
                var trustTwoId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672faf");

                _trustRepository.Setup(r => r.SearchTrusts("Trust name")).ReturnsAsync(
                    new RepositoryResult<List<GetTrustsModel>>
                    {
                        Result = new List<GetTrustsModel>
                        {
                            new GetTrustsModel {Id = trustId},
                            new GetTrustsModel {Id = trustTwoId}
                        }
                    }
                );

                await _subject.TrustSearch("Trust name", true);
                
                Assert.Equal(true, _subject.ViewData["ChangeLink"]);
            }
        }

        public class OutgoingTrustDetailsTests : TransfersControllerTests
        {
            private readonly Guid _trustId;
            private readonly GetTrustsModel _foundTrust;

            public OutgoingTrustDetailsTests()
            {
                _trustId = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3855a3");
                _foundTrust = new GetTrustsModel
                {
                    Id = _trustId,
                    TrustName = "Example trust",
                    CompaniesHouseNumber = "12345678",
                    EstablishmentType = "Multi Academy Trust",
                    TrustReferenceNumber = "TR12345",
                    Address = "One example street\n\rExample City \n\rExample other line"
                };

                _trustRepository.Setup(r => r.GetTrustById(_trustId)).ReturnsAsync(
                    new RepositoryResult<GetTrustsModel>
                    {
                        Result = _foundTrust
                    });
            }

            [Fact]
            public async void GivenGuid_LookupTrustFromAPIAndAssignToView()
            {
                var response = await _subject.OutgoingTrustDetails(_trustId);

                _trustRepository.Verify(r => r.GetTrustById(_trustId), Times.Once);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<OutgoingTrustDetails>(viewResponse.Model);

                Assert.Equal(_foundTrust, viewModel.Trust);
            }
            
            [Fact]
            public async void GivenGuidAndQuery_AssignQueryToTheView()
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
            public void GivenTrustGuid_StoresTheTrustInTheSessionAndRedirects()
            {
                var trustId = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3852af");
                var response = _subject.ConfirmOutgoingTrust(trustId);

                _session.Verify(s => s.Set(
                    "OutgoingTrustId",
                    It.Is<byte[]>(input =>
                        Encoding.UTF8.GetString(input) == trustId.ToString()
                    )));

                AssertRedirectToAction(response, "OutgoingTrustAcademies");
            }
            
            [Fact]
            public void GivenTrustGuid_ClearExistingInformationInTheSession()
            {
                var trustId = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3852af");
                _subject.ConfirmOutgoingTrust(trustId);
                
                _session.Verify(s => s.Remove("IncomingTrustId"));
                _session.Verify(s => s.Remove("OutgoingAcademyIds"));
            }
        }

        public class OutgoingTrustAcademiesTests : TransfersControllerTests
        {
            private const string AcademyName = "Academy 001";
            private const string AcademyNameTwo = "Academy 002";

            public OutgoingTrustAcademiesTests()
            {
                var trustId = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3852af");
                var trustIdByteArray = Encoding.UTF8.GetBytes(trustId.ToString());

                _session.Setup(s => s.TryGetValue("OutgoingTrustId", out trustIdByteArray)).Returns(true);

                _academiesRepository.Setup(r => r.GetAcademiesByTrustId(trustId)).ReturnsAsync(
                    new RepositoryResult<List<GetAcademiesModel>>
                    {
                        Result = new List<GetAcademiesModel>
                        {
                            new GetAcademiesModel {AcademyName = AcademyName},
                            new GetAcademiesModel {AcademyName = AcademyNameTwo},
                        }
                    }
                );
            }

            [Fact]
            public async void GivenTrustGuidInSession_FetchesTheAcademiesForThatTrust()
            {
                var response = await _subject.OutgoingTrustAcademies();
                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<OutgoingTrustAcademies>(viewResponse.Model);

                Assert.Equal(AcademyName, viewModel.Academies[0].AcademyName);
                Assert.Equal(AcademyNameTwo, viewModel.Academies[1].AcademyName);
            }
            
            [Fact]
            public async void GivenNoErrorMessage_SetsErrorExistsToFalse()
            {
                var response = await _subject.OutgoingTrustAcademies();
                var viewResponse = Assert.IsType<ViewResult>(response);
                Assert.Equal(false, viewResponse.ViewData["Error.Exists"]);
            }

            [Fact]
            public async void GivenErrorMessage_PutsTheErrorIntoTheViewData()
            {
                _subject.TempData["ErrorMessage"] = "Error message";
                
                var response = await _subject.OutgoingTrustAcademies();
                var viewResponse = Assert.IsType<ViewResult>(response);
                
                Assert.Equal(true, viewResponse.ViewData["Error.Exists"]);
                Assert.Equal("Error message", viewResponse.ViewData["Error.Message"]);
            }

            [Fact]
            public async void GivenChangeLink_PutsChangeLinkIntoTheViewData()
            {
                var response = await _subject.OutgoingTrustAcademies(true);
                var viewResponse = Assert.IsType<ViewResult>(response);
                
                Assert.Equal(true, viewResponse.ViewData["ChangeLink"]);
            }
            
            [Fact]
            public async void GivenOutgoingAcademiesExistInSession_PutsOutgoingAcademyIdIntoTheViewData()
            {
                var outgoingAcademyId = Guid.NewGuid();
                var outgoingAcademyIds = new List<Guid> {outgoingAcademyId};
                var outgoingAcademyIdsByteArray = Encoding.UTF8.GetBytes(string.Join(",", outgoingAcademyIds));
                _session.Setup(s => s.TryGetValue("OutgoingAcademyIds", out outgoingAcademyIdsByteArray)).Returns(true);
                
                var response = await _subject.OutgoingTrustAcademies();
                var viewResponse = Assert.IsType<ViewResult>(response);
                
                Assert.Equal(outgoingAcademyId.ToString(), viewResponse.ViewData["OutgoingAcademyId"]);
            }
        }

        public class SubmitOutgoingTrustAcademiesTests : TransfersControllerTests
        {
            [Fact]
            public void GivenAcademyGuid_StoresItInTheSessionAndRedirects()
            {
                var idOne = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3852af");
                var academyIdString = string.Join(",", new[] {idOne}.Select(id => id.ToString()).ToList());

                var result = _subject.SubmitOutgoingTrustAcademies(idOne);

                var resultRedirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("IncomingTrustIdentified", resultRedirect.ActionName);

                _session.Verify(s => s.Set(
                    "OutgoingAcademyIds",
                    It.Is<byte[]>(input =>
                        Encoding.UTF8.GetString(input) == academyIdString
                    )));
            }

            [Fact]
            public void GivenNoAcademyGuid_RedirectBackToOutgoingTrustAcademiesWithError()
            {
                var result = _subject.SubmitOutgoingTrustAcademies(null);

                var resultRedirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("OutgoingTrustAcademies", resultRedirect.ActionName);
                Assert.Equal("Please select an academy", _subject.TempData["ErrorMessage"]);
            }
            
            [Fact]
            public void GivenChangeLink_RedirectBackToOutgoingTrustAcademiesWithError()
            {
                var idOne = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3852af");
                var result = _subject.SubmitOutgoingTrustAcademies(idOne, true);

                var resultRedirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("CheckYourAnswers", resultRedirect.ActionName);
            }
        }

        public class SubmitIncomingTrustIdentifiedTests : TransfersControllerTests
        {
            [Fact]
            private void GivenYes_RedirectToIncomingTrustPage()
            {
                var response = _subject.SubmitIncomingTrustIdentified("yes");
                var redirectResponse = Assert.IsType<RedirectToActionResult>(response);
                Assert.Equal("IncomingTrust", redirectResponse.ActionName);
            }

            [Fact]
            private void GivenNo_RedirectToCheckYourAnswers()
            {
                var response = _subject.SubmitIncomingTrustIdentified("no");
                var redirectResponse = Assert.IsType<RedirectToActionResult>(response);
                Assert.Equal("CheckYourAnswers", redirectResponse.ActionName);
            }
        }

        public class IncomingTrust : TransfersControllerTests
        {
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
                Assert.Equal("Please enter a search term", _subject.TempData["ErrorMessage"]);
            }

            [Fact]
            public async void GivenNoSearchResultsForString_RedirectToIncomingTrustPageWithError()
            {
                _trustRepository.Setup(r => r.SearchTrusts("Trust name")).ReturnsAsync(
                    new RepositoryResult<List<GetTrustsModel>>()
                    {
                        Result = new List<GetTrustsModel>()
                    });

                var response = await _subject.SearchIncomingTrust("Trust name");
                var redirectResponse = AssertRedirectToAction(response, "IncomingTrust");
                Assert.Equal("Trust name", redirectResponse.RouteValues["query"]);
                Assert.Equal("No search results", _subject.TempData["ErrorMessage"]);
            }

            [Fact]
            public async void GivenRepositoryReturnsAnError_RedirectToTrustNamePageWithAnError()
            {
                _trustRepository.Setup(r => r.SearchTrusts("Trust name")).ReturnsAsync(
                    new RepositoryResult<List<GetTrustsModel>>
                    {
                        Error = new RepositoryResultBase.RepositoryError
                        {
                            ErrorMessage = "TRAMS error message",
                            StatusCode = HttpStatusCode.InternalServerError
                        }
                    }
                );

                var response = await _subject.SearchIncomingTrust("Trust name");
                var redirectResponse = AssertRedirectToAction(response, "IncomingTrust");
                Assert.Equal("Trust name", redirectResponse.RouteValues["query"]);
                Assert.Equal("TRAMS error message", _subject.TempData["ErrorMessage"]);
            }

            [Fact]
            public async void GivenSearchingByString_SetsMappedResponsesOnTheView()
            {
                var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");
                var trustTwoId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672faf");

                _trustRepository.Setup(r => r.SearchTrusts("Trust name")).ReturnsAsync(
                    new RepositoryResult<List<GetTrustsModel>>
                    {
                        Result = new List<GetTrustsModel>
                        {
                            new GetTrustsModel {Id = trustId},
                            new GetTrustsModel {Id = trustTwoId}
                        }
                    }
                );

                var result = await _subject.SearchIncomingTrust("Trust name");

                AssertTrustsAreAssignedToTheView(result, trustId, trustTwoId);
                AssertTrustRepositoryIsCalledCorrectly();
            }
            
            [Fact]
            public async void GivenChangeLink_SetsChangeLinkOnTheView()
            {
                var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");
                var trustTwoId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672faf");

                _trustRepository.Setup(r => r.SearchTrusts("Trust name")).ReturnsAsync(
                    new RepositoryResult<List<GetTrustsModel>>
                    {
                        Result = new List<GetTrustsModel>
                        {
                            new GetTrustsModel {Id = trustId},
                            new GetTrustsModel {Id = trustTwoId}
                        }
                    }
                );

                await _subject.SearchIncomingTrust("Trust name", true);

                Assert.Equal(true, _subject.ViewData["ChangeLink"]);
            }
        }

        public class IncomingTrustDetailsTests : TransfersControllerTests
        {
            [Fact]
            public async void GivenGuid_LookupTrustFromAPIAndAssignToView()
            {
                var trustId = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3855a3");
                var foundTrust = new GetTrustsModel
                {
                    Id = trustId,
                    TrustName = "Example trust",
                    CompaniesHouseNumber = "12345678",
                    EstablishmentType = "Multi Academy Trust",
                    TrustReferenceNumber = "TR12345",
                    Address = "One example street\n\rExample City \n\rExample other line"
                };

                _trustRepository.Setup(r => r.GetTrustById(trustId)).ReturnsAsync(
                    new RepositoryResult<GetTrustsModel>
                    {
                        Result = foundTrust
                    });

                var response = await _subject.IncomingTrustDetails(trustId);

                _trustRepository.Verify(r => r.GetTrustById(trustId), Times.Once);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<OutgoingTrustDetails>(viewResponse.Model);

                Assert.Equal(foundTrust, viewModel.Trust);
            }
            
            [Fact]
            public async void GivenChangeLink_SetChangeLinkInView()
            {
                var trustId = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3855a3");
                var foundTrust = new GetTrustsModel
                {
                    Id = trustId,
                    TrustName = "Example trust",
                    CompaniesHouseNumber = "12345678",
                    EstablishmentType = "Multi Academy Trust",
                    TrustReferenceNumber = "TR12345",
                    Address = "One example street\n\rExample City \n\rExample other line"
                };

                _trustRepository.Setup(r => r.GetTrustById(trustId)).ReturnsAsync(
                    new RepositoryResult<GetTrustsModel>
                    {
                        Result = foundTrust
                    });

                await _subject.IncomingTrustDetails(trustId, "", true);
                Assert.Equal(true, _subject.ViewData["ChangeLink"]);
            }
        }

        public class ConfirmIncomingTrustTests : TransfersControllerTests
        {
            [Fact]
            public void GivenTrustGuid_StoresTheTrustInTheSessionAndRedirects()
            {
                var trustId = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3852af");
                var response = _subject.ConfirmIncomingTrust(trustId);

                _session.Verify(s => s.Set(
                    "IncomingTrustId",
                    It.Is<byte[]>(input =>
                        Encoding.UTF8.GetString(input) == trustId.ToString()
                    )));
                AssertRedirectToAction(response, "CheckYourAnswers");
            }
        }

        public class CheckYourAnswersTests : TransfersControllerTests
        {
            private readonly GetTrustsModel _outgoingTrust = new GetTrustsModel
                {Id = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3852af")};

            private readonly GetTrustsModel _incomingTrust = new GetTrustsModel
                {Id = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a385210")};

            private readonly GetAcademiesModel _academyOne = new GetAcademiesModel
                {Id = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a385211")};

            private readonly GetAcademiesModel _academyTwo = new GetAcademiesModel
                {Id = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a385212")};

            private readonly GetAcademiesModel _academyThree = new GetAcademiesModel
                {Id = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a385213")};

            public CheckYourAnswersTests()
            {
                var outgoingTrustIdByteArray = Encoding.UTF8.GetBytes(_outgoingTrust.Id.ToString());
                _session.Setup(s => s.TryGetValue("OutgoingTrustId", out outgoingTrustIdByteArray)).Returns(true);

                var incomingTrustIdByteArray = Encoding.UTF8.GetBytes(_incomingTrust.Id.ToString());
                _session.Setup(s => s.TryGetValue("IncomingTrustId", out incomingTrustIdByteArray)).Returns(true);

                var outgoingAcademyIds = new List<Guid> {_academyOne.Id, _academyTwo.Id};
                var outgoingAcademyIdsByteArray = Encoding.UTF8.GetBytes(string.Join(",", outgoingAcademyIds));
                _session.Setup(s => s.TryGetValue("OutgoingAcademyIds", out outgoingAcademyIdsByteArray)).Returns(true);
                _trustRepository.Setup(r => r.GetTrustById(_outgoingTrust.Id)).ReturnsAsync(
                    new RepositoryResult<GetTrustsModel>
                    {
                        Result = _outgoingTrust
                    });

                _trustRepository.Setup(r => r.GetTrustById(_incomingTrust.Id)).ReturnsAsync(
                    new RepositoryResult<GetTrustsModel>
                    {
                        Result = _incomingTrust
                    });

                _academiesRepository.Setup(r => r.GetAcademiesByTrustId(_outgoingTrust.Id)).ReturnsAsync(
                    new RepositoryResult<List<GetAcademiesModel>>
                    {
                        Result = new List<GetAcademiesModel> {_academyOne, _academyTwo, _academyThree}
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
                _trustRepository.Verify(r => r.GetTrustById(_outgoingTrust.Id), Times.Once);
                _trustRepository.Verify(r => r.GetTrustById(_incomingTrust.Id), Times.Once);
                _academiesRepository.Verify(r => r.GetAcademiesByTrustId(_outgoingTrust.Id), Times.Once);
            }

            [Fact]
            public async void GivenAllInformationInSession_CreatesTheViewModelCorrectly()
            {
                var response = await _subject.CheckYourAnswers();

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<CheckYourAnswers>(viewResponse.Model);

                Assert.Equal(_outgoingTrust.Id, viewModel.OutgoingTrust.Id);
                Assert.Equal(_incomingTrust.Id, viewModel.IncomingTrust.Id);

                var expectedAcademyIds = new List<Guid> {_academyOne.Id, _academyTwo.Id};
                var viewAcademyIds = viewModel.OutgoingAcademies.Select(academy => academy.Id);

                Assert.Equal(expectedAcademyIds, viewAcademyIds);
            }
        }

        public class SubmitProjectTests : TransfersControllerTests
        {
            private readonly GetTrustsModel _outgoingTrust = new GetTrustsModel
                {Id = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3852af")};

            private readonly GetTrustsModel _incomingTrust = new GetTrustsModel
                {Id = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a385210")};

            private readonly GetAcademiesModel _academyOne = new GetAcademiesModel
                {Id = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a385211")};

            private readonly GetAcademiesModel _academyTwo = new GetAcademiesModel
                {Id = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a385212")};

            private readonly RepositoryResult<Guid?> _postProjectResponse = new RepositoryResult<Guid?>
                {Result = Guid.NewGuid()};


            public SubmitProjectTests()
            {
                var outgoingTrustIdByteArray = Encoding.UTF8.GetBytes(_outgoingTrust.Id.ToString());
                _session.Setup(s => s.TryGetValue("OutgoingTrustId", out outgoingTrustIdByteArray)).Returns(true);

                var incomingTrustIdByteArray = Encoding.UTF8.GetBytes(_incomingTrust.Id.ToString());
                _session.Setup(s => s.TryGetValue("IncomingTrustId", out incomingTrustIdByteArray)).Returns(true);

                var outgoingAcademyIds = new List<Guid> {_academyOne.Id, _academyTwo.Id};
                var outgoingAcademyIdsByteArray = Encoding.UTF8.GetBytes(string.Join(",", outgoingAcademyIds));
                _session.Setup(s => s.TryGetValue("OutgoingAcademyIds", out outgoingAcademyIdsByteArray)).Returns(true);

                _projectsRepository.Setup(r => r.InsertProject(It.IsAny<PostProjectsRequestModel>()))
                    .ReturnsAsync(_postProjectResponse);
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

                var expectedProjectRequestModel = new PostProjectsRequestModel
                {
                    ProjectAcademies = new List<PostProjectsAcademiesModel>
                    {
                        new PostProjectsAcademiesModel
                        {
                            AcademyId = _academyOne.Id,
                            Trusts = new List<PostProjectsAcademiesTrustsModel>
                                {new PostProjectsAcademiesTrustsModel {TrustId = _outgoingTrust.Id},}
                        },
                        new PostProjectsAcademiesModel
                        {
                            AcademyId = _academyTwo.Id,
                            Trusts = new List<PostProjectsAcademiesTrustsModel>
                                {new PostProjectsAcademiesTrustsModel {TrustId = _outgoingTrust.Id},}
                        }
                    },
                    ProjectTrusts = new List<PostProjectsTrustsModel>
                    {
                        new PostProjectsTrustsModel {TrustId = _incomingTrust.Id}
                    }
                };

                _projectsRepository.Verify(
                    r => r.InsertProject(It.Is<PostProjectsRequestModel>(input =>
                        input.ProjectAcademies[0].AcademyId ==
                        expectedProjectRequestModel.ProjectAcademies[0].AcademyId)),
                    Times.Once);
            }

            [Fact]
            public async void GivenProjectIsInserted_RedirectsToProjectFeaturesWithCreatedId()
            {
                var createdProjectGuid = Guid.NewGuid();
                _projectsRepository.Setup(r => r.InsertProject(It.IsAny<PostProjectsRequestModel>()))
                    .ReturnsAsync(new RepositoryResult<Guid?> {Result = createdProjectGuid});

                var response = await _subject.SubmitProject();

                var responseRedirect = Assert.IsType<RedirectToActionResult>(response);
                Assert.Equal("ProjectFeatures", responseRedirect.ActionName);
                Assert.Equal(createdProjectGuid, responseRedirect.RouteValues["projectId"]);
            }
        }

        #region HelperMethods

        private void AssertTrustRepositoryIsCalledCorrectly()
        {
            _trustRepository.Verify(r => r.SearchTrusts("Trust name"));
        }

        private static void AssertTrustsAreAssignedToTheView(IActionResult result, Guid trustId, Guid trustTwoId)
        {
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<TrustSearch>(viewResult.ViewData.Model);

            Assert.Equal(trustId, viewModel.Trusts[0].Id);
            Assert.Equal(trustTwoId, viewModel.Trusts[1].Id);
        }

        private static RedirectToActionResult AssertRedirectToAction(IActionResult response, string actionName)
        {
            var redirectResponse = Assert.IsType<RedirectToActionResult>(response);
            Assert.Equal(actionName, redirectResponse.ActionName);
            return redirectResponse;
        }

        #endregion
    }
}