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
                TransferringAcademies = TransferringAcademies(input),
                OutgoingTrustName = input.OutgoingTrust.GroupName,
                OutgoingTrustUkprn = input.OutgoingTrust.Ukprn,
                Name = input.TransferringAcademies[0]?.OutgoingAcademy?.Name
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
                        OutgoingAcademyName = transfer.OutgoingAcademy.Name,
                        OutgoingAcademyUkprn = transfer.OutgoingAcademy.Ukprn,
                        OutgoingAcademyUrn = transfer.OutgoingAcademy.Urn
                    }
                )
                .ToList();
        }

        private static TransferRationale Rationale(TramsProject input)
        {
            return new TransferRationale
            {
                Project = input.Rationale.ProjectRationale,
                Trust = input.Rationale.TrustSponsorRationale
            };
        }

        private static TransferFeatures Features(TramsProject input)
        {
            return new TransferFeatures
            {
                ReasonForTransfer = new ReasonForTransfer()
                {
                    IsSubjectToRddOrEsfaIntervention = input.Features.RddOrEsfaIntervention,
                    InterventionDetails = input.Features.RddOrEsfaInterventionDetail
                },
                TypeOfTransfer = EnumHelpers<TransferFeatures.TransferTypes>.Parse(input.Features.TypeOfTransfer),
                OtherTypeOfTransfer = input.Features.OtherTransferTypeDescription,
                WhoInitiatedTheTransfer =
                    EnumHelpers<TransferFeatures.ProjectInitiators>.Parse(input.Features.WhoInitiatedTheTransfer)
            };
        }

        private static TransferDates Dates(TramsProject input)
        {
            return new TransferDates
            {
                Htb = input.Dates.HtbDate,
                Target = input.Dates.TargetDateForTransfer,
                FirstDiscussed = input.Dates.TransferFirstDiscussed
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

            return new TransferBenefits
            {
                IntendedBenefits = input.Benefits.IntendedTransferBenefits.SelectedBenefits
                    .Select(EnumHelpers<TransferBenefits.IntendedBenefit>.Parse)
                    .ToList(),
                OtherIntendedBenefit = input.Benefits.IntendedTransferBenefits.OtherBenefitValue,
                OtherFactors = otherFactors
            };
        }
    }
}