using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Data.TRAMS.Models.EducationPerformance;
using Data.TRAMS.Tests.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;
using EducationPerformance = Data.Models.KeyStagePerformance.EducationPerformance;
using KeyStage2 = Data.Models.KeyStagePerformance.KeyStage2;
using FluentAssertions;

namespace Data.TRAMS.Tests
{
    public class TramsEducationPerformanceRepositoryTests
    {
        private readonly Mock<ITramsHttpClient> _client;
        private readonly Mock<IMapper<TramsEducationPerformance, EducationPerformance>> _mapper;
        private readonly TramsEducationPerformanceRepository _subject;
        private readonly IDistributedCache _distributedCache;

        public TramsEducationPerformanceRepositoryTests()
        {
            _client = new Mock<ITramsHttpClient>();
            _mapper = new Mock<IMapper<TramsEducationPerformance, EducationPerformance>>();
            var opts = Options.Create(new MemoryDistributedCacheOptions());
            _distributedCache = new MemoryDistributedCache(opts);
            _subject = new TramsEducationPerformanceRepository(_client.Object, _mapper.Object, _distributedCache);
        }

        public class GetByAcademyUrn : TramsEducationPerformanceRepositoryTests
        {
            private readonly TramsEducationPerformance _foundEducationPerformance;

            public GetByAcademyUrn()
            {
                _foundEducationPerformance = TestFixtures.EducationPerformance.GetSingleTramsEducationPerformance();
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(_foundEducationPerformance))
                });

                _mapper.Setup(m => 
                    m.Map(It.IsAny<TramsEducationPerformance>()))
                    .Returns<TramsEducationPerformance>((input) => new EducationPerformance()
                {
                    KeyStage2Performance = new List<KeyStage2>
                    {
                        new KeyStage2
                        {
                            Year = $"Mapped {input.KeyStage2[0].Year}"
                        }
                    }
                });
            }
            
            [Fact]
            public async void GivenUrn_GetsEducationPerformance_FromCache()
            {
                var cacheKey = "GetPerformanceByAcademy_12345";
                var performance = new RepositoryResult<EducationPerformance>
                {
                    Result = new EducationPerformance()
                    {
                      KeyStage2AdditionalInformation = "KeyStage2AdditionalInformation"
                    }
                };
                await _distributedCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(performance));
            
                var result = await _subject.GetByAcademyUrn("12345");
                Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(performance));
            }
            
            [Fact]
            public async void GivenUrn_GetsEducationPerformance_SetsCache()
            {
                var result = await _subject.GetByAcademyUrn("12345");
                var cached = await _distributedCache.GetStringAsync("GetPerformanceByAcademy_12345");
                Assert.Equal(JsonConvert.SerializeObject(result),cached);
            }
            
            [Fact]
            public async void GivenUrn_CallsEducationPerformanceApiEndpoint()
            {
                await _subject.GetByAcademyUrn("12345");

                _client.Verify(c => c.GetAsync("educationPerformance/12345"), Times.Once);
            }
            
            [Fact]
            public async void GivenResultFromApi_MapsResult()
            {
                await _subject.GetByAcademyUrn("12345");

                _mapper.Verify(m =>
                    m.Map(It.Is<TramsEducationPerformance>(perf => 
                        perf.KeyStage2[0].Year == _foundEducationPerformance.KeyStage2[0].Year)), Times.Once);
            }
            
            [Fact]
            public async void GivenResultFromApi_ReturnsMappedResult()
            {
                var response = await _subject.GetByAcademyUrn("12345");

                Assert.Equal($"Mapped {_foundEducationPerformance.KeyStage2[0].Year}", response.Result.KeyStage2Performance[0].Year);
            }
            
            [Theory]
            [InlineData(HttpStatusCode.InternalServerError)]
            [InlineData(HttpStatusCode.Unauthorized)]
            public async void GivenApiReturnsError_ThrowsApiError(HttpStatusCode httpStatusCode)
            {
                HttpClientTestHelpers.SetupGet<TramsEducationPerformance>(_client, null, httpStatusCode);
                
                await Assert.ThrowsAsync<TramsApiException>(() => _subject.GetByAcademyUrn("12345"));
            }
            
            [Fact]
            public async void GivenApiReturnsNotFound_ShouldReturnEmptyEducationPerformance()
            {
                HttpClientTestHelpers.SetupGet<TramsEducationPerformance>(_client, null, HttpStatusCode.NotFound);
                var result = await _subject.GetByAcademyUrn("12345");

                var blankEducationPerformance = new EducationPerformance();
                
                Assert.IsType<EducationPerformance>(result.Result);
                blankEducationPerformance.Should().BeEquivalentTo(result.Result);
            }
        }
    }
}