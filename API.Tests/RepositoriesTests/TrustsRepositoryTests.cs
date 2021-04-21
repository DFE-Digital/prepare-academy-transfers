using API.HttpHelpers;
using API.Models.Downstream.D365;
using API.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using API.Mapping;
using API.Models.Upstream.Response;
using Xunit;

namespace API.Tests.RepositoriesTests
{
    public class TrustsRepositoryTests
    {
        private readonly TrustsDynamicsRepository _subject;
        private readonly Mock<IOdataUrlBuilder<GetTrustsD365Model>> _mockUrlBuilder;
        private readonly Mock<IAuthenticatedHttpClient> _mockClient;
        private readonly Mock<IMapper<GetTrustsD365Model, GetTrustsModel>> _getTrustMapper;

        public TrustsRepositoryTests()
        {
            _mockClient = new Mock<IAuthenticatedHttpClient>();
            _mockUrlBuilder = new Mock<IOdataUrlBuilder<GetTrustsD365Model>>();
            _getTrustMapper = new Mock<IMapper<GetTrustsD365Model, GetTrustsModel>>();
            var mockedLogger = new Mock<ILogger<TrustsDynamicsRepository>>();
            var mockSanitizer = new Mock<IODataSanitizer>();

            _subject = new TrustsDynamicsRepository(_mockClient.Object, _mockUrlBuilder.Object, mockSanitizer.Object,
                mockedLogger.Object, _getTrustMapper.Object);
        }

        [Fact]
        public async void GetTrustById_FailedHttpClientCall_ReturnsFailedResult()
        {
            //Arrange
            var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1");

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Error - Bad Request", Encoding.UTF8, "application/json")
            };

            _mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            _mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            //Execute
            var result = await _subject.GetTrustById(trustId);

            //Assert
            _mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", trustId), Times.Once);
            _mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

            Assert.False(result.IsValid);
            Assert.Null(result.Result);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
            Assert.Equal("Error - Bad Request", result.Error.ErrorMessage);
        }

        [Fact]
        public async void GetTrustById_OkResult_IncorrectEstablishmentType_ReturnsCorrectResult()
        {
            //Arrange
            var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1");

            var returnedEntity = new GetTrustsD365Model
            {
                EstablishmentTypeId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1")
            };
            var json = JsonConvert.SerializeObject(returnedEntity);
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            _mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);
            _mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            //Execute
            var result = await _subject.GetTrustById(trustId);

            //Assert
            _mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", trustId), Times.Once);
            _mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

            Assert.True(result.IsValid);
            Assert.Null(result.Result);
        }

        [Fact]
        public async void GetTrustById_OkResult_CorrectEstablishmentType_ReturnsCorrectResult()
        {
            //Arrange
            var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1");

            var returnedEntity = new GetTrustsD365Model
            {
                EstablishmentTypeId = Guid.Parse("81014326-5D51-E911-A82E-000D3A385A17"),
                TrustName = "Some Trust",
                TrustReferenceNumber = "TR100001"
            };
            var json = JsonConvert.SerializeObject(returnedEntity);
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            _mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);
            _mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            _getTrustMapper.Setup(m => m.Map(It.IsAny<GetTrustsD365Model>())).Returns(new GetTrustsModel
            {
                TrustName = "Mapped Trust",
                TrustReferenceNumber = "Mapped TR100001"
            });

            //Execute
            var result = await _subject.GetTrustById(trustId);

            //Assert
            _mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", trustId), Times.Once);
            _mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

            Assert.True(result.IsValid);
            Assert.NotNull(result.Result);
            Assert.Equal("Mapped Trust", result.Result.TrustName);
            Assert.Equal("Mapped TR100001", result.Result.TrustReferenceNumber);
        }

        [Fact]
        public async void SearchTrusts_FailedHttpClientCall_ReturnsFailedResult()
        {
            //Arrange
            const string query = "searchQuery";

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Error - Bad Request", Encoding.UTF8, "application/json")
            };

            _mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);
            _mockUrlBuilder.Setup(m => m.BuildFilterUrl("accounts", It.IsAny<List<string>>()))
                .Returns("buildFilterUrl");

            //Execute
            var result = await _subject.SearchTrusts(query);

            //Assert
            _mockUrlBuilder.Verify(m => m.BuildFilterUrl("accounts", It.IsAny<List<string>>()), Times.Once);
            _mockClient.Verify(m => m.GetAsync("buildFilterUrl"), Times.Once);

            Assert.False(result.IsValid);
            Assert.Null(result.Result);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
            Assert.Equal("Error - Bad Request", result.Error.ErrorMessage);
        }
    }
}