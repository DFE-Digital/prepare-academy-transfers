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
                TrustSponsorRationale = input.Rationale.Trust
            };
        }

        private static AcademyTransferProjectFeatures Features(Project input)
        {
            return new AcademyTransferProjectFeatures
            {
                TypeOfTransfer = input.Features.TypeOfTransfer.ToString(),
                OtherTransferTypeDescription = input.Features.OtherTypeOfTransfer,
                RddOrEsfaIntervention = input.Features.ReasonForTransfer.IsSubjectToRddOrEsfaIntervention,
                WhoInitiatedTheTransfer = input.Features.WhoInitiatedTheTransfer.ToString(),
                RddOrEsfaInterventionDetail = input.Features.ReasonForTransfer.InterventionDetails
            };
        }

        private static AcademyTransferProjectDates Dates(Project input)
        {
            return new AcademyTransferProjectDates
            {
                HtbDate = input.Dates.Htb,
                TransferFirstDiscussed = input.Dates.FirstDiscussed,
                TargetDateForTransfer = input.Dates.Target
            };
        }

        private static List<TransferringAcademyUpdate> TransferringAcademies(Project input)
        {
            return input.TransferringAcademies.Select(transferringAcademy =>
                new TransferringAcademyUpdate
                {
                    IncomingTrustUkprn = transferringAcademy.IncomingTrustUkprn,
                    OutgoingAcademyUkprn = transferringAcademy.OutgoingAcademyUkprn
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
                    }
                }
            };
        }
    }
}