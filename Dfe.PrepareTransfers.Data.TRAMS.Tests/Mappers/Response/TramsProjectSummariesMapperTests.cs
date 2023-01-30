using System.Collections.Generic;
using Dfe.PrepareTransfers.Data.TRAMS.Mappers.Response;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Models.AcademyTransferProject;
using Xunit;

namespace Dfe.PrepareTransfers.Data.TRAMS.Tests.Mappers.Response
{
    public class TramsProjectSummariesMapperTests
    {
        [Fact]
        public void GivenProjectSummary_MapsCorrectly()
        {
            var toMap = new TramsProjectSummary
            {
                OutgoingTrustName = "Outgoing trust name",
                OutgoingTrustUkprn = "123",
                ProjectReference = "SW-MAT-123456789",
                ProjectUrn = "URN",
                TransferringAcademies = new List<TransferringAcademy>
                {
                    new TransferringAcademy
                    {
                        IncomingTrustUkprn = "456", 
                        IncomingTrustName = "Incoming trust name", 
                        OutgoingAcademyUkprn = "789"
                    }
                }
            };

            var subject = new TramsProjectSummariesMapper();
            var res = subject.Map(toMap);

            Assert.Equal(toMap.ProjectUrn, res.Urn);
            Assert.Equal(toMap.ProjectReference, res.Reference);
            Assert.Equal(toMap.OutgoingTrustName, res.OutgoingTrustName);
            Assert.Equal(toMap.OutgoingTrustUkprn, res.OutgoingTrustUkprn);
            Assert.Equal(toMap.TransferringAcademies[0].OutgoingAcademyUkprn,
                res.TransferringAcademies[0].OutgoingAcademyUkprn);
            Assert.Equal(toMap.TransferringAcademies[0].IncomingTrustName,
                res.TransferringAcademies[0].IncomingTrustName);
            Assert.Equal(toMap.TransferringAcademies[0].IncomingTrustUkprn,
                res.TransferringAcademies[0].IncomingTrustUkprn);
        }
    }
}