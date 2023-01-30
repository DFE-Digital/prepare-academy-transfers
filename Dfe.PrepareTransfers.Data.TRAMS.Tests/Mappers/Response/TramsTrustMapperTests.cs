using System.Collections.Generic;
using System.Linq;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Mappers.Response;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Data.TRAMS.Tests.Mappers.Response
{
    public class TramsTrustMapperTests
    {
        private readonly TramsTrustMapper _subject;
        private readonly Mock<IMapper<TramsEstablishment, Academy>> _establishmentMapper;

        public TramsTrustMapperTests()
        {
            _establishmentMapper = new Mock<IMapper<TramsEstablishment, Academy>>();
            _subject = new TramsTrustMapper(_establishmentMapper.Object);
        }

        [Fact]
        public void GivenTramsTrust_ItMapsTrustFieldsCorrectly()
        {
            var trustToMap = new TramsTrust
            {
                GiasData = new TramsTrustGiasData
                {
                    CompaniesHouseNumber = "1231231",
                    GroupContactAddress = new GroupContactAddress
                    {
                        AdditionalLine = "Extra line",
                        County = "County",
                        Locality = "Locality",
                        Postcode = "Postcode",
                        Street = "Street",
                        Town = "Town"
                    },
                    GroupId = "0001",
                    GroupName = "Trust name",
                    Ukprn = "1001",
                },
                IfdData = new TramsTrustIfdData()
                {
                    LeadRscRegion = "London"
                }
            };

            var result = _subject.Map(trustToMap);
            var expectedAddress = new List<string> {"Trust name", "Street", "Town", "County, Postcode"};

            Assert.Equal(trustToMap.GiasData.CompaniesHouseNumber, result.CompaniesHouseNumber);
            Assert.Equal(trustToMap.GiasData.GroupId, result.GiasGroupId);
            Assert.Equal(trustToMap.GiasData.GroupName, result.Name);
            Assert.Equal(trustToMap.GiasData.Ukprn, result.Ukprn);
            Assert.Equal("Not available", result.EstablishmentType);
            Assert.Equal(expectedAddress, result.Address);
            Assert.Equal(trustToMap.IfdData.LeadRscRegion, result.LeadRscRegion);
        }

        [Fact]
        public void GivenTrustWithOneAcademy_MapASingleAcademy()
        {
            _establishmentMapper.Setup(m => m.Map(It.IsAny<TramsEstablishment>()))
                .Returns<TramsEstablishment>(input => new Academy {Ukprn = $"Mapped {input.Ukprn}"});

            var trustToMap = new TramsTrust
            {
                GiasData = new TramsTrustGiasData(),
                Establishments = new List<TramsEstablishment>
                {
                    new TramsEstablishment {Ukprn = "001"}
                }
            };

            var result = _subject.Map(trustToMap);
            _establishmentMapper.Verify(m =>
                m.Map(It.Is<TramsEstablishment>(establishment => establishment.Ukprn == "001")));
            Assert.Equal("Mapped 001", result.Academies[0].Ukprn);
        }
        
        [Fact]
        public void GivenTrustWithTwoAcademies_MapAcademies()
        {
            _establishmentMapper.Setup(m => m.Map(It.IsAny<TramsEstablishment>()))
                .Returns<TramsEstablishment>(input => new Academy {Ukprn = $"Mapped {input.Ukprn}"});

            var trustToMap = new TramsTrust
            {
                GiasData = new TramsTrustGiasData(),
                Establishments = new List<TramsEstablishment>
                {
                    new TramsEstablishment {Ukprn = "001"},
                    new TramsEstablishment {Ukprn = "002"}
                }
            };

            var result = _subject.Map(trustToMap);
            _establishmentMapper.Verify(m =>
                m.Map(It.Is<TramsEstablishment>(establishment => establishment.Ukprn == "001")));
            _establishmentMapper.Verify(m =>
                m.Map(It.Is<TramsEstablishment>(establishment => establishment.Ukprn == "002")));
            Assert.Equal("Mapped 001", result.Academies[0].Ukprn);
            Assert.Equal("Mapped 002", result.Academies[1].Ukprn);
        }
    }
}