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
                AssertRedirectToAction(response, "TrustName");
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

                var result = await _subject.TrustSearch("Trust name");

                AssertTrustsAreAssignedToTheView(result, trustId, trustTwoId);
                AssertTrustRepositoryIsCalledCorrectly();
            }
        }

        public class OutgoingTrustDetailsTests : TransfersControllerTests
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

                var response = await _subject.OutgoingTrustDetails(trustId);

                _trustRepository.Verify(r => r.GetTrustById(trustId), Times.Once);

                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<OutgoingTrustDetails>(viewResponse.Model);

                Assert.Equal(foundTrust, viewModel.Trust);
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
        }

        public class OutgoingTrustAcademiesTests : TransfersControllerTests
        {
            [Fact]
            public async void GivenTrustGuidInSession_FetchesTheAcademiesForThatTrust()
            {
                const string academyName = "Academy 001";
                const string academyNameTwo = "Academy 002";
                var trustId = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3852af");
                var trustIdByteArray = Encoding.UTF8.GetBytes(trustId.ToString());

                _session.Setup(s => s.TryGetValue("OutgoingTrustId", out trustIdByteArray)).Returns(true);

                _academiesRepository.Setup(r => r.GetAcademiesByTrustId(trustId)).ReturnsAsync(
                    new RepositoryResult<List<GetAcademiesModel>>
                    {
                        Result = new List<GetAcademiesModel>
                        {
                            new GetAcademiesModel {AcademyName = academyName},
                            new GetAcademiesModel {AcademyName = academyNameTwo},
                        }
                    }
                );

                var response = await _subject.OutgoingTrustAcademies();
                var viewResponse = Assert.IsType<ViewResult>(response);
                var viewModel = Assert.IsType<OutgoingTrustAcademies>(viewResponse.Model);

                Assert.Equal("Academy 001", viewModel.Academies[0].AcademyName);
                Assert.Equal("Academy 002", viewModel.Academies[1].AcademyName);
            }
        }

        public class SubmitOutgoingTrustAcademiesTests : TransfersControllerTests
        {
            [Fact]
            public void GivenAcademyGuids_StoresTheThemInTheSessionAndRedirects()
            {
                var idOne = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3852af");
                var idTwo = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3854af");
                var idThree = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3854af");
                var academyIds = new List<Guid> {idOne, idTwo, idThree}.ToArray();
                var academyIdString = string.Join(",", academyIds.Select(id => id.ToString()).ToList());

                var result = _subject.SubmitOutgoingTrustAcademies(academyIds);

                var resultRedirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("IncomingTrustIdentified", resultRedirect.ActionName);

                _session.Verify(s => s.Set(
                    "OutgoingAcademyIds",
                    It.Is<byte[]>(input =>
                        Encoding.UTF8.GetString(input) == academyIdString
                    )));
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
                AssertRedirectToAction(response, "IncomingTrust");
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

        private static void AssertRedirectToAction(IActionResult response, string actionName)
        {
            var redirectResponse = Assert.IsType<RedirectToActionResult>(response);
            Assert.Equal(actionName, redirectResponse.ActionName);
        }
        
        #endregion
    }
}