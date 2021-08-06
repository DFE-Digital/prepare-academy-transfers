using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Helpers;

namespace Frontend.Services
{
    public class GetHtbDocumentForProject : IGetHtbDocumentForProject
    {
        private readonly IGetInformationForProject _getInformationForProject;
        
        public GetHtbDocumentForProject(IGetInformationForProject getInformationForProject )
        {
            _getInformationForProject = getInformationForProject;
        }
        
        public async Task<GetHtbDocumentForProjectResponse> Execute(string projectUrn)
        {
            var informationForProjectResult = await _getInformationForProject.Execute(projectUrn);
            if (!informationForProjectResult.IsValid)
            {
                return CreateErrorResponse(informationForProjectResult.ResponseError);
            }

            var project = informationForProjectResult.Project;
            var academy = informationForProjectResult.OutgoingAcademy;
            var educationPerformance = informationForProjectResult.EducationPerformance;
            
            var htbDocument = new HtbDocument
            {
                Recommendation = EnumHelpers<TransferAcademyAndTrustInformation.RecommendationResult>.GetDisplayValue(project.AcademyAndTrustInformation.Recommendation),
                Author = project.AcademyAndTrustInformation.Author,
                ProjectName = project.Name,
                SponsorName = project.IncomingTrustName,
                AcademyTypeAndRoute = academy.EstablishmentType,
                SchoolName = academy.Name,
                SchoolUrn =  academy.Urn,
                TrustName = project.OutgoingTrustName,
                TrustReferenceNumber = project.OutgoingTrustUkprn,
                SchoolType = academy.EstablishmentType,
                SchoolPhase = academy.GeneralInformation.SchoolPhase,
                AgeRange = academy.GeneralInformation.AgeRange,
                SchoolCapacity = academy.GeneralInformation.Capacity,
                PublishedAdmissionNumber = academy.GeneralInformation.Pan,
                NumberOnRoll = $"{academy.GeneralInformation.NumberOnRoll} ({academy.GeneralInformation.PercentageFull}%) ",
                PercentageSchoolFull = academy.GeneralInformation.PercentageFull,
                PercentageFreeSchoolMeals = academy.PupilNumbers.EligibleForFreeSchoolMeals,
                OfstedLastInspection = DatesHelper.DateStringToGovUkDate(academy.LatestOfstedJudgement.InspectionDate),
                OverallEffectiveness = academy.LatestOfstedJudgement.OverallEffectiveness,
                RationaleForProject = project.Rationale.Project,
                RationaleForTrust = project.Rationale.Trust,
                ClearedBy = "Cleared by",
                Version = "Version",
                DateOfHtb = DatesHelper.DateStringToGovUkDate(project.Dates.Htb),
                DateOfProposedTransfer = DatesHelper.DateStringToGovUkDate(project.Dates.Target),
                DateTransferWasFirstDiscussed = DatesHelper.DateStringToGovUkDate(project.Dates.FirstDiscussed),
                ViabilityIssues = academy.GeneralInformation.ViabilityIssue,
                FinancialDeficit = academy.GeneralInformation.Pfi,
                Pfi = academy.GeneralInformation.Pfi,
                PercentageGoodOrOutstandingInDiocesanTrust = academy.GeneralInformation.DiocesesPercent,
                DistanceFromTheAcademyToTheTrustHeadquarters = academy.GeneralInformation.DistanceToSponsorHq,
                MpAndParty = academy.GeneralInformation.MpAndParty,
                WhoInitiatedTheTransfer = EnumHelpers<TransferFeatures.ProjectInitiators>.GetDisplayValue(project.Features.WhoInitiatedTheTransfer),
                ReasonForTransfer = project.Features.IsTransferSubjectToIntervention ? "Subject to Intervention" : "Not subject to intervention",
                MoreDetailsAboutTheTransfer = project.Features.ReasonForTransfer.InterventionDetails,
                TypeOfTransfer = project.Features.TypeOfTransfer == TransferFeatures.TransferTypes.Empty ? project.Features.OtherTypeOfTransfer : 
                    EnumHelpers<TransferFeatures.TransferTypes>.GetDisplayValue(project.Features.TypeOfTransfer),
                TransferBenefits = GetTransferBenefits(project.Benefits),
                OtherFactors = GetOtherFactors(project.Benefits),
                GirlsOnRoll = academy.PupilNumbers.GirlsOnRoll,
                BoysOnRoll = academy.PupilNumbers.BoysOnRoll,
                PupilsWithSen = academy.PupilNumbers.WithStatementOfSen,
                PupilsWithFirstLanguageNotEnglish = academy.PupilNumbers.WhoseFirstLanguageIsNotEnglish,
                PupilsFsm6Years = academy.PupilNumbers.EligibleForFreeSchoolMeals,
                PupilNumbersAdditionalInformation = project.PupilNumbersAdditionalInformation,
                OfstedReport = academy.LatestOfstedJudgement.OfstedReport,
                OfstedAdditionalInformation = project.LatestOfstedJudgementAdditionalInformation,
                KeyStage2Performance = educationPerformance.KeyStage2Performance,
                KeyStage4Performance = educationPerformance.KeyStage4Performance,
                KeyStage5Performance = educationPerformance.KeyStage5Performance,
                KeyStage2AdditionalInformation = project.KeyStage2PerformanceAdditionalInformation,
                KeyStage4AdditionalInformation = project.KeyStage4PerformanceAdditionalInformation,
                KeyStage5AdditionalInformation = project.KeyStage5PerformanceAdditionalInformation,
                LocalAuthorityName = academy.LocalAuthorityName
            };

            return new GetHtbDocumentForProjectResponse
            {
                HtbDocument = htbDocument
            };
        }
        
        private static GetHtbDocumentForProjectResponse CreateErrorResponse(
            ServiceResponseError serviceResponseError)
        {
            if (serviceResponseError.ErrorCode == ErrorCode.NotFound)
            {
                return new GetHtbDocumentForProjectResponse()
                {
                    ResponseError = new ServiceResponseError
                    {
                        ErrorCode = ErrorCode.NotFound,
                        ErrorMessage = "Not found"
                    }
                };
            }

            return new GetHtbDocumentForProjectResponse
            {
                ResponseError = new ServiceResponseError
                {
                    ErrorCode = ErrorCode.ApiError,
                    ErrorMessage = "API has encountered an error"
                }
            };
        }
        
        private static string GetOtherFactors(TransferBenefits transferBenefits)
        {
            var otherFactorsSummary = transferBenefits.OtherFactors.Select(otherFactor => new[]
            {
                EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(otherFactor.Key),
                otherFactor.Value
            }).ToList();

            var sb = new StringBuilder();
            foreach (var otherFactor in otherFactorsSummary)
            {
                sb.Append($"{otherFactor[0]}\n");
                if (!string.IsNullOrEmpty(otherFactor[1]))
                    sb.Append($"{otherFactor[1]}\n");
            }

            return sb.ToString();
        }

        private static string GetTransferBenefits(TransferBenefits transferBenefits)
        {
            var benefitSummary = transferBenefits.IntendedBenefits
                .FindAll(EnumHelpers<TransferBenefits.IntendedBenefit>.HasDisplayValue)
                .Select(EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayValue)
                .ToList();

            if (transferBenefits.IntendedBenefits.Contains(TransferBenefits.IntendedBenefit.Other))
            {
                benefitSummary.Add($"Other: {transferBenefits.OtherIntendedBenefit}");
            }

            var sb = new StringBuilder();
            foreach (var benefit in benefitSummary)
            {
                sb.Append($"{benefit}\n");
            }

            return sb.ToString();
        }
    }
}