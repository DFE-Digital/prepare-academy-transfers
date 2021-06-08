using System.Collections.Generic;
using Data.Models;
using Data.Models.Projects;
using Data.TRAMS.Mappers.Request;
using Data.TRAMS.Models;
using Data.TRAMS.Tests.TestFixtures;
using Xunit;

namespace Data.TRAMS.Tests.Mappers.Request
{
    public class InternalProjectToUpdateMapperTests
    {
        [Fact]
        public void GivenProject_ReturnsMappedProject()
        {
            var subject = new InternalProjectToUpdateMapper();
            var toMap = new Project
            {
                Benefits = new TransferBenefits
                {
                    IntendedBenefits = new List<TransferBenefits.IntendedBenefit>
                    {
                        TransferBenefits.IntendedBenefit.ImprovingOfstedRating,
                        TransferBenefits.IntendedBenefit.Other
                    },
                    OtherIntendedBenefit = "Other intended benefit",
                    OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>
                    {
                        {TransferBenefits.OtherFactor.HighProfile, "High profile"},
                        {TransferBenefits.OtherFactor.FinanceAndDebtConcerns, "Finance"},
                        {TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues, "Land"}
                    }
                },
                Dates = new TransferDates(),
                Features = new TransferFeatures(),
                Name = "Project name",
                Rationale = new TransferRationale(),
                State = "State",
                Status = "Status",
                Urn = "12345",
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies {IncomingTrustUkprn = "1234", OutgoingAcademyUkprn = "4321"}
                },
                OutgoingTrustName = "Outgoing trust name",
                OutgoingTrustUkprn = "Outgoing trust Ukprn"
            };

            var result = subject.Map(toMap);

            Assert.Equal(toMap.OutgoingTrustUkprn, result.OutgoingTrustUkprn);
            Assert.Equal(toMap.State, result.State);
            Assert.Equal(toMap.Status, result.Status);
            Assert.Equal(toMap.Urn, result.ProjectUrn);

            AssertTransferringAcademiesCorrect(toMap, result);
            AssertBenefitsAreCorrect(result);
        }

        private static void AssertBenefitsAreCorrect(TramsProjectUpdate result)
        {
            Assert.Equal("ImprovingOfstedRating", result.Benefits.IntendedTransferBenefits.SelectedBenefits[0]);
            Assert.Equal("Other", result.Benefits.IntendedTransferBenefits.SelectedBenefits[1]);
            Assert.Equal("Other intended benefit", result.Benefits.IntendedTransferBenefits.OtherBenefitValue);
            Assert.True(result.Benefits.OtherFactorsToConsider.HighProfile.ShouldBeConsidered);
            Assert.Equal("High profile", result.Benefits.OtherFactorsToConsider.HighProfile.FurtherSpecification);
            Assert.True(result.Benefits.OtherFactorsToConsider.FinanceAndDebt.ShouldBeConsidered);
            Assert.Equal("Finance", result.Benefits.OtherFactorsToConsider.FinanceAndDebt.FurtherSpecification);
            Assert.True(result.Benefits.OtherFactorsToConsider.ComplexLandAndBuilding.ShouldBeConsidered);
            Assert.Equal("Land", result.Benefits.OtherFactorsToConsider.ComplexLandAndBuilding.FurtherSpecification);
        }

        private static void AssertTransferringAcademiesCorrect(Project toMap, TramsProjectUpdate result)
        {
            Assert.Equal(toMap.TransferringAcademies[0].IncomingTrustUkprn,
                result.TransferringAcademies[0].IncomingTrustUkprn);
            Assert.Equal(toMap.TransferringAcademies[0].OutgoingAcademyUkprn,
                result.TransferringAcademies[0].OutgoingAcademyUkprn);
        }
    }
}