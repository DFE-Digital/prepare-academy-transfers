using System.Collections.Generic;
using Dfe.Academies.Contracts.V4;
using Dfe.Academies.Contracts.V4.Trusts;
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
            var trustToMap = new TrustDto
            {

                    CompaniesHouseNumber = "1231231",
                    Address = new AddressDto
                    {
                        Additional = "Extra line",
                        County = "County",
                        Locality = "Locality",
                        Postcode = "Postcode",
                        Street = "Street",
                        Town = "Town"
                    },
                    ReferenceNumber = "0001",
                    Name = "Trust name",
                    Ukprn = "1001",
            };

            var result = _subject.Map(trustToMap);
            var expectedAddress = new List<string> {"Trust name", "Street", "Town", "County, Postcode"};

            Assert.Equal(trustToMap.CompaniesHouseNumber, result.CompaniesHouseNumber);
            Assert.Equal(trustToMap.ReferenceNumber, result.GiasGroupId);
            Assert.Equal(trustToMap.Name, result.Name);
            Assert.Equal(trustToMap.Ukprn, result.Ukprn);
            Assert.Equal("Not available", result.EstablishmentType);
            Assert.Equal(expectedAddress, result.Address);
            //Assert.Equal(trustToMap.IfdData.LeadRscRegion, result.LeadRscRegion);
        }

        //ToDo: fix these once we can get establishments by ukprn
        //[Fact]
        //public void GivenTrustWithOneAcademy_MapASingleAcademy()
        //{
        //    _establishmentMapper.Setup(m => m.Map(It.IsAny<TramsEstablishment>()))
        //        .Returns<TramsEstablishment>(input => new Academy {Ukprn = $"Mapped {input.Ukprn}"});

        //    var trustToMap = new TrustDto
        //    {
        //        GiasData = new TramsTrustGiasData(),
        //        Establishments = new List<TramsEstablishment>
        //        {
        //            new TramsEstablishment {Ukprn = "001"}
        //        }
        //    };

        //    var result = _subject.Map(trustToMap);
        //    _establishmentMapper.Verify(m =>
        //        m.Map(It.Is<TramsEstablishment>(establishment => establishment.Ukprn == "001")));
        //    Assert.Equal("Mapped 001", result.Academies[0].Ukprn);
        //}
        
        //[Fact]
        //public void GivenTrustWithTwoAcademies_MapAcademies()
        //{
        //    _establishmentMapper.Setup(m => m.Map(It.IsAny<TramsEstablishment>()))
        //        .Returns<TramsEstablishment>(input => new Academy {Ukprn = $"Mapped {input.Ukprn}"});

        //    var trustToMap = new TramsTrust
        //    {
        //        GiasData = new TramsTrustGiasData(),
        //        Establishments = new List<TramsEstablishment>
        //        {
        //            new TramsEstablishment {Ukprn = "001"},
        //            new TramsEstablishment {Ukprn = "002"}
        //        }
        //    };

        //    var result = _subject.Map(trustToMap);
        //    _establishmentMapper.Verify(m =>
        //        m.Map(It.Is<TramsEstablishment>(establishment => establishment.Ukprn == "001")));
        //    _establishmentMapper.Verify(m =>
        //        m.Map(It.Is<TramsEstablishment>(establishment => establishment.Ukprn == "002")));
        //    Assert.Equal("Mapped 001", result.Academies[0].Ukprn);
        //    Assert.Equal("Mapped 002", result.Academies[1].Ukprn);
        //}
    }
}