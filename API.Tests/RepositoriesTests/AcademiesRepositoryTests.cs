using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using API.HttpHelpers;
using API.Mapping;
using API.Models.Downstream.D365;
using API.Models.Upstream.Response;
using API.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace API.Tests.RepositoriesTests
{
    public class AcademiesRepositoryTests
    {
        private readonly Mock<IAuthenticatedHttpClient> _mockClient;
        private readonly Mock<IOdataUrlBuilder<GetAcademiesD365Model>> _mockUrlBuilder;
        private readonly AcademiesRepository _subject;
        private readonly Mock<IMapper<GetAcademiesD365Model, GetAcademiesModel>> _365AcademiesMapper;

        public AcademiesRepositoryTests()
        {
            _mockClient = new Mock<IAuthenticatedHttpClient>();
            _mockUrlBuilder = new Mock<IOdataUrlBuilder<GetAcademiesD365Model>>();
            _365AcademiesMapper = new Mock<IMapper<GetAcademiesD365Model, GetAcademiesModel>>();
            var mockedLogger = new Mock<ILogger<AcademiesRepository>>();
            _subject = new AcademiesRepository(_mockClient.Object, _mockUrlBuilder.Object, mockedLogger.Object,
                _365AcademiesMapper.Object);
        }

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

            _mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            _mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            //Execute
            var result = await _subject.GetAcademyById(academyId);

            //Assert
            _mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", academyId), Times.Once);
            _mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

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

            var returnedEntities = new GetAcademiesD365Model {Id = academyId};

            var json = JsonConvert.SerializeObject(returnedEntities);
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            _mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            _mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            //Execute
            var result = await _subject.GetAcademyById(academyId);

            //Assert
            _mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", academyId), Times.Once);
            _mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

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

            _mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            _mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            //Execute
            var result = await _subject.GetAcademyById(academyId);

            //Assert
            _mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", academyId), Times.Once);
            _mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

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

            var mappedEntity = new GetAcademiesModel
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
            _mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            _mockUrlBuilder.Setup(m => m.BuildRetrieveOneUrl("accounts", It.IsAny<Guid>()))
                .Returns("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)");

            _365AcademiesMapper.Setup(m => m.Map(It.Is<GetAcademiesD365Model>(o => o.Id == returnedEntity.Id)))
                .Returns(mappedEntity);

            var result = await _subject.GetAcademyById(academyId);

            //Assert
            _mockUrlBuilder.Verify(m => m.BuildRetrieveOneUrl("accounts", academyId), Times.Once);
            _mockClient.Verify(m => m.GetAsync("/accounts(a16e9020-9123-4420-8055-851d1b672fb1)"), Times.Once);

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

            _mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            _mockUrlBuilder.Setup(m => m.GetPropertyAnnotation(nameof(GetAcademiesD365Model.ParentTrustId)))
                .Returns("mockedTrustIdField");

            _mockUrlBuilder.Setup(m => m.BuildFilterUrl("accounts", It.IsAny<List<string>>()))
                .Returns("buildFilterUrl");

            var academyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1");

            //Execute
            var result = await _subject.GetAcademiesByTrustId(academyId);

            //Assert
            var expectedFilters = new List<string>
            {
                "(mockedTrustIdField eq a16e9020-9123-4420-8055-851d1b672fb1)"
            };
            _mockUrlBuilder.Verify(m => m.BuildFilterUrl("accounts", expectedFilters), Times.Once);
            _mockClient.Verify(m => m.GetAsync("buildFilterUrl"), Times.Once);

            Assert.False(result.IsValid);
            Assert.Null(result.Result);
            Assert.Equal(HttpStatusCode.BadRequest, result.Error.StatusCode);
            Assert.Equal("Error - Bad Request", result.Error.ErrorMessage);
        }

        [Fact]
        public async void GetAcademiesByTrustId_OkResult_NoItems_ReturnsCorrectResult()
        {
            //Arrange
            var academyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1");

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

            _mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            _mockUrlBuilder.Setup(m => m.GetPropertyAnnotation(nameof(GetAcademiesD365Model.ParentTrustId)))
                .Returns("_parentaccountid_value");

            _mockUrlBuilder.Setup(m => m.BuildFilterUrl("accounts", It.IsAny<List<string>>()))
                .Returns("buildFilterUrl");


            //Execute
            var result = await _subject.GetAcademiesByTrustId(academyId);

            //Assert
            var expectedFilters = new List<string>
            {
                "(_parentaccountid_value eq a16e9020-9123-4420-8055-851d1b672fb1)"
            };
            _mockUrlBuilder.Verify(m => m.BuildFilterUrl("accounts", expectedFilters), Times.Once);
            _mockClient.Verify(m => m.GetAsync("buildFilterUrl"), Times.Once);

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
                    new GetAcademiesD365Model {AcademyName = "Academy 001"},
                    new GetAcademiesD365Model {AcademyName = "Academy 002"}
                }
            };

            var json = JsonConvert.SerializeObject(returnedEntities);
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            _mockClient.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(httpResponse);

            _mockUrlBuilder.Setup(m => m.GetPropertyAnnotation(nameof(GetAcademiesD365Model.ParentTrustId)))
                .Returns("_parentaccountid_value");

            _mockUrlBuilder.Setup(m => m.BuildFilterUrl("accounts", It.IsAny<List<string>>()))
                .Returns("buildFilterUrl");

            _365AcademiesMapper.Setup(m => m.Map(It.IsAny<GetAcademiesD365Model>())).Returns<GetAcademiesD365Model>(
                input => new GetAcademiesModel {AcademyName = $"Mapped {input.AcademyName}"});

            var trustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1");

            //Execute
            var result = await _subject.GetAcademiesByTrustId(trustId);

            //Assert
            var expectedFilters = new List<string>
            {
                "(_parentaccountid_value eq a16e9020-9123-4420-8055-851d1b672fb1)"
            };
            _mockUrlBuilder.Verify(m => m.BuildFilterUrl("accounts", expectedFilters), Times.Once);
            _mockClient.Verify(m => m.GetAsync("buildFilterUrl"), Times.Once);
            _365AcademiesMapper.Verify(m => m.Map(It.IsAny<GetAcademiesD365Model>()), Times.Exactly(2));

            Assert.True(result.IsValid);
            Assert.Equal("Mapped Academy 001", result.Result[0].AcademyName);
            Assert.Equal("Mapped Academy 002", result.Result[1].AcademyName);
            Assert.Equal(2, result.Result.Count);
        }

        #endregion
    }
}