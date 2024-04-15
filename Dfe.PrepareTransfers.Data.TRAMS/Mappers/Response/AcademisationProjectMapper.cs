using System;
using System.Collections.Generic;
using System.Linq;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Dfe.PrepareTransfers.Helpers;

namespace Dfe.PrepareTransfers.Data.TRAMS.Mappers.Response
{
    public class AcademisationProjectMapper : IMapper<AcademisationProject, Project>
    {
        public Project Map(AcademisationProject input)
        {
            return new Project
            {
                Id = input.Id,
                Benefits = Benefits(input),
                LegalRequirements = LegalRequirements(input),
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
                AssignedUser = input.AssignedUser,
                IsFormAMat = input.IsFormAMat
            };
        }

        private static TransferAcademyAndTrustInformation AcademyAndTrustInformation(AcademisationProject input)
        {
            return new TransferAcademyAndTrustInformation
            {
                Author = input.GeneralInformation.Author,
                Recommendation = EnumHelpers<TransferAcademyAndTrustInformation.RecommendationResult>.Parse(input.GeneralInformation.Recommendation) 
            };
        }

        private static List<TransferringAcademies> TransferringAcademies(AcademisationProject input)
        {
            return input.TransferringAcademies
                .Select(transfer =>
                    new TransferringAcademies
                    {
                        IncomingTrustName = string.IsNullOrEmpty(transfer.IncomingTrustName) ? transfer.IncomingTrust.GroupName : transfer.IncomingTrustName,
                        IncomingTrustUkprn = transfer.IncomingTrust.Ukprn,
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

        private static TransferRationale Rationale(AcademisationProject input)
        {
            return new TransferRationale
            {
                Project = input.Rationale.ProjectRationale,
                Trust = input.Rationale.TrustSponsorRationale,
                IsCompleted = input.Rationale.IsCompleted
            };
        }
        private static TransferLegalRequirements LegalRequirements(AcademisationProject input)
        {
            return new TransferLegalRequirements()
            {
                IncomingTrustAgreement = ToThreeOptions(input.LegalRequirements.IncomingTrustAgreement),
                DiocesanConsent = ToThreeOptions(input.LegalRequirements.DiocesanConsent),
                OutgoingTrustConsent = ToThreeOptions(input.LegalRequirements.OutgoingTrustConsent),
                IsCompleted = input.LegalRequirements.IsCompleted
            };
        }

        private static TransferFeatures Features(AcademisationProject input)
        {
            return new TransferFeatures
            {
                TypeOfTransfer = EnumHelpers<TransferFeatures.TransferTypes>.Parse(input.Features.TypeOfTransfer),
                OtherTypeOfTransfer = input.Features.OtherTransferTypeDescription,
                SpecificReasonsForTheTransfer = input.Features.SpecificReasonsForTransfer.Select(x => EnumHelpers<TransferFeatures.SpecificReasonForTheTransferTypes>.Parse(x)).ToList(),
                ReasonForTheTransfer =
                    EnumHelpers<TransferFeatures.ReasonForTheTransferTypes>.Parse(input.Features.WhoInitiatedTheTransfer),
                IsCompleted = input.Features.IsCompleted
            };
        }

        private static TransferDates Dates(AcademisationProject input)
        {
            return new TransferDates
            {
                Htb = input.Dates.HtbDate,
                Target = input.Dates.TargetDateForTransfer,
                HasHtbDate = input.Dates.HasHtbDate,
                HasTargetDateForTransfer = input.Dates.HasTargetDateForTransfer
            };
        }

        private static TransferBenefits Benefits(AcademisationProject input)
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
        private static ThreeOptions? ToThreeOptions(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            if (source == "Not applicable")
            {
                return ThreeOptions.NotApplicable;
            }

            ThreeOptions? status = (ThreeOptions)Enum.Parse(typeof(ThreeOptions), source, true);

            return status;
        }
    }
}