using API.HttpHelpers;
using API.Models.D365;
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
    public class AcademiesRepositoryTests
    {
        #region GetAcademyById Tests

        [Fact]
        public async void GetAcademyById_FailedHttpClientCall_ReturnsFailedResult()
        {
            //Arrange
            var academyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1");

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Error - Bad Request", Encoding.UTF8, "application/json")
            };
            var mockClient = new Mock<IAuthenticatedHttpClient>();
            mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            var mockLogger = new Mock<ILogger<AcademiesRepository>>();

            var mockUrlBuilder = new Mock<IOdataUrlBuilder<GetAcademiesD365Model>>();
            mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                          .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            var academiesRepository = new AcademiesRepository(mockClient.Object, mockUrlBuilder.Object, mockLogger.Object);

            //Execute
            var result = await academiesRepository.GetAcademyById(academyId);

            //Assert
            mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", academyId), Times.Once);
            mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

            Assert.False(result.IsValid);
            Assert.Null(result.Result);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
            Assert.Equal("Error - Bad Request", result.Error.ErrorMessage);
        }

        [Fact]
        public async void GetAcademyById_OkResult_Empty_ReturnsCorrectResult()
        {
            //Arrange
            var academyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1");

            var returnedEntities = new GetAcademiesD365Model();
            var json = JsonConvert.SerializeObject(returnedEntities);
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var mockClient = new Mock<IAuthenticatedHttpClient>();
            mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            var mockedLogger = new Mock<ILogger<AcademiesRepository>>();

            var mockUrlBuilder = new Mock<IOdataUrlBuilder<GetAcademiesD365Model>>();
            mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                          .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            var academiesRepository = new AcademiesRepository(mockClient.Object, mockUrlBuilder.Object, mockedLogger.Object);

            //Execute
            var result = await academiesRepository.GetAcademyById(academyId);

            //Assert
            mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", academyId), Times.Once);
            mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

            Assert.True(result.IsValid);
            Assert.Null(result.Result);
        }

        [Fact]
        public async void GetAcademyById_Ok_NoParentId_ReturnsCorrectResult()
        {
            //Arrange
            var academyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1");

            var returnedEntities = new GetAcademiesD365Model
            {
                AcademyName = "Some Academy",
                Id = Guid.NewGuid(),
                Urn = "URN"
            };

            var json = JsonConvert.SerializeObject(returnedEntities);
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var mockClient = new Mock<IAuthenticatedHttpClient>();
            mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            var mockedLogger = new Mock<ILogger<AcademiesRepository>>();

            var mockUrlBuilder = new Mock<IOdataUrlBuilder<GetAcademiesD365Model>>();
            mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                          .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            var academiesRepository = new AcademiesRepository(mockClient.Object, mockUrlBuilder.Object, mockedLogger.Object);

            //Execute
            var result = await academiesRepository.GetAcademyById(academyId);

            //Assert
            mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", academyId), Times.Once);
            mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

            Assert.True(result.IsValid);
            Assert.Null(result.Result);
        }

        [Fact]
        public async void GetAcademyById_Ok_ParentIdSet_ReturnsCorrectResult()
        {
            var academyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1");
            var trustId = Guid.Parse("b16e9020-9123-4420-8055-851d1b672fb1");

            //Arrange
            var returnedEntity = new GetAcademiesD365Model
            {
                AcademyName = "Some Academy",
                Id = academyId,
                Urn = "URN",
                ParentTrustId = trustId
            };

            var json = JsonConvert.SerializeObject(returnedEntity);
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var mockClient = new Mock<IAuthenticatedHttpClient>();
            mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            var mockedLogger = new Mock<ILogger<AcademiesRepository>>();

            var mockUrlBuilder = new Mock<IOdataUrlBuilder<GetAcademiesD365Model>>();
            mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                          .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            var academiesRepository = new AcademiesRepository(mockClient.Object, mockUrlBuilder.Object, mockedLogger.Object);

            //Execute
            var result = await academiesRepository.GetAcademyById(academyId);

            //Assert
            mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", academyId), Times.Once);
            mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

            Assert.True(result.IsValid);
            Assert.NotNull(result.Result);
            Assert.Equal(academyId, result.Result.Id);
            Assert.Equal(trustId, result.Result.ParentTrustId);
            Assert.Equal("Some Academy", result.Result.AcademyName);
            Assert.Equal("URN", result.Result.Urn);
        }

        #endregion

        #region GetAcademiesByTrustId Tests

        [Fact]
        public async void GetAcademiesByTrustId_FailedHttpClientCall_ReturnsFailedResult()
        {
            //Arrange
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Error - Bad Request", Encoding.UTF8, "application/json")
            };
            var mockClient = new Mock<IAuthenticatedHttpClient>();
            mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            var mockedLogger = new Mock<ILogger<AcademiesRepository>>();

            var mockUrlBuilder = new Mock<IOdataUrlBuilder<GetAcademiesD365Model>>();
            mockUrlBuilder.Setup(m => m.GetPropertyAnnotation(nameof(GetAcademiesD365Model.ParentTrustId)))
                          .Returns("mockedTrustIdField");

            mockUrlBuilder.Setup(m => m.BuildFilterUrl("accounts", It.IsAny<List<string>>()))
                          .Returns("buildFilterUrl");

            var academiesRepository = new AcademiesRepository(mockClient.Object, mockUrlBuilder.Object, mockedLogger.Object);

            var academyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1");

            //Execute
            var result = await academiesRepository.GetAcademiesByTrustId(academyId);

            //Assert
            var expectedFilters = new List<string>
            {
                "(mockedTrustIdField eq a16e9020-9123-4420-8055-851d1b672fb1)"
            };
            mockUrlBuilder.Verify(m => m.BuildFilterUrl("accounts", expectedFilters), Times.Once);
            mockClient.Verify(m => m.GetAsync("buildFilterUrl"), Times.Once);

            Assert.False(result.IsValid);
            Assert.Null(result.Result);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
            Assert.Equal("Error - Bad Request", result.Error.ErrorMessage);
        }

        [Fact]
        public async void GetAcademiesByTrustId_OkResult_NoItems_ReturnsCorrectResult()
        {
            //Arrange
            var returnedEntities = new ResultSet<GetAcademiesD365Model>
            {
                Items = new List<GetAcademiesD365Model>()
            };

            var json = JsonConvert.SerializeObject(returnedEntities);
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var mockClient = new Mock<IAuthenticatedHttpClient>();
            mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            var mockedLogger = new Mock<ILogger<AcademiesRepository>>();

            var mockUrlBuilder = new Mock<IOdataUrlBuilder<GetAcademiesD365Model>>();
            mockUrlBuilder.Setup(m => m.GetPropertyAnnotation(nameof(GetAcademiesD365Model.ParentTrustId)))
                          .Returns("_parentaccountid_value");

            mockUrlBuilder.Setup(m => m.BuildFilterUrl("accounts", It.IsAny<List<string>>()))
                          .Returns("buildFilterUrl");

            var academiesRepository = new AcademiesRepository(mockClient.Object, mockUrlBuilder.Object, mockedLogger.Object);

            var academyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1");

            //Execute
            var result = await academiesRepository.GetAcademiesByTrustId(academyId);

            //Assert
            var expectedFilters = new List<string>
            {
                "(_parentaccountid_value eq a16e9020-9123-4420-8055-851d1b672fb1)"
            };
            mockUrlBuilder.Verify(m => m.BuildFilterUrl("accounts", expectedFilters), Times.Once);
            mockClient.Verify(m => m.GetAsync("buildFilterUrl"), Times.Once);

            Assert.True(result.IsValid);
            Assert.NotNull(result.Result);
            Assert.Empty(result.Result);
        }

        [Fact]
        public async void GetAcademiesByTrustId_OkResult_WithItems_ReturnsCorrectResult()
        {
            //Arrange
            var returnedEntities = new ResultSet<GetAcademiesD365Model>
            {
                Items = new List<GetAcademiesD365Model>
                {
                    new GetAcademiesD365Model(),
                    new GetAcademiesD365Model()
                }
            };

            var json = JsonConvert.SerializeObject(returnedEntities);
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var mockClient = new Mock<IAuthenticatedHttpClient>();
            mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            var mockedLogger = new Mock<ILogger<AcademiesRepository>>();

            var mockUrlBuilder = new Mock<IOdataUrlBuilder<GetAcademiesD365Model>>();
            mockUrlBuilder.Setup(m => m.GetPropertyAnnotation(nameof(GetAcademiesD365Model.ParentTrustId)))
                          .Returns("_parentaccountid_value");

            mockUrlBuilder.Setup(m => m.BuildFilterUrl("accounts", It.IsAny<List<string>>()))
                          .Returns("buildFilterUrl");

            var academiesRepository = new AcademiesRepository(mockClient.Object, mockUrlBuilder.Object, mockedLogger.Object);

            var academyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1");

            //Execute
            var result = await academiesRepository.GetAcademiesByTrustId(academyId);

            //Assert
            var expectedFilters = new List<string>
            {
                "(_parentaccountid_value eq a16e9020-9123-4420-8055-851d1b672fb1)"
            };
            mockUrlBuilder.Verify(m => m.BuildFilterUrl("accounts", expectedFilters), Times.Once);
            mockClient.Verify(m => m.GetAsync("buildFilterUrl"), Times.Once);

            Assert.True(result.IsValid);
            Assert.NotNull(result.Result);
            Assert.Equal(2, result.Result.Count);
        }

        #endregion
    }
}