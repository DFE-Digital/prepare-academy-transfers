using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Data.TRAMS.ExtensionMethods;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Models.AcademyTransferProject;

namespace Dfe.PrepareTransfers.Data.TRAMS.Mappers.Request
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
                TransferringAcademies = TransferringAcademies (input),
                Benefits = Benefits(input),
                LegalRequirements = LegalRequirements(input),
                Dates = Dates(input),
                Features = Features(input),
                Rationale = Rationale(input),
                GeneralInformation = GeneralInformation(input),
                AssignedUser = input.AssignedUser
            };
        }

        public static AcademyTransferProjectAcademyAndTrustInformation GeneralInformation(Project input)
        {
            return new AcademyTransferProjectAcademyAndTrustInformation
            {
                Author = input.AcademyAndTrustInformation.Author,
                Recommendation = input.AcademyAndTrustInformation.Recommendation.ToString()
            };
        }

        public static TransferProjectCreate MapToCreate(Project input)
        {
            return new TransferProjectCreate
            {
                OutgoingTrustUkprn = input.OutgoingTrustUkprn,
                IncomingTrustUkprn = input.IncomingTrustUkprn,
                TransferringAcademyUkprns = input.TransferringAcademies.Select(x => x.OutgoingAcademyUkprn).ToList()
            };
        }

        public static AcademyTransferProjectRationale Rationale(Project input)
        {
            return new AcademyTransferProjectRationale
            {
                ProjectRationale = input.Rationale.Project ?? string.Empty,
                TrustSponsorRationale = input.Rationale.Trust ?? string.Empty,
                IsCompleted = input.Rationale.IsCompleted
            };
        }

        public static AcademyTransferProjectFeatures Features(Project input)
        {
            return new AcademyTransferProjectFeatures
            {
                TypeOfTransfer = input.Features.TypeOfTransfer.ToString(),
                OtherTransferTypeDescription = input.Features.OtherTypeOfTransfer,
                WhoInitiatedTheTransfer = input.Features.ReasonForTheTransfer.ToString(),
                IsCompleted = input.Features.IsCompleted
            };
        }

        public static AcademyTransferProjectDates Dates(Project input)
        {
            return new AcademyTransferProjectDates
            {
                HtbDate = input.Dates.HasHtbDate != false && input.Dates.Htb != null ? DateTime.ParseExact(input.Dates.Htb, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("u") : null,
                HasHtbDate = input.Dates.HasHtbDate,
                TargetDateForTransfer = input.Dates.HasTargetDateForTransfer != false && input.Dates.Target != null ? DateTime.ParseExact(input.Dates.Target, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("u") : null,
                HasTargetDateForTransfer = input.Dates.HasTargetDateForTransfer
            };
        }

        public static AcademyTransferProjectLegalRequirements LegalRequirements(Project input)
        {
            return new AcademyTransferProjectLegalRequirements()
            {
                IncomingTrustAgreement = input.LegalRequirements.IncomingTrustAgreement.ToDescription(),
                DiocesanConsent = input.LegalRequirements.DiocesanConsent.ToDescription(),
                OutgoingTrustConsent = input.LegalRequirements.OutgoingTrustConsent.ToDescription(),
                IsCompleted = input.LegalRequirements.IsCompleted
            };
        }
        private static List<TransferringAcademyUpdate> TransferringAcademies(Project input)
        {
            return input.TransferringAcademies.Select(transferringAcademy =>
                new TransferringAcademyUpdate
                {
                    //IncomingTrustUkprn = transferringAcademy.IncomingTrustUkprn,
                    TransferringAcademyUkprn = transferringAcademy.OutgoingAcademyUkprn,
                    PupilNumbersAdditionalInformation = transferringAcademy.PupilNumbersAdditionalInformation,
                    LatestOfstedReportAdditionalInformation = transferringAcademy.LatestOfstedReportAdditionalInformation,
                    KeyStage2PerformanceAdditionalInformation = transferringAcademy.KeyStage2PerformanceAdditionalInformation,
                    KeyStage4PerformanceAdditionalInformation = transferringAcademy.KeyStage4PerformanceAdditionalInformation,
                    KeyStage5PerformanceAdditionalInformation = transferringAcademy.KeyStage5PerformanceAdditionalInformation
                }).ToList();
        }

        public static TransferringAcademyUpdate TransferringAcademy(TransferringAcademies input)
        {
            return new TransferringAcademyUpdate
                {
                    //IncomingTrustUkprn = input.IncomingTrustUkprn,
                    TransferringAcademyUkprn = input.OutgoingAcademyUkprn,
                    PupilNumbersAdditionalInformation = input.PupilNumbersAdditionalInformation ?? string.Empty,
                    LatestOfstedReportAdditionalInformation = input.LatestOfstedReportAdditionalInformation ?? string.Empty,
                    KeyStage2PerformanceAdditionalInformation = input.KeyStage2PerformanceAdditionalInformation ?? string.Empty,
                    KeyStage4PerformanceAdditionalInformation = input.KeyStage4PerformanceAdditionalInformation ?? string.Empty,
                    KeyStage5PerformanceAdditionalInformation = input.KeyStage5PerformanceAdditionalInformation ?? string.Empty
                };
        }

        public static AcademyTransferProjectBenefits Benefits(Project input)
        {
            return new AcademyTransferProjectBenefits
            {
                IntendedTransferBenefits = new IntendedTransferBenefits
                {
                    SelectedBenefits = input.Benefits.IntendedBenefits
                        .Select(benefit => benefit.ToString())
                        .ToList(),
                    OtherBenefitValue = input.Benefits.OtherIntendedBenefit ?? string.Empty,
                },
                OtherFactorsToConsider = new OtherFactorsToConsider
                {
                    HighProfile = new OtherFactor
                    {
                        ShouldBeConsidered =
                            input.Benefits.OtherFactors.ContainsKey(TransferBenefits.OtherFactor.HighProfile),
                        FurtherSpecification =
                            input.Benefits.OtherFactors.GetValueOrDefault(TransferBenefits.OtherFactor.HighProfile,
                                "") ?? string.Empty,
                    },
                    FinanceAndDebt = new OtherFactor
                    {
                        ShouldBeConsidered =
                            input.Benefits.OtherFactors.ContainsKey(TransferBenefits.OtherFactor
                                .FinanceAndDebtConcerns),
                        FurtherSpecification =
                            input.Benefits.OtherFactors.GetValueOrDefault(
                                TransferBenefits.OtherFactor.FinanceAndDebtConcerns, "") ?? string.Empty
                    },
                    ComplexLandAndBuilding = new OtherFactor
                    {
                        ShouldBeConsidered =
                            input.Benefits.OtherFactors.ContainsKey(TransferBenefits.OtherFactor
                                .ComplexLandAndBuildingIssues),
                        FurtherSpecification =
                            input.Benefits.OtherFactors.GetValueOrDefault(
                                TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues, "") ?? string.Empty
                    },
                    OtherRisks = new OtherFactor
                    {
                        ShouldBeConsidered = input.Benefits.OtherFactors.ContainsKey(TransferBenefits.OtherFactor
                            .OtherRisks),
                        FurtherSpecification =
                            input.Benefits.OtherFactors.GetValueOrDefault(
                                TransferBenefits.OtherFactor.OtherRisks, "") ?? string.Empty
                    }
                },
                IsCompleted = input.Benefits.IsCompleted,
                AnyRisks = input.Benefits.AnyRisks,
                EqualitiesImpactAssessmentConsidered = input.Benefits.EqualitiesImpactAssessmentConsidered
            };
        }
    }
}