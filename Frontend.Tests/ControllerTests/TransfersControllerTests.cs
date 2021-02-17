using System;
using System.Collections.Generic;
using System.Net;
using API.Mapping;
using API.Models.Downstream.D365;
using API.Models.Upstream.Response;
using API.Repositories;
using Frontend.Controllers;
using Frontend.Views.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace Frontend.Tests.ControllerTests
{
    public class TransfersControllerTests
    {
        private readonly Mock<ITrustsRepository> _trustRepository;
        private readonly Mock<IMapper<GetTrustsD365Model, GetTrustsModel>> _getTrustMapper;
        private readonly TransfersController _subject;

        public TransfersControllerTests()
        {
            _trustRepository = new Mock<ITrustsRepository>();
            _getTrustMapper = new Mock<IMapper<GetTrustsD365Model, GetTrustsModel>>();

            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempDataDictionaryFactory =
                new TempDataDictionaryFactory(tempDataProvider.Object);
            var tempData = tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());

            _subject = new TransfersController(
                _trustRepository.Object,
                _getTrustMapper.Object) {TempData = tempData};
        }

        #region TrustName

        [Fact]
        public void GivenErrorMessageExists_SetErrorInViewData()
        {
            _subject.TempData["ErrorMessage"] = "This is an error message";
            _subject.TrustName();

            Assert.Equal(true, _subject.ViewData["Error.Exists"]);
            Assert.Equal("This is an error message", _subject.ViewData["Error.Message"]);
        }

        #endregion

        #region TrustSearch

        [Fact]
        public async void GivenSearchingByEmptyString_RedirectToTrustNamePageWithAnError()
        {
            var response = await _subject.TrustSearch("");
            AssertRedirectToTrustName(response);
            Assert.Equal("Please enter a search term", _subject.TempData["ErrorMessage"]);
        }

        [Fact]
        public async void GivenRepositoryReturnsAnError_RedirectToTrustNamePageWithAnError()
        {
            _trustRepository.Setup(r => r.SearchTrusts("Trust name")).ReturnsAsync(
                new RepositoryResult<List<GetTrustsD365Model>>
                {
                    Error = new RepositoryResultBase.RepositoryError
                    {
                        ErrorMessage = "TRAMS error message",
                        StatusCode = HttpStatusCode.InternalServerError
                    }
                }
            );

            var response = await _subject.TrustSearch("Trust name");
            AssertRedirectToTrustName(response);
            Assert.Equal("TRAMS error message", _subject.TempData["ErrorMessage"]);
        }

        [Fact]
        public async void GivenSearchingByString_SetsMappedResponsesOnTheView()
        {
            var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9");
            var trustTwoId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672faf");

            _trustRepository.Setup(r => r.SearchTrusts("Trust name")).ReturnsAsync(
                new RepositoryResult<List<GetTrustsD365Model>>
                {
                    Result = new List<GetTrustsD365Model>
                    {
                        new GetTrustsD365Model {Id = trustId},
                        new GetTrustsD365Model {Id = trustTwoId}
                    }
                }
            );

            _getTrustMapper.Setup(m => m.Map(It.IsAny<GetTrustsD365Model>()))
                .Returns<GetTrustsD365Model>((input) => new GetTrustsModel {Id = input.Id});

            var result = await _subject.TrustSearch("Trust name");

            AssertTrustsAreAssignedToTheView(result, trustId, trustTwoId);
            AssertTrustRepositoryIsCalledCorrectly();
            AssertTrustsAreMappedCorrectly(trustId, trustTwoId);
        }

        private void AssertTrustsAreMappedCorrectly(Guid trustId, Guid trustTwoId)
        {
            _getTrustMapper.Verify(m => m.Map(It.Is<GetTrustsD365Model>(model => model.Id == trustId)), Times.Once);
            _getTrustMapper.Verify(m => m.Map(It.Is<GetTrustsD365Model>(model => model.Id == trustTwoId)), Times.Once);
        }

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

        private static void AssertRedirectToTrustName(IActionResult response)
        {
            var redirectResponse = Assert.IsType<RedirectToActionResult>(response);
            Assert.Equal("TrustName", redirectResponse.ActionName);
        }

        #endregion

        #region OutgoingTrustDetails

        [Fact]
        public async void GivenGuid_LookupTrustFromAPIAndAssignToView()
        {
            var trustId = Guid.Parse("9a7be920-eaa0-e911-a83f-000d3a3855a3");
            var mappedTrust = new GetTrustsModel()
            {
                Id = trustId,
                TrustName = "Example trust",
                CompaniesHouseNumber = "12345678",
                EstablishmentType = "Multi Academy Trust",
                TrustReferenceNumber = "TR12345",
                Address = "One example street\n\rExample City \n\rExample other line"
            };

            _trustRepository.Setup(r => r.GetTrustById(trustId)).ReturnsAsync(new RepositoryResult<GetTrustsD365Model>()
            {
                Result = new GetTrustsD365Model()
                {
                    Id = trustId,
                    TrustName = "Example trust",
                    CompaniesHouseNumber = "12345678",
                    EstablishmentType = "Multi Academy Trust",
                    TrustReferenceNumber = "TR12345",
                    Address = "One example street\n\rExample City \n\rExample other line"
                }
            });

            _getTrustMapper.Setup(m => m.Map(It.IsAny<GetTrustsD365Model>()))
                .Returns<GetTrustsD365Model>(input => mappedTrust);

            var response = await _subject.OutgoingTrustDetails(trustId);

            _trustRepository.Verify(r => r.GetTrustById(trustId), Times.Once);
            _getTrustMapper.Verify(m => m.Map(It.Is<GetTrustsD365Model>(model => model.Id == trustId)));

            var viewResponse = Assert.IsType<ViewResult>(response);
            var viewModel = Assert.IsType<OutgoingTrustDetails>(viewResponse.Model);

            Assert.Equal(mappedTrust, viewModel.Trust);
        }

        #endregion
    }
}