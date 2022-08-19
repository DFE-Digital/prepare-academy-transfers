using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.Models.Projects;
using Data.TRAMS.Models;
using Data.TRAMS.Models.AcademyTransferProject;

namespace Data.TRAMS.Mappers.Request
{
    public class InternalProjectToUpdateMapper : IMapper<Project, TramsProjectUpdate>
    {
        public TramsProjectUpdate Map(Project input)
        {
            return new TramsProjectUpdate
            {
                OutgoingTrustUkprn = input.OutgoingTrustUkprn,
                State = input.State,
                Status = input.Status,
                ProjectUrn = input.Urn,
                ProjectReference = input.Reference,
                TransferringAcademies = TransferringAcademies(input),
                Benefits = Benefits(input),
                Dates = Dates(input),
                Features = Features(input),
                Rationale = Rationale(input),
                GeneralInformation = GeneralInformation(input)
            };
        }

        private AcademyTransferProjectAcademyAndTrustInformation GeneralInformation(Project input)
        {
            return new AcademyTransferProjectAcademyAndTrustInformation
            {
                Author = input.AcademyAndTrustInformation.Author,
                Recommendation = input.AcademyAndTrustInformation.Recommendation.ToString()
            };
        }

        private static AcademyTransferProjectRationale Rationale(Project input)
        {
            return new AcademyTransferProjectRationale
            {
                ProjectRationale = input.Rationale.Project,
                TrustSponsorRationale = input.Rationale.Trust,
                IsCompleted = input.Rationale.IsCompleted
            };
        }

        private static AcademyTransferProjectFeatures Features(Project input)
        {
            return new AcademyTransferProjectFeatures
            {
                TypeOfTransfer = input.Features.TypeOfTransfer.ToString(),
                OtherTransferTypeDescription = input.Features.OtherTypeOfTransfer,
                WhoInitiatedTheTransfer = input.Features.ReasonForTheTransfer.ToString(),
                IsCompleted = input.Features.IsCompleted
            };
        }

        private static AcademyTransferProjectDates Dates(Project input)
        {
            return new AcademyTransferProjectDates
            {
                HtbDate = input.Dates.HasHtbDate != false ? input.Dates.Htb : null,
                HasHtbDate = input.Dates.HasHtbDate,
                TargetDateForTransfer = input.Dates.HasTargetDateForTransfer != false ? input.Dates.Target : null,
                HasTargetDateForTransfer = input.Dates.HasTargetDateForTransfer
            };
        }

        private static List<TransferringAcademyUpdate> TransferringAcademies(Project input)
        {
            return input.TransferringAcademies.Select(transferringAcademy =>
                new TransferringAcademyUpdate
                {
                    IncomingTrustUkprn = transferringAcademy.IncomingTrustUkprn,
                    OutgoingAcademyUkprn = transferringAcademy.OutgoingAcademyUkprn,
                    PupilNumbersAdditionalInformation = transferringAcademy.PupilNumbersAdditionalInformation,
                    LatestOfstedReportAdditionalInformation = transferringAcademy.LatestOfstedReportAdditionalInformation,
                    KeyStage2PerformanceAdditionalInformation = transferringAcademy.KeyStage2PerformanceAdditionalInformation,
                    KeyStage4PerformanceAdditionalInformation = transferringAcademy.KeyStage4PerformanceAdditionalInformation,
                    KeyStage5PerformanceAdditionalInformation = transferringAcademy.KeyStage5PerformanceAdditionalInformation
                }).ToList();
        }

        private static AcademyTransferProjectBenefits Benefits(Project input)
        {
            return new AcademyTransferProjectBenefits
            {
                IntendedTransferBenefits = new IntendedTransferBenefits
                {
                    SelectedBenefits = input.Benefits.IntendedBenefits
                        .Select(benefit => benefit.ToString())
                        .ToList(),
                    OtherBenefitValue = input.Benefits.OtherIntendedBenefit
                },
                OtherFactorsToConsider = new OtherFactorsToConsider
                {
                    HighProfile = new OtherFactor
                    {
                        ShouldBeConsidered =
                            input.Benefits.OtherFactors.ContainsKey(TransferBenefits.OtherFactor.HighProfile),
                        FurtherSpecification =
                            input.Benefits.OtherFactors.GetValueOrDefault(TransferBenefits.OtherFactor.HighProfile,
                                "")
                    },
                    FinanceAndDebt = new OtherFactor
                    {
                        ShouldBeConsidered =
                            input.Benefits.OtherFactors.ContainsKey(TransferBenefits.OtherFactor
                                .FinanceAndDebtConcerns),
                        FurtherSpecification =
                            input.Benefits.OtherFactors.GetValueOrDefault(
                                TransferBenefits.OtherFactor.FinanceAndDebtConcerns, "")
                    },
                    ComplexLandAndBuilding = new OtherFactor
                    {
                        ShouldBeConsidered =
                            input.Benefits.OtherFactors.ContainsKey(TransferBenefits.OtherFactor
                                .ComplexLandAndBuildingIssues),
                        FurtherSpecification =
                            input.Benefits.OtherFactors.GetValueOrDefault(
                                TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues, "")
                    },
                    OtherRisks = new OtherFactor
                    {
                        ShouldBeConsidered = input.Benefits.OtherFactors.ContainsKey(TransferBenefits.OtherFactor
                            .OtherRisks),
                        FurtherSpecification =
                            input.Benefits.OtherFactors.GetValueOrDefault(
                                TransferBenefits.OtherFactor.OtherRisks, "")
                    }
                },
                IsCompleted = input.Benefits.IsCompleted,
                AnyRisks = input.Benefits.AnyRisks,
                EqualitiesImpactAssessmentConsidered = input.Benefits.EqualitiesImpactAssessmentConsidered
            };
        }
    }
}