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
        private readonly Mock<IMapper<TramsAcademy, Academy>> _mapper;
        private readonly TramsAcademiesRepository _subject;

        public TramsAcademiesRepositoryTests()
        {
            _client = new Mock<ITramsHttpClient>();
            _mapper = new Mock<IMapper<TramsAcademy, Academy>>();
            _subject = new TramsAcademiesRepository(_client.Object, _mapper.Object);
        }

        public class GetAcademyByUkprnTests : TramsAcademiesRepositoryTests
        {
            [Fact]
            public async void GivenUkprn_GetsAcademyWithGivenUkprn()
            {
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(Academies.SingleAcademy()))
                });

                await _subject.GetAcademyByUkprn("12345");

                _client.Verify(c => c.GetAsync("establishment/12345"), Times.Once);
            }

            [Fact]
            public async void GivenUkprn_MapFoundAcademy()
            {
                var academy = Academies.SingleAcademy();
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(academy))
                });

                await _subject.GetAcademyByUkprn("12345");

                _mapper.Verify(m => m.Map(It.Is<TramsAcademy>(mappedAcademy => mappedAcademy.Ukprn == academy.Ukprn)),
                    Times.Once);
            }

            [Fact]
            public async void GivenUkprn_ReturnsMappedAcademy()
            {
                var academy = Academies.SingleAcademy();
                _client.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(academy))
                });

                _mapper.Setup(m => m.Map(It.IsAny<TramsAcademy>())).Returns<TramsAcademy>(input => new Academy
                {
                    Ukprn = $"Mapped {academy.Ukprn}"
                });

                var response = await _subject.GetAcademyByUkprn("12345");

                Assert.Equal("Mapped 12345", response.Result.Ukprn);
            }
        }
    }
}