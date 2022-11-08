using System.Net;
using System.Net.Http;
using Data.Models;
using Data.TRAMS.Models;
using Data.TRAMS.Tests.TestFixtures;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Data.TRAMS.Tests
{
    public class TramsAcademiesRepositoryTests
    {
        private readonly Mock<ITramsHttpClient> _client;
        private readonly Mock<IMapper<TramsEstablishment, Academy>> _mapper;
        private readonly TramsEstablishmentRepository _subject;
        private readonly IDistributedCache _distributedCache;

        public TramsAcademiesRepositoryTests()
        {
            _client = new Mock<ITramsHttpClient>();
            _mapper = new Mock<IMapper<TramsEstablishment, Academy>>();
            var opts = Options.Create(new MemoryDistributedCacheOptions());
            _distributedCache = new MemoryDistributedCache(opts);
            _subject = new TramsEstablishmentRepository(_client.Object, _mapper.Object, _distributedCache);
        }

        public class GetAcademyByUkprnTests : TramsAcademiesRepositoryTests
        {
            [Fact]
            public async void GivenUkprn_GetsAcademyWithGivenUkprn()
            {
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(Establishments.SingleEstablishment()))
                });

                await _subject.GetAcademyByUkprn("12345");

                _client.Verify(c => c.GetAsync("establishment/12345"), Times.Once);
            }

            [Fact]
            public async void GivenUkprn_GetsAcademyFromCache()
            {
                var cacheKey = "GetAcademyByUkprn_12345";
                var academy = new RepositoryResult<Academy>
                {
                    Result = new Academy
                    {
                        Urn = "toJson"
                    }
                };
                await _distributedCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(academy));

                var result = await _subject.GetAcademyByUkprn("12345");
                Assert.Equal(JsonConvert.SerializeObject(academy), JsonConvert.SerializeObject(result));
            }
            
            [Fact]
            public async void GivenUrn_GetsAcademy_SetsCache()
            {
                var academy = Establishments.SingleEstablishment();
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(academy))
                });

                _mapper.Setup(m => m.Map(It.IsAny<TramsEstablishment>())).Returns<TramsEstablishment>(input =>
                    new Academy
                    {
                        Ukprn = $"Mapped {academy.Ukprn}"
                    });

                var result = await _subject.GetAcademyByUkprn("12345");
                var cached = await _distributedCache.GetStringAsync("GetAcademyByUkprn_12345");
                Assert.Equal(JsonConvert.SerializeObject(result),cached);
            }

            [Fact]
            public async void GivenUkprn_MapFoundAcademy()
            {
                var academy = Establishments.SingleEstablishment();
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(academy))
                });

                await _subject.GetAcademyByUkprn("12345");

                _mapper.Verify(
                    m => m.Map(It.Is<TramsEstablishment>(mappedAcademy => mappedAcademy.Ukprn == academy.Ukprn)),
                    Times.Once);
            }

            [Fact]
            public async void GivenUkprn_ReturnsMappedAcademy()
            {
                var academy = Establishments.SingleEstablishment();
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(academy))
                });

                _mapper.Setup(m => m.Map(It.IsAny<TramsEstablishment>())).Returns<TramsEstablishment>(input =>
                    new Academy
                    {
                        Ukprn = $"Mapped {academy.Ukprn}"
                    });

                var response = await _subject.GetAcademyByUkprn("12345");

                Assert.Equal("Mapped 12345", response.Result.Ukprn);
            }

            [Theory]
            [InlineData(HttpStatusCode.NotFound)]
            [InlineData(HttpStatusCode.InternalServerError)]
            public async void GivenApiReturnsError_ThrowsApiError(HttpStatusCode httpStatusCode)
            {
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = httpStatusCode
                });

                await Assert.ThrowsAsync<TramsApiException>(() => _subject.GetAcademyByUkprn("12345"));
            }
        }
    }
}