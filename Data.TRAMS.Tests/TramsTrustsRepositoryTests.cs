using System.Collections.Generic;
using System.Net.Http;
using Data.Models;
using Data.TRAMS.Models;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Data.TRAMS.Tests
{
    public class TramsTrustsRepositoryTests
    {
        private readonly Mock<ITramsHttpClient> _client;
        private readonly Mock<IMapper<TramsTrustSearchResult, TrustSearchResult>> _mapper;
        private readonly TramsTrustsRepository _subject;

        public TramsTrustsRepositoryTests()
        {
            _client = new Mock<ITramsHttpClient>();
            _mapper = new Mock<IMapper<TramsTrustSearchResult, TrustSearchResult>>();
            _subject = new TramsTrustsRepository(_client.Object, _mapper.Object);
        }

        public class SearchTrustsTests : TramsTrustsRepositoryTests
        {
            [Fact]
            public async void GivenSearchTerm_QueriesTheApiWithTheSearchTerm()
            {
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(TestFixtures.TrustSearchResults.GetTrustSearchResults()))
                });

                await _subject.SearchTrusts("Cats");

                _client.Verify(c => c.GetAsync("trusts?group_name=Cats&urn=Cats&companies_house_number=Cats"),
                    Times.Once);
            }

            [Fact]
            public async void GivenASingleSearchResult_ReturnsTheMappedResult()
            {
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(TestFixtures.TrustSearchResults.GetTrustSearchResults()))
                });

                _mapper.Setup(m => m.Map(It.IsAny<TramsTrustSearchResult>())).Returns<TramsTrustSearchResult>(result =>
                    new TrustSearchResult
                    {
                        Ukprn = $"Mapped {result.Urn}",
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
                        JsonConvert.SerializeObject(TestFixtures.TrustSearchResults.GetTrustSearchResults(2)))
                });

                _mapper.Setup(m => m.Map(It.IsAny<TramsTrustSearchResult>())).Returns<TramsTrustSearchResult>(result =>
                    new TrustSearchResult
                    {
                        Ukprn = $"Mapped {result.Urn}",
                        TrustName = $"Mapped {result.GroupName}"
                    });

                var response = await _subject.SearchTrusts();

                Assert.Equal("Mapped 1", response.Result[0].Ukprn);
                Assert.Equal("Mapped 2", response.Result[1].Ukprn);
            }
        }
    }
}