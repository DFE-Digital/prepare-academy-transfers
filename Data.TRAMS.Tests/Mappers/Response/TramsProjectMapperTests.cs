using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.Models.Projects;
using Data.TRAMS.ExtensionMethods;
using Data.TRAMS.Mappers.Response;
using Data.TRAMS.Models;
using Data.TRAMS.Models.AcademyTransferProject;
using Helpers;
using Xunit;

namespace Data.TRAMS.Tests.Mappers.Response
{
    public class TramsProjectMapperTests
    {
        [Fact]
        public void GivenTramsProject_MapsCorrectly()
        {
            var toMap = new TramsProject
            {
                Benefits = new AcademyTransferProjectBenefits
                {
                    IntendedTransferBenefits = new IntendedTransferBenefits
                    {
                        SelectedBenefits = new List<string>
                        {
                            TransferBenefits.IntendedBenefit.ImprovingSafeguarding.ToString(),
                            TransferBenefits.IntendedBenefit.Other.ToString()
                        },
                        OtherBenefitValue = "Other benefit value"
                    },
                    OtherFactorsToConsider = new OtherFactorsToConsider
                    {
                        HighProfile = new OtherFactor
                            {FurtherSpecification = "High profile", ShouldBeConsidered = true},
                        FinanceAndDebt = new OtherFactor
                            {FurtherSpecification = "Finance", ShouldBeConsidered = true},
                        ComplexLandAndBuilding = new OtherFactor
                            {FurtherSpecification = "Complex land and building", ShouldBeConsidered = true},
                        OtherRisks = new OtherFactor
                        {
                            FurtherSpecification = "Other risks", ShouldBeConsidered = true
                        }
                    },
                    IsCompleted = true
                },
                Dates = new AcademyTransferProjectDates
                {
                    HtbDate = "01/01/2020",
                    TargetDateForTransfer = "02/01/2020",
                    HasHtbDate = true,
                    HasTargetDateForTransfer = true,
                },
                Features = new AcademyTransferProjectFeatures
                {
                    TypeOfTransfer = TransferFeatures.TransferTypes.TrustsMerging.ToString(),
                    OtherTransferTypeDescription = "Other",
                    RddOrEsfaIntervention = true,
                    WhoInitiatedTheTransfer = TransferFeatures.ReasonForTheTransferTypes.Dfe.ToString(),
                    RddOrEsfaInterventionDetail = "Intervention details",
                    IsCompleted = true
                },
                Rationale = new AcademyTransferProjectRationale
                {
                    ProjectRationale = "Project rationale",
                    TrustSponsorRationale = "Trust rationale",
                    IsCompleted = true
                },
                GeneralInformation = new AcademyTransferProjectAcademyAndTrustInformation
                {
                    Author = "Author",
                    Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Approve.ToString()
                },
                State = "State",
                Status = "Status",
                OutgoingTrust = new TrustSummary
                {
                    GroupName = "Outgoing trust name",
                    Ukprn = "123"
                },
                ProjectUrn = "Urn",
                ProjectReference = "SW-SAT-12345678",
                TransferringAcademies = new List<TransferringAcademy>
                {
                    new TransferringAcademy
                    {
                        IncomingTrust = new TrustSummary
                        {
                            GroupName = "Incoming trust",
                            Ukprn = "456"
                        },
                        OutgoingAcademy = new AcademySummary
                        {
                            Name = "Outgoing academy",
                            Urn = "789",
                            Ukprn = "987"
                        },
                        PupilNumbersAdditionalInformation = "PupilNumbersAdditionalInformation",
                        LatestOfstedReportAdditionalInformation = "LatestOfstedJudgementAdditionalInformation",
                        KeyStage2PerformanceAdditionalInformation = "KeyStage2PerformanceAdditionalInformation",
                        KeyStage4PerformanceAdditionalInformation = "KeyStage4PerformanceAdditionalInformation",
                        KeyStage5PerformanceAdditionalInformation = "KeyStage5PerformanceAdditionalInformation"
                    }
                },
                LegalRequirements = new AcademyTransferProjectLegalRequirements()
                {
                    TrustAgreement = "No",
                    DiocesanConsent = "No",
                    FoundationConsent = "No",
                    IsCompleted= false,
                },
                OutgoingTrustUkprn = "123"
            };

            var subject = new TramsProjectMapper();
            var result = subject.Map(toMap);

            Assert.Equal(toMap.State, result.State);
            Assert.Equal(toMap.Status, result.Status);
            Assert.Equal(toMap.ProjectUrn, result.Urn);
            Assert.Equal(toMap.ProjectReference, result.Reference);
            Assert.Equal(toMap.OutgoingTrustUkprn, result.OutgoingTrustUkprn);
            Assert.Equal(toMap.OutgoingTrust.GroupName, result.OutgoingTrustName);

            AssertBenefitsCorrect(toMap, result);
            AssertDatesCorrect(toMap, result);
            AssertFeaturesCorrect(toMap, result);
            AssertLegalRequirementsAreCorrect(toMap, result);
            AssertRationaleCorrect(toMap, result);
            AssertGeneralInformationCorrect(toMap, result);
            AssertTransferringAcademiesCorrect(toMap, result);
        }

        private void AssertGeneralInformationCorrect(TramsProject toMap, Project result)
        {
            Assert.Equal(toMap.GeneralInformation.Author, result.AcademyAndTrustInformation.Author);
            var expectedRecommendation =
                EnumHelpers<TransferAcademyAndTrustInformation.RecommendationResult>.Parse(toMap.GeneralInformation.Recommendation);
            Assert.Equal(expectedRecommendation, result.AcademyAndTrustInformation.Recommendation);
        }

        private static void AssertTransferringAcademiesCorrect(TramsProject toMap, Project result)
        {
            var expectedTransfer = toMap.TransferringAcademies[0];
            Assert.Equal(expectedTransfer.IncomingTrust.GroupName, result.TransferringAcademies[0].IncomingTrustName);
            Assert.Equal(expectedTransfer.IncomingTrust.Ukprn, result.TransferringAcademies[0].IncomingTrustUkprn);
            Assert.Equal(expectedTransfer.OutgoingAcademy.Name, result.TransferringAcademies[0].OutgoingAcademyName);
            Assert.Equal(expectedTransfer.OutgoingAcademy.Ukprn, result.TransferringAcademies[0].OutgoingAcademyUkprn);
            Assert.Equal(expectedTransfer.OutgoingAcademy.Urn, result.TransferringAcademies[0].OutgoingAcademyUrn);
        }
        private static void AssertLegalRequirementsAreCorrect(TramsProject toMap, Project result)
        {
            Assert.Equal(toMap.LegalRequirements.TrustAgreement, result.LegalRequirements.TrustAgreement.ToDescription());
            Assert.Equal(toMap.LegalRequirements.DiocesanConsent, result.LegalRequirements.DiocesanConsent.ToDescription());
            Assert.Equal(toMap.LegalRequirements.FoundationConsent, result.LegalRequirements.FoundationConsent.ToDescription());
            Assert.Equal(toMap.LegalRequirements.IsCompleted, result.LegalRequirements.IsCompleted);
        }
        private static void AssertRationaleCorrect(TramsProject toMap, Project result)
        {
            Assert.Equal(toMap.Rationale.ProjectRationale, result.Rationale.Project);
            Assert.Equal(toMap.Rationale.TrustSponsorRationale, result.Rationale.Trust);
            Assert.Equal(toMap.Rationale.IsCompleted, result.Rationale.IsCompleted);
        }

        private static void AssertFeaturesCorrect(TramsProject toMap, Project result)
        {
            var expectedType = EnumHelpers<TransferFeatures.TransferTypes>.Parse(toMap.Features.TypeOfTransfer);
            var expectedInitiator =
                EnumHelpers<TransferFeatures.ReasonForTheTransferTypes>.Parse(toMap.Features.WhoInitiatedTheTransfer);
            Assert.Equal(expectedType, result.Features.TypeOfTransfer);
            Assert.Equal(toMap.Features.OtherTransferTypeDescription, result.Features.OtherTypeOfTransfer);
            Assert.Equal(expectedInitiator, result.Features.ReasonForTheTransfer);
            Assert.Equal(toMap.Features.IsCompleted, result.Features.IsCompleted);
        }

        private static void AssertDatesCorrect(TramsProject toMap, Project result)
        {
            Assert.Equal(toMap.Dates.HtbDate, result.Dates.Htb);
            Assert.Equal(toMap.Dates.TargetDateForTransfer, result.Dates.Target);
            Assert.Equal(toMap.Dates.HasHtbDate, result.Dates.HasHtbDate);
            Assert.Equal(toMap.Dates.HasTargetDateForTransfer, result.Dates.HasTargetDateForTransfer);
        }

        private static void AssertBenefitsCorrect(TramsProject toMap, Project result)
        {
            var expectedBenefitsList = toMap.Benefits.IntendedTransferBenefits.SelectedBenefits
                .Select(EnumHelpers<TransferBenefits.IntendedBenefit>.Parse)
                .ToList();
            Assert.Equal(expectedBenefitsList, result.Benefits.IntendedBenefits);
            Assert.Equal(toMap.Benefits.IntendedTransferBenefits.OtherBenefitValue,
                result.Benefits.OtherIntendedBenefit);
            Assert.Equal(toMap.Benefits.OtherFactorsToConsider.HighProfile.FurtherSpecification,
                result.Benefits.OtherFactors[TransferBenefits.OtherFactor.HighProfile]);
            Assert.Equal(toMap.Benefits.OtherFactorsToConsider.FinanceAndDebt.FurtherSpecification,
                result.Benefits.OtherFactors[TransferBenefits.OtherFactor.FinanceAndDebtConcerns]);
            Assert.Equal(toMap.Benefits.OtherFactorsToConsider.ComplexLandAndBuilding.FurtherSpecification,
                result.Benefits.OtherFactors[TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues]);
            Assert.Equal(toMap.Benefits.IsCompleted, result.Benefits.IsCompleted);
        }
    }
}