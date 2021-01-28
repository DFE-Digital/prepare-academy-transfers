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
using Xunit;

namespace API.Tests.RepositoriesTests
{
    public class TrustsRepositoryTests
    {
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
            var mockClient = new Mock<IAuthenticatedHttpClient>();
            mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            var mockedLogger = new Mock<ILogger<TrustsRepository>>();

            var mockUrlBuilder = new Mock<IOdataUrlBuilder<GetTrustsD365Model>>();
            mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                          .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            var mockSanitizer = new Mock<IODataSanitizer>();

            var academiesRepository = new TrustsRepository(mockClient.Object, mockUrlBuilder.Object, mockSanitizer.Object, mockedLogger.Object);

            //Execute
            var result = await academiesRepository.GetTrustById(trustId);

            //Assert
            mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", trustId), Times.Once);
            mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

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

            var mockClient = new Mock<IAuthenticatedHttpClient>();
            mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            var mockedLogger = new Mock<ILogger<TrustsRepository>>();

            var mockUrlBuilder = new Mock<IOdataUrlBuilder<GetTrustsD365Model>>();
            mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                          .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            var mockSanitizer = new Mock<IODataSanitizer>();

            var academiesRepository = new TrustsRepository(mockClient.Object, mockUrlBuilder.Object, mockSanitizer.Object, mockedLogger.Object);

            //Execute
            var result = await academiesRepository.GetTrustById(trustId);

            //Assert
            mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", trustId), Times.Once);
            mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

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

            var mockClient = new Mock<IAuthenticatedHttpClient>();
            mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            var mockedLogger = new Mock<ILogger<TrustsRepository>>();

            var mockUrlBuilder = new Mock<IOdataUrlBuilder<GetTrustsD365Model>>();
            mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                          .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            var mockSanitizer = new Mock<IODataSanitizer>();

            var academiesRepository = new TrustsRepository(mockClient.Object, mockUrlBuilder.Object, mockSanitizer.Object, mockedLogger.Object);

            //Execute
            var result = await academiesRepository.GetTrustById(trustId);

            //Assert
            mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", trustId), Times.Once);
            mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

            Assert.True(result.IsValid);
            Assert.NotNull(result.Result);
            Assert.Equal("Some Trust", result.Result.TrustName);
            Assert.Equal("TR100001", result.Result.TrustReferenceNumber);
        }

        [Fact]
        public async void SearchTrusts_FailedHttpClientCall_ReturnsFailedResult()
        {
            //Arrange
            var query = "searchQuery";

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Error - Bad Request", Encoding.UTF8, "application/json")
            };
            var mockClient = new Mock<IAuthenticatedHttpClient>();
            mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            var mockedLogger = new Mock<ILogger<TrustsRepository>>();
            
            Mock<IOdataUrlBuilder<GetTrustsD365Model>> mockUrlBuilder = new Mock<IOdataUrlBuilder<GetTrustsD365Model>>();
            mockUrlBuilder.Setup(m => m.BuildFilterUrl("accounts", It.IsAny<List<string>>()))
                          .Returns("buildFilterUrl");

            var mockSanitizer = new Mock<IODataSanitizer>();

            var academiesRepository = new TrustsRepository(mockClient.Object, mockUrlBuilder.Object, mockSanitizer.Object, mockedLogger.Object);

            //Execute
            var result = await academiesRepository.SearchTrusts(query);

            //Assert
            mockUrlBuilder.Verify(m => m.BuildFilterUrl("accounts", It.IsAny<List<string>>()), Times.Once);
            mockClient.Verify(m => m.GetAsync("buildFilterUrl"), Times.Once);

            Assert.False(result.IsValid);
            Assert.Null(result.Result);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
            Assert.Equal("Error - Bad Request", result.Error.ErrorMessage);
        }
    }
}