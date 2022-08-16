using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.Models.Projects;
using Data.TRAMS.Models;
using Helpers;

namespace Data.TRAMS.Mappers.Response
{
    public class TramsProjectMapper : IMapper<TramsProject, Project>
    {
        public Project Map(TramsProject input)
        {
            return new Project
            {
                Benefits = Benefits(input),
                Dates = Dates(input),
                Features = Features(input),
                Rationale = Rationale(input),
                AcademyAndTrustInformation = AcademyAndTrustInformation(input),
                State = input.State,
                Status = input.Status,
                Urn = input.ProjectUrn,
                Reference = input.ProjectReference,
                TransferringAcademies = TransferringAcademies(input),
                OutgoingTrustName = input.OutgoingTrust.GroupName,
                OutgoingTrustUkprn = input.OutgoingTrust.Ukprn,
            };
        }

        private static TransferAcademyAndTrustInformation AcademyAndTrustInformation(TramsProject input)
        {
            return new TransferAcademyAndTrustInformation
            {
                Author = input.GeneralInformation.Author,
                Recommendation = EnumHelpers<TransferAcademyAndTrustInformation.RecommendationResult>.Parse(input.GeneralInformation.Recommendation) 
            };
        }

        private static List<TransferringAcademies> TransferringAcademies(TramsProject input)
        {
            return input.TransferringAcademies
                .Select(transfer =>
                    new TransferringAcademies
                    {
                        IncomingTrustName = transfer.IncomingTrust.GroupName,
                        IncomingTrustUkprn = transfer.IncomingTrust.Ukprn,
                        IncomingTrustLeadRscRegion = transfer.IncomingTrust.LeadRscRegion,
                        OutgoingAcademyName = transfer.OutgoingAcademy.Name,
                        OutgoingAcademyUkprn = transfer.OutgoingAcademy.Ukprn,
                        OutgoingAcademyUrn = transfer.OutgoingAcademy.Urn,
                        PupilNumbersAdditionalInformation = transfer.PupilNumbersAdditionalInformation,
                        LatestOfstedReportAdditionalInformation = transfer.LatestOfstedReportAdditionalInformation,
                        KeyStage2PerformanceAdditionalInformation = transfer.KeyStage2PerformanceAdditionalInformation,
                        KeyStage4PerformanceAdditionalInformation = transfer.KeyStage4PerformanceAdditionalInformation,
                        KeyStage5PerformanceAdditionalInformation = transfer.KeyStage5PerformanceAdditionalInformation
                    }
                )
                .ToList();
        }

        private static TransferRationale Rationale(TramsProject input)
        {
            return new TransferRationale
            {
                Project = input.Rationale.ProjectRationale,
                Trust = input.Rationale.TrustSponsorRationale,
                IsCompleted = input.Rationale.IsCompleted
            };
        }

        private static TransferFeatures Features(TramsProject input)
        {
            return new TransferFeatures
            {
                TypeOfTransfer = EnumHelpers<TransferFeatures.TransferTypes>.Parse(input.Features.TypeOfTransfer),
                OtherTypeOfTransfer = input.Features.OtherTransferTypeDescription,
                ReasonForTheTransfer =
                    EnumHelpers<TransferFeatures.ReasonForTheTransferTypes>.Parse(input.Features.WhoInitiatedTheTransfer),
                IsCompleted = input.Features.IsCompleted
            };
        }

        private static TransferDates Dates(TramsProject input)
        {
            return new TransferDates
            {
                Htb = input.Dates.HtbDate,
                Target = input.Dates.TargetDateForTransfer,
                HasHtbDate = input.Dates.HasHtbDate,
                HasTargetDateForTransfer = input.Dates.HasTargetDateForTransfer
            };
        }

        private static TransferBenefits Benefits(TramsProject input)
        {
            var otherFactors = new Dictionary<TransferBenefits.OtherFactor, string>();
            var inputFactors = input.Benefits.OtherFactorsToConsider;
            if (inputFactors.HighProfile.ShouldBeConsidered != null &&
                (bool) inputFactors.HighProfile.ShouldBeConsidered)
            {
                otherFactors[TransferBenefits.OtherFactor.HighProfile] = inputFactors.HighProfile.FurtherSpecification;
            }

            if (inputFactors.FinanceAndDebt.ShouldBeConsidered != null &&
                (bool) inputFactors.FinanceAndDebt.ShouldBeConsidered)
            {
                otherFactors[TransferBenefits.OtherFactor.FinanceAndDebtConcerns] =
                    inputFactors.FinanceAndDebt.FurtherSpecification;
            }

            if (inputFactors.ComplexLandAndBuilding.ShouldBeConsidered != null &&
                (bool) inputFactors.ComplexLandAndBuilding.ShouldBeConsidered)
            {
                otherFactors[TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues] =
                    inputFactors.ComplexLandAndBuilding.FurtherSpecification;
            }
            
            if (inputFactors.OtherRisks.ShouldBeConsidered != null &&
                (bool) inputFactors.OtherRisks.ShouldBeConsidered)
            {
                otherFactors[TransferBenefits.OtherFactor.OtherRisks] =
                    inputFactors.OtherRisks.FurtherSpecification;
            }

            return new TransferBenefits
            {
                IntendedBenefits = input.Benefits.IntendedTransferBenefits.SelectedBenefits
                    .Select(EnumHelpers<TransferBenefits.IntendedBenefit>.Parse)
                    .ToList(),
                OtherIntendedBenefit = input.Benefits.IntendedTransferBenefits.OtherBenefitValue,
                OtherFactors = otherFactors,
                AnyRisks = input.Benefits.AnyRisks,
                EqualitiesImpactAssessmentConsidered = input.Benefits.EqualitiesImpactAssessmentConsidered,
                IsCompleted = input.Benefits.IsCompleted
            };
        }
    }
}