using System.Collections.Generic;
using Data.TRAMS.Mappers.Response;
using Data.TRAMS.Models;
using Data.TRAMS.Models.AcademyTransferProject;
using Xunit;

namespace Data.TRAMS.Tests.Mappers.Response
{
    public class TramsProjectSummariesMapperTests
    {
        [Fact]
        public void GivenProjectSummary_MapsCorrectly()
        {
            var toMap = new TramsProjectSummary
            {
                OutgoingTrust = new TrustSummary
                {
                    GroupId = "321",
                    GroupName = "Outgoing trust name",
                    Ukprn = "123",
                },
                OutgoingTrustUkprn = "123",
                ProjectNumber = "Number",
                ProjectUrn = "URN",
                TransferringAcademies = new List<TransferringAcademy>
                {
                    new TransferringAcademy
                    {
                        OutgoingAcademy = new AcademySummary
                        {
                            Name = "Outgoing academy",
                            Ukprn = "789",
                            Urn = "987"
                        },
                        IncomingTrust = new TrustSummary
                        {
                            GroupId = "654",
                            GroupName = "Incoming trust",
                            Ukprn = "456"
                        },
                        IncomingTrustUkprn = "456", OutgoingAcademyUkprn = "789"
                    }
                }
            };

            var subject = new TramsProjectSummariesMapper();
            var res = subject.Map(toMap);

            Assert.Equal(toMap.ProjectUrn, res.Urn);
            Assert.Equal(toMap.OutgoingTrust.GroupName, res.OutgoingTrustName);
            Assert.Equal(toMap.TransferringAcademies[0].OutgoingAcademyUkprn,
                res.TransferringAcademies[0].OutgoingAcademyUkprn);
            Assert.Equal(toMap.TransferringAcademies[0].OutgoingAcademy.Name,
                res.TransferringAcademies[0].OutgoingAcademyName);
            Assert.Equal(toMap.TransferringAcademies[0].IncomingTrust.GroupName,
                res.TransferringAcademies[0].IncomingTrustName);
            Assert.Equal(toMap.TransferringAcademies[0].IncomingTrustUkprn,
                res.TransferringAcademies[0].IncomingTrustUkprn);
        }
    }
}