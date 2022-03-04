using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Data.TRAMS.Models.EducationPerformance;
using Data.TRAMS.Tests.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Newtonsoft.Json;
using Xunit;
using EducationPerformance = Data.Models.KeyStagePerformance.EducationPerformance;
using KeyStage2 = Data.Models.KeyStagePerformance.KeyStage2;

namespace Data.TRAMS.Tests
{
    public class TramsEducationPerformanceRepositoryTests
    {
        private readonly Mock<ITramsHttpClient> _client;
        private readonly Mock<IMapper<TramsEducationPerformance, EducationPerformance>> _mapper;
        private readonly TramsEducationPerformanceRepository _subject;

        public TramsEducationPerformanceRepositoryTests()
        {
            _client = new Mock<ITramsHttpClient>();
            _mapper = new Mock<IMapper<TramsEducationPerformance, EducationPerformance>>();
            _subject = new TramsEducationPerformanceRepository(_client.Object, _mapper.Object, new Mock<IDistributedCache>().Object);
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
            [InlineData(HttpStatusCode.NotFound)]
            [InlineData(HttpStatusCode.InternalServerError)]
            public async void GivenApiReturnsError_ThrowsApiError(HttpStatusCode httpStatusCode)
            {
                HttpClientTestHelpers.SetupGet<TramsEducationPerformance>(_client, null, httpStatusCode);
                
                await Assert.ThrowsAsync<TramsApiException>(() => _subject.GetByAcademyUrn("12345"));
            }
        }
    }
}