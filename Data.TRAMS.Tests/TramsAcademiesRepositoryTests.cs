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
    public class TramsAcademiesRepositoryTests
    {
        private readonly Mock<ITramsHttpClient> _client;
        private readonly Mock<IMapper<TramsEstablishment, Academy>> _mapper;
        private readonly TramsEstablishmentRepository _subject;

        public TramsAcademiesRepositoryTests()
        {
            _client = new Mock<ITramsHttpClient>();
            _mapper = new Mock<IMapper<TramsEstablishment, Academy>>();
            _subject = new TramsEstablishmentRepository(_client.Object, _mapper.Object);
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

            [Fact]
            public async void Given404_ReturnsErrorFromRepository()
            {
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

                var response = await _subject.GetAcademyByUkprn("12345");

                Assert.False(response.IsValid);
                Assert.Equal(HttpStatusCode.NotFound, response.Error.StatusCode);
                Assert.Equal("Academy not found", response.Error.ErrorMessage);
            }

            [Fact]
            public async void Given500_ReturnsErrorFromRepository()
            {
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

                var response = await _subject.GetAcademyByUkprn("12345");

                Assert.False(response.IsValid);
                Assert.Equal(HttpStatusCode.InternalServerError, response.Error.StatusCode);
                Assert.Equal("API encountered an error", response.Error.ErrorMessage);
            }
        }
    }
}