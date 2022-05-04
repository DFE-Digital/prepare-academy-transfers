using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Data.Models.Projects;
using Frontend.Helpers;
using Frontend.Models.ProjectTemplate;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Helpers;

namespace Frontend.Services
{
    public class GetProjectTemplateModel : IGetProjectTemplateModel
    {
        private readonly IGetInformationForProject _getInformationForProject;

        public GetProjectTemplateModel(IGetInformationForProject getInformationForProject)
        {
            _getInformationForProject = getInformationForProject;
        }

        public async Task<GetProjectTemplateModelResponse> Execute(string projectUrn)
        {
            var informationForProjectResult = await _getInformationForProject.Execute(projectUrn);
            var project = informationForProjectResult.Project;
            var academies = informationForProjectResult.OutgoingAcademies;
            var projectTemplateModel = new ProjectTemplateModel
            {
                Recommendation =
                    EnumHelpers<TransferAcademyAndTrustInformation.RecommendationResult>.GetDisplayValue(
                        project.AcademyAndTrustInformation.Recommendation),
                Author = project.AcademyAndTrustInformation.Author,
                ProjectName = project.IncomingTrustName,
                ProjectReference = project.Reference,
                IncomingTrustName = project.IncomingTrustName,
                IncomingTrustUkprn = project.IncomingTrustUkprn,
                RationaleForProject = project.Rationale.Project,
                RationaleForTrust = project.Rationale.Trust,
                ClearedBy = "",
                Version = System.DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentUICulture),
                DateOfHtb = DatesHelper.FormatDateString(project.Dates.Htb, project.Dates.HasHtbDate),
                DateOfProposedTransfer =
                    DatesHelper.FormatDateString(project.Dates.Target, project.Dates.HasTargetDateForTransfer),
                DateTransferWasFirstDiscussed = DatesHelper.FormatDateString(project.Dates.FirstDiscussed,
                    project.Dates.HasFirstDiscussedDate),

                WhoInitiatedTheTransfer =
                    EnumHelpers<TransferFeatures.ReasonForTheTransferTypes>.GetDisplayValue(project.Features
                        .ReasonForTheTransfer),
                TypeOfTransfer = project.Features.TypeOfTransfer == TransferFeatures.TransferTypes.Other
                    ? $"Other: {project.Features.OtherTypeOfTransfer}"
                    : EnumHelpers<TransferFeatures.TransferTypes>.GetDisplayValue(project.Features.TypeOfTransfer),
                TransferBenefits = GetTransferBenefits(project.Benefits),
                AnyRisks = project.Benefits.AnyRisks.HasValue
                    ? project.Benefits.AnyRisks == true ? "Yes" : "No"
                    : string.Empty,
                OtherFactors = GetOtherFactors(project.Benefits),
                Academies = GetAcademyData(academies)
            };

            return new GetProjectTemplateModelResponse
            {
                ProjectTemplateModel = projectTemplateModel
            };
        }

        private List<ProjectTemplateAcademyModel> GetAcademyData(List<Academy> academies)
        {
            var academyModels = new List<ProjectTemplateAcademyModel>();
            foreach (var academy in academies)
            {
                var academyModel = new ProjectTemplateAcademyModel();
                academyModel.AcademyTypeAndRoute = academy.EstablishmentType;
                academyModel.SchoolName = academy.Name;
                academyModel.SchoolUrn = academy.Urn;
                academyModel.AcademyTypeAndRoute = academy.EstablishmentType;
                academyModel.SchoolType = academy.GeneralInformation.SchoolType;
                academyModel.SchoolPhase = academy.GeneralInformation.SchoolPhase;
                academyModel.AgeRange = academy.GeneralInformation.AgeRange;
                academyModel.SchoolCapacity = academy.GeneralInformation.Capacity;
                academyModel.PublishedAdmissionNumber = academy.GeneralInformation.Pan;
                academyModel.NumberOnRoll =
                    $"{academy.GeneralInformation.NumberOnRoll} ({academy.GeneralInformation.PercentageFull})";
                academyModel.PercentageFreeSchoolMeals = academy.GeneralInformation.PercentageFsm;
                academyModel.OfstedLastInspection =
                    DatesHelper.DateStringToGovUkDate(academy.LatestOfstedJudgement.InspectionDate);
                academyModel.OverallEffectiveness = academy.LatestOfstedJudgement.OverallEffectiveness;
                academyModel.ViabilityIssues = academy.GeneralInformation.ViabilityIssue;
                academyModel.FinancialDeficit = academy.GeneralInformation.Deficit;
                academyModel.Pfi = academy.GeneralInformation.Pfi;
                academyModel.PercentageGoodOrOutstandingInDiocesanTrust = academy.GeneralInformation.DiocesesPercent;
                academyModel.DistanceFromTheAcademyToTheTrustHeadquarters =
                    academy.GeneralInformation.DistanceToSponsorHq;
                academyModel.MpAndParty = academy.GeneralInformation.MpAndParty;
                academyModel.GirlsOnRoll = academy.PupilNumbers.GirlsOnRoll;
                academyModel.BoysOnRoll = academy.PupilNumbers.BoysOnRoll;
                academyModel.PupilsWithSen = academy.PupilNumbers.WithStatementOfSen;
                academyModel.PupilsWithFirstLanguageNotEnglish = academy.PupilNumbers.WhoseFirstLanguageIsNotEnglish;
                academyModel.PupilsFsm6Years =
                    academy.PupilNumbers.PercentageEligibleForFreeSchoolMealsDuringLast6Years;
                academyModel.PupilNumbersAdditionalInformation = academy.PupilNumbers.AdditionalInformation;
                academyModel.OfstedReport = academy.LatestOfstedJudgement.OfstedReport;
                academyModel.OfstedAdditionalInformation = academy.LatestOfstedJudgement.AdditionalInformation;
                academyModel.KeyStage2Performance = academy.EducationPerformance.KeyStage2Performance;
                academyModel.KeyStage4Performance =
                    PerformanceDataHelpers.GetFormattedKeyStage4Results(academy.EducationPerformance
                        .KeyStage4Performance);
                academyModel.KeyStage5Performance = academy.EducationPerformance.KeyStage5Performance;
                academyModel.KeyStage2AdditionalInformation =
                    academy.EducationPerformance.KeyStage2AdditionalInformation;
                academyModel.KeyStage4AdditionalInformation =
                    academy.EducationPerformance.KeyStage4AdditionalInformation;
                academyModel.KeyStage5AdditionalInformation =
                    academy.EducationPerformance.KeyStage5AdditionalInformation;
                academyModel.LocalAuthorityName = academy.LocalAuthorityName;
                academyModels.Add(academyModel);
            }

            return academyModels;
        }

        private static List<Tuple<string,string>> GetOtherFactors(TransferBenefits transferBenefits)
        {
           return transferBenefits.OtherFactors
                .OrderBy(o => (int) o.Key)
                .Select(otherFactor => new Tuple<string,string>(
                    EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(otherFactor.Key),
                    otherFactor.Value)).ToList();
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