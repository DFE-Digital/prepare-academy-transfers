using API.Mapping;
using API.Models.D365;
using System;
using Xunit;

namespace API.Tests
{
    public class ModelMappingTests
    {
        [Fact]
        public void GetAcademiesD365ModelToGetAcademiesModelMapperTest()
        {
            var academiesD365Model = new GetAcademiesD365Model
            {
                Id = Guid.Parse("4EEAEE65-9A3E-E911-A828-000D3A385A1C"),
                ParentTrustId = Guid.Parse("81014326-5D51-E911-A82E-000D3A385A17"),
                AcademyName = "Academy Name",
                Address = "Address 1",
                DioceseName = "Dicoese Name",
                EstablishmentType = "Academy Converter",
                LocalAuthorityName = "Local Authority",
                LocalAuthorityNumber = "424242",
                OfstedInspectionDate = new DateTime(2020, 01, 01),
                OftstedRating = "Good",
                Predecessor = new GetAcademiesD365Model.PredecessorEstablishment { Pfi = "No" },
                ReligiousCharacter = "Does not apply",
                ReligiousEthos = "Ethos does not apply",
                StateCode = 1,
                StatusCode = 3,
                Urn = "4242"
            };

            var mapper = new GetAcademiesD365ModelToGetAcademiesModelMapper();

            var result = mapper.Map(academiesD365Model);

            Assert.NotNull(result);
            Assert.Equal(Guid.Parse("4EEAEE65-9A3E-E911-A828-000D3A385A1C"), result.Id);
            Assert.Equal(Guid.Parse("81014326-5D51-E911-A82E-000D3A385A17"), result.ParentTrustId);
            Assert.Equal("Academy Name", result.AcademyName);
            Assert.Equal("Address 1", result.Address);
            Assert.Equal("Dicoese Name", result.DioceseName);
            Assert.Equal("Academy Converter", result.EstablishmentType);
            Assert.Equal("Local Authority", result.LocalAuthorityName);
            Assert.Equal("424242", result.LocalAuthorityNumber);
            Assert.Equal(new DateTime(2020, 01, 01), result.OfstedInspectionDate);
            Assert.Equal("Good", result.OfstedRating);
            Assert.Equal("No", result.Pfi);
            Assert.Equal("Does not apply", result.ReligiousCharacter);
            Assert.Equal("Ethos does not apply", result.ReligiousEthos);
            Assert.Equal("4242", result.Urn);
        }

        [Fact]
        public void GetTrustD365ModelToGetTrustModelMapperTest()
        {
            var d365Model = new GetTrustsD365Model
            {
                Address = "Address 1",
                CompaniesHouseNumber = "Company House Number",
                EstablishmentType = "Multi-academy Trust",
                EstablishmentTypeGroup = "Establishment Group",
                Id = Guid.Parse("81014326-5D51-E911-A82E-000D3A385A17"),
                TrustName = "Trust Name",
                TrustReferenceNumber = "TR424242",
                Ukprn = "4242",
                Upin = "42424",
            };

            var mapper = new GetTrustD365ModelToGetTrustsModelMapper();

            var result = mapper.Map(d365Model);

            Assert.NotNull(result);
            Assert.Equal("Address 1", result.Address);
            Assert.Equal("Company House Number", result.CompaniesHouseNumber);
            Assert.Equal("Multi-academy Trust", result.EstablishmentType);
            Assert.Equal("Establishment Group", result.EstablishmentTypeGroup);
            Assert.Equal(Guid.Parse("81014326-5D51-E911-A82E-000D3A385A17"), result.Id);
            Assert.Equal("Trust Name", result.TrustName);
            Assert.Equal("TR424242", result.TrustReferenceNumber);
            Assert.Equal("4242", result.Ukprn);
            Assert.Equal("42424", result.Upin);
        }
    }
}
