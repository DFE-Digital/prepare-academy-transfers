using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Dfe.Academies.Contracts.V4.Trusts;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Tests.Dfe.PrepareTransfers.Helpers;
using Dfe.PrepareTransfers.Data.TRAMS.Tests.TestFixtures;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Dfe.PrepareTransfers.Data.TRAMS.Tests
{
    public class TramsTrustsRepositoryTests
    {
        private readonly Mock<ITramsHttpClient> _client;
        private readonly Mock<IMapper<TrustDto, Trust>> _trustMapper;
        private readonly TramsTrustsRepository _subject;

        public TramsTrustsRepositoryTests()
        {
            _client = new Mock<ITramsHttpClient>();
            _trustMapper = new Mock<IMapper<TrustDto, Trust>>();
            _subject = new TramsTrustsRepository(_client.Object, _trustMapper.Object);
        }

        public class SearchTrustsTests : TramsTrustsRepositoryTests
        {
            [Fact]
            public async void GivenSearchTerm_QueriesTheApiWithTheSearchTerm()
            {
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(TrustSearchResults.GetTrustSearchResults()))
                });

                await _subject.SearchTrusts("Cats");

                _client.Verify(c => c.GetAsync("trusts?groupName=Cats&ukprn=Cats&companiesHouseNumber=Cats"),
                    Times.Once);
            }

            [Fact]
            public async void GivenASingleSearchResult_ReturnsTheMappedResult()
            {
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(TrustSearchResults.GetTrustSearchResults()))
                });

                _trustMapper.Setup(m => m.Map(It.IsAny<TrustDto>()))
                    .Returns<Trust>(
                    result =>
                        new Trust
                        {
                            Ukprn = $"Mapped {result.Ukprn}",
                            Name = $"Mapped {result.Name}"
                        } 
                        );

                var response = await _subject.SearchTrusts();

                Assert.Equal("Mapped 1", response[0].Ukprn);
            }

            [Fact]
            public async void GivenMultipleSearchResults_ReturnsTheMappedResult()
            {
                HttpClientTestHelpers.SetupGet(_client, TrustSearchResults.GetTrustSearchResults(2));

                _trustMapper.Setup(m => m.Map(It.IsAny<TrustDto>()))
                    .Returns<Trust>(result =>
                        new Trust
                        {
                            Ukprn = $"Mapped {result.Ukprn}",
                            Name = $"Mapped {result.Name}"
                        });

                var response = await _subject.SearchTrusts();

                Assert.Equal("Mapped 1", response[0].Ukprn);
                Assert.Equal("Mapped 2", response[1].Ukprn);
            }
            
            [Fact]
            public async void GivenMultipleSearchResultsWithTrustToExclude_ReturnsTheMappedResultWithTheTrustExcluded()
            {
                HttpClientTestHelpers.SetupGet(_client, TrustSearchResults.GetTrustSearchResults(2));

                _trustMapper.Setup(m => m.Map(It.IsAny<TrustDto>()))
               .Returns<Trust>(result =>
                   new Trust
                   {
                       Ukprn = $"Mapped {result.Ukprn}",
                       Name = $"Mapped {result.Name}"
                   });

                var response = await _subject.SearchTrusts("query", "2");

                Assert.Equal("Mapped 1", response[0].Ukprn);
                Assert.Single(response);
            }
            
            [Theory]
            [InlineData(HttpStatusCode.NotFound)]
            [InlineData(HttpStatusCode.InternalServerError)]
            public async void GivenApiReturnsError_ThrowsApiError(HttpStatusCode httpStatusCode)
            {
                HttpClientTestHelpers.SetupGet<TramsTrustSearchResult>(_client, null, httpStatusCode);
                
                await Assert.ThrowsAsync<TramsApiException>(() => _subject.SearchTrusts("12345"));
            }
        }

        public class GetTrustByUkprnTests : TramsTrustsRepositoryTests
        {
            private readonly TrustDto _foundTrust;

            public GetTrustByUkprnTests()
            {
                _foundTrust = Trusts.GetSingleTrust();
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(_foundTrust))
                });

                _trustMapper.Setup(m => m.Map(It.IsAny<TrustDto>())).Returns<TrustDto>((input) => new Trust()
                {
                    Name = $"Mapped {input.Name}",
                    Ukprn = $"Mapped {input.Ukprn}"
                });
            }

            [Fact]
            public async void GivenUkprn_SearchesForTrustOnTheApi()
            {
                await _subject.GetByUkprn("12345");

                _client.Verify(c => c.GetAsync("trust/12345"), Times.Once);
            }

            [Fact]
            public async void GivenResultFromApi_MapsResult()
            {
                await _subject.GetByUkprn("12345");

                _trustMapper.Verify(m =>
                    m.Map(It.Is<TrustDto>(trust => trust.Ukprn == _foundTrust.Ukprn)), Times.Once);
            }

            [Fact]
            public async void GivenResultFromApi_ReturnsMappedResult()
            {
                var response = await _subject.GetByUkprn("12345");

                Assert.Equal($"Mapped {_foundTrust.Ukprn}", response.Ukprn);
            }
            
            [Theory]
            [InlineData(HttpStatusCode.NotFound)]
            [InlineData(HttpStatusCode.InternalServerError)]
            public async void GivenApiReturnsError_ThrowsApiError(HttpStatusCode httpStatusCode)
            {
                HttpClientTestHelpers.SetupGet<TrustDto>(_client, null, httpStatusCode);
                
                await Assert.ThrowsAsync<TramsApiException>(() => _subject.GetByUkprn("12345"));
            }
        }
    }
}