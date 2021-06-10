using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Data.Models;
using Data.TRAMS.Models;
using Data.TRAMS.Tests.TestFixtures;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Data.TRAMS.Tests
{
    public class TramsTrustsRepositoryTests
    {
        private readonly Mock<ITramsHttpClient> _client;
        private readonly Mock<IMapper<TramsTrustSearchResult, TrustSearchResult>> _trustSearchResultsMapper;
        private readonly Mock<IMapper<TramsTrust, Trust>> _trustMapper;
        private readonly TramsTrustsRepository _subject;

        public TramsTrustsRepositoryTests()
        {
            _client = new Mock<ITramsHttpClient>();
            _trustSearchResultsMapper = new Mock<IMapper<TramsTrustSearchResult, TrustSearchResult>>();
            _trustMapper = new Mock<IMapper<TramsTrust, Trust>>();
            _subject = new TramsTrustsRepository(_client.Object, _trustSearchResultsMapper.Object, _trustMapper.Object);
        }

        #region API Interim

        public class SearchTrustsTests : TramsTrustsRepositoryTests
        {
            [Fact]
            public async void GivenMatchingSearchTerm_ReturnsTrustSearchResult()
            {
                _client.Setup(c => c.GetAsync("trust/1234")).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new TramsTrust()))
                });

                var foundTrust = new Trust()
                {
                    Ukprn = "1234",
                    Name = "TrustName",
                    CompaniesHouseNumber = "4321",
                    Academies = new List<Academy>
                    {
                        new Academy {Name = "Academy 1", Ukprn = "1", Urn = "2"},
                        new Academy {Name = "Academy 2", Ukprn = "3", Urn = "4"}
                    }
                };

                _trustMapper.Setup(m => m.Map(It.IsAny<TramsTrust>())).Returns(foundTrust);

                var result = await _subject.SearchTrusts("1234");
                var trustSearchResults = result.Result;
                var trustResult = trustSearchResults[0];

                Assert.Single(trustSearchResults);
                Assert.Equal(foundTrust.Ukprn, trustResult.Ukprn);
                Assert.Equal(foundTrust.Name, trustResult.TrustName);
                Assert.Equal(foundTrust.CompaniesHouseNumber, trustResult.CompaniesHouseNumber);
                Assert.Equal(foundTrust.Academies[0].Name, trustResult.Academies[0].Name);
                Assert.Equal(foundTrust.Academies[0].Urn, trustResult.Academies[0].Urn);
                Assert.Equal(foundTrust.Academies[0].Ukprn, trustResult.Academies[0].Ukprn);
                Assert.Equal(foundTrust.Academies[1].Name, trustResult.Academies[1].Name);
                Assert.Equal(foundTrust.Academies[1].Urn, trustResult.Academies[1].Urn);
                Assert.Equal(foundTrust.Academies[1].Ukprn, trustResult.Academies[1].Ukprn);
            }

            [Fact]
            public async void GivenNoMatch_ReturnEmptyTrustResults()
            {
                _client.Setup(c => c.GetAsync("trust/1234")).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(""),
                    StatusCode = HttpStatusCode.NotFound
                });

                var result = await _subject.SearchTrusts("1234");
                var trustSearchResults = result.Result;
                Assert.Empty(trustSearchResults);
            }
        }

        #endregion

        public class SearchTrustsOriginalTests : TramsTrustsRepositoryTests
        {
            [Fact]
            public async void GivenSearchTerm_QueriesTheApiWithTheSearchTerm()
            {
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(TrustSearchResults.GetTrustSearchResults()))
                });

                await _subject.SearchTrustsOriginal("Cats");

                _client.Verify(c => c.GetAsync("trusts?group_name=Cats&urn=Cats&companies_house_number=Cats"),
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

                _trustSearchResultsMapper.Setup(m => m.Map(It.IsAny<TramsTrustSearchResult>()))
                    .Returns<TramsTrustSearchResult>(result =>
                        new TrustSearchResult
                        {
                            Ukprn = $"Mapped {result.Urn}",
                            TrustName = $"Mapped {result.GroupName}"
                        });

                var response = await _subject.SearchTrustsOriginal();

                Assert.Equal("Mapped 1", response.Result[0].Ukprn);
            }

            [Fact]
            public async void GivenMultipleSearchResults_ReturnsTheMappedResult()
            {
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(TrustSearchResults.GetTrustSearchResults(2)))
                });

                _trustSearchResultsMapper.Setup(m => m.Map(It.IsAny<TramsTrustSearchResult>()))
                    .Returns<TramsTrustSearchResult>(result =>
                        new TrustSearchResult
                        {
                            Ukprn = $"Mapped {result.Urn}",
                            TrustName = $"Mapped {result.GroupName}"
                        });

                var response = await _subject.SearchTrustsOriginal();

                Assert.Equal("Mapped 1", response.Result[0].Ukprn);
                Assert.Equal("Mapped 2", response.Result[1].Ukprn);
            }
        }

        public class GetTrustByUkprnTests : TramsTrustsRepositoryTests
        {
            private readonly TramsTrust _foundTrust;

            public GetTrustByUkprnTests()
            {
                _foundTrust = Trusts.GetSingleTrust();
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(_foundTrust))
                });

                _trustMapper.Setup(m => m.Map(It.IsAny<TramsTrust>())).Returns<TramsTrust>((input) => new Trust()
                {
                    Name = $"Mapped {input.GiasData.GroupName}",
                    Ukprn = $"Mapped {input.GiasData.Ukprn}"
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
                    m.Map(It.Is<TramsTrust>(trust => trust.GiasData.Ukprn == _foundTrust.GiasData.Ukprn)), Times.Once);
            }

            [Fact]
            public async void GivenResultFromApi_ReturnsMappedResult()
            {
                var response = await _subject.GetByUkprn("12345");

                Assert.Equal($"Mapped {_foundTrust.GiasData.Ukprn}", response.Result.Ukprn);
            }
        }
    }
}