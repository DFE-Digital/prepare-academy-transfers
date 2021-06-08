using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.Models.Projects;
using Data.TRAMS.Models;
using Data.TRAMS.Models.AcademyTransferProject;
using Helpers;

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
                TransferringAcademies = input.TransferringAcademies.Select(transferringAcademy =>
                    new TransferringAcademyUpdate
                    {
                        IncomingTrustUkprn = transferringAcademy.IncomingTrustUkprn,
                        OutgoingAcademyUkprn = transferringAcademy.OutgoingAcademyUkprn
                    }).ToList(),
                Benefits = new AcademyTransferProjectBenefits()
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
                }
            };
        }
    }
}