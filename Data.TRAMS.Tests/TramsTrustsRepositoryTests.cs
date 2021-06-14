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

                _trustSearchResultsMapper.Setup(m => m.Map(It.IsAny<TramsTrustSearchResult>()))
                    .Returns<TramsTrustSearchResult>(result =>
                        new TrustSearchResult
                        {
                            Ukprn = $"Mapped {result.Ukprn}",
                            TrustName = $"Mapped {result.GroupName}"
                        });

                var response = await _subject.SearchTrusts();

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
                            Ukprn = $"Mapped {result.Ukprn}",
                            TrustName = $"Mapped {result.GroupName}"
                        });

                var response = await _subject.SearchTrusts();

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