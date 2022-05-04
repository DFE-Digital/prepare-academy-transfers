using System.Collections.Generic;
using Data.Models;
using Data.Models.Projects;
using Data.TRAMS.Mappers.Request;
using Data.TRAMS.Models;
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
                Dates = new TransferDates
                {
                    FirstDiscussed = "01/01/2020",
                    Htb = "02/01/2020",
                    Target = "03/01/2020",
                    HasHtbDate = true,
                    HasFirstDiscussedDate = true,
                    HasTargetDateForTransfer = true
                },
                Features = new TransferFeatures
                {
                    TypeOfTransfer = TransferFeatures.TransferTypes.Other,
                    OtherTypeOfTransfer = "Other type of transfer",
                    WhoInitiatedTheTransfer = TransferFeatures.ProjectInitiators.Dfe
                },
                
                Rationale = new TransferRationale
                {
                    Project = "Project rationale",
                    Trust = "Trust rationale"
                },
                State = "State",
                Status = "Status",
                Urn = "12345",
                Reference = "SW-MAT-12345678",
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies
                    {
                        IncomingTrustUkprn = "1234", OutgoingAcademyUkprn = "4321", IncomingTrustName = "incoming trust",
                        PupilNumbersAdditionalInformation = "PupilNumbersAdditionalInformation",
                        LatestOfstedReportAdditionalInformation = "LatestOfstedJudgementAdditionalInformation",
                        KeyStage2PerformanceAdditionalInformation = "KeyStage2PerformanceAdditionalInformation",
                        KeyStage4PerformanceAdditionalInformation = "KeyStage4PerformanceAdditionalInformation",
                        KeyStage5PerformanceAdditionalInformation = "KeyStage5PerformanceAdditionalInformation"
                    }
                },
                OutgoingTrustName = "Outgoing trust name",
                OutgoingTrustUkprn = "Outgoing trust Ukprn",
                AcademyAndTrustInformation = new TransferAcademyAndTrustInformation
                {
                    Author = "Author",
                    Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Empty
                },
                GeneralInformationAdditionalInformation = "GeneralInformationAdditionalInformation",
              
            };

            var result = subject.Map(toMap);

            Assert.Equal(toMap.OutgoingTrustUkprn, result.OutgoingTrustUkprn);
            Assert.Equal(toMap.State, result.State);
            Assert.Equal(toMap.Status, result.Status);
            Assert.Equal(toMap.Urn, result.ProjectUrn);
            Assert.Equal(toMap.Reference, result.ProjectReference);

            AssertTransferringAcademiesCorrect(toMap, result);
            AssertBenefitsAreCorrect(toMap, result);
            AssertDatesAreCorrect(toMap, result);
            AssertFeaturesAreCorrect(toMap, result);
            AssertRationaleIsCorrect(toMap, result);
            AssertGeneralInformationIsCorrect(toMap, result);
        }

        //todo: add test for multiple academies
        
        [Fact]
        public void GivenHasDateIsFalse_ShouldSetHasDateToNull()
        {
            var subject = new InternalProjectToUpdateMapper();
            var toMap = new Project
            {
                Dates = new TransferDates
                {
                    Htb = "Date1",
                    Target = "Date2",
                    FirstDiscussed = "Date3",
                    HasHtbDate = false,
                    HasTargetDateForTransfer = false,
                    HasFirstDiscussedDate = false
                },
            };

            var result = subject.Map(toMap);
            Assert.False(result.Dates.HasHtbDate);
            Assert.False(result.Dates.HasTargetDateForTransfer);
            Assert.False(result.Dates.HasTransferFirstDiscussedDate);
            Assert.Null(result.Dates.HtbDate);
            Assert.Null(result.Dates.TargetDateForTransfer);
            Assert.Null(result.Dates.TransferFirstDiscussed);
        }
        
        [Fact]
        public void GivenHasDateIsNull_ShouldSetHasDate()
        {
            var subject = new InternalProjectToUpdateMapper();
            var toMap = new Project
            {
                Dates = new TransferDates
                {
                    Htb = "Date1",
                    Target = "Date2",
                    FirstDiscussed = "Date3",
                    HasHtbDate = null,
                    HasTargetDateForTransfer = null,
                    HasFirstDiscussedDate = null
                },
            };

            var result = subject.Map(toMap);
            Assert.Null(result.Dates.HasHtbDate);
            Assert.Null(result.Dates.HasTargetDateForTransfer);
            Assert.Null(result.Dates.HasTransferFirstDiscussedDate);
            Assert.Equal(result.Dates.HtbDate, toMap.Dates.Htb);
            Assert.Equal(result.Dates.TargetDateForTransfer, toMap.Dates.Target);
            Assert.Equal(result.Dates.TransferFirstDiscussed, toMap.Dates.FirstDiscussed);
        }

        private static void AssertGeneralInformationIsCorrect(Project toMap, TramsProjectUpdate result)
        {
            Assert.Equal(toMap.AcademyAndTrustInformation.Author, result.GeneralInformation.Author);
            Assert.Equal(toMap.AcademyAndTrustInformation.Recommendation.ToString(), result.GeneralInformation.Recommendation);
        }

        private static void AssertRationaleIsCorrect(Project toMap, TramsProjectUpdate result)
        {
            Assert.Equal(toMap.Rationale.Project, result.Rationale.ProjectRationale);
            Assert.Equal(toMap.Rationale.Trust, result.Rationale.TrustSponsorRationale);
            Assert.Equal(toMap.Rationale.IsCompleted, result.Rationale.IsCompleted);
        }

        private static void AssertFeaturesAreCorrect(Project toMap, TramsProjectUpdate result)
        {
            Assert.Equal(toMap.Features.TypeOfTransfer.ToString(), result.Features.TypeOfTransfer);
            Assert.Equal(toMap.Features.OtherTypeOfTransfer, result.Features.OtherTransferTypeDescription);
            Assert.Equal(toMap.Features.WhoInitiatedTheTransfer.ToString(), result.Features.WhoInitiatedTheTransfer);
            Assert.Equal(toMap.Features.IsCompleted, result.Features.IsCompleted);
        }

        private static void AssertDatesAreCorrect(Project toMap, TramsProjectUpdate result)
        {
            Assert.Equal(toMap.Dates.FirstDiscussed, result.Dates.TransferFirstDiscussed);
            Assert.Equal(toMap.Dates.Htb, result.Dates.HtbDate);
            Assert.Equal(toMap.Dates.Target, result.Dates.TargetDateForTransfer);
            Assert.True(result.Dates.HasHtbDate);
            Assert.True(result.Dates.HasTargetDateForTransfer);
            Assert.True(result.Dates.HasTransferFirstDiscussedDate);
        }

        private static void AssertBenefitsAreCorrect(Project toMap, TramsProjectUpdate result)
        {
            Assert.Equal(toMap.Benefits.IntendedBenefits[0].ToString(), result.Benefits.IntendedTransferBenefits.SelectedBenefits[0]);
            Assert.Equal(toMap.Benefits.IntendedBenefits[1].ToString(), result.Benefits.IntendedTransferBenefits.SelectedBenefits[1]);
            Assert.Equal("Other intended benefit", result.Benefits.IntendedTransferBenefits.OtherBenefitValue);
            Assert.True(result.Benefits.OtherFactorsToConsider.HighProfile.ShouldBeConsidered);
            Assert.Equal("High profile", result.Benefits.OtherFactorsToConsider.HighProfile.FurtherSpecification);
            Assert.True(result.Benefits.OtherFactorsToConsider.FinanceAndDebt.ShouldBeConsidered);
            Assert.Equal("Finance", result.Benefits.OtherFactorsToConsider.FinanceAndDebt.FurtherSpecification);
            Assert.True(result.Benefits.OtherFactorsToConsider.ComplexLandAndBuilding.ShouldBeConsidered);
            Assert.Equal("Land", result.Benefits.OtherFactorsToConsider.ComplexLandAndBuilding.FurtherSpecification);
            Assert.Equal(toMap.Benefits.IsCompleted, result.Benefits.IsCompleted);
        }

        private static void AssertTransferringAcademiesCorrect(Project toMap, TramsProjectUpdate result)
        {
            Assert.Equal(toMap.TransferringAcademies[0].IncomingTrustUkprn,
                result.TransferringAcademies[0].IncomingTrustUkprn);
            Assert.Equal(toMap.TransferringAcademies[0].OutgoingAcademyUkprn,
                result.TransferringAcademies[0].OutgoingAcademyUkprn);
            Assert.Equal(toMap.TransferringAcademies[0].PupilNumbersAdditionalInformation, result.TransferringAcademies[0].PupilNumbersAdditionalInformation);
            Assert.Equal(toMap.TransferringAcademies[0].LatestOfstedReportAdditionalInformation, result.TransferringAcademies[0].LatestOfstedReportAdditionalInformation);
            Assert.Equal(toMap.TransferringAcademies[0].KeyStage2PerformanceAdditionalInformation, result.TransferringAcademies[0].KeyStage2PerformanceAdditionalInformation);
            Assert.Equal(toMap.TransferringAcademies[0].KeyStage4PerformanceAdditionalInformation, result.TransferringAcademies[0].KeyStage4PerformanceAdditionalInformation);
            Assert.Equal(toMap.TransferringAcademies[0].KeyStage5PerformanceAdditionalInformation, result.TransferringAcademies[0].KeyStage5PerformanceAdditionalInformation);

        }
    }
}