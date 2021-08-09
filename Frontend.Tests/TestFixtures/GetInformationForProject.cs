using System.Collections.Generic;
using Data.Models;
using Data.Models.Academies;
using Data.Models.KeyStagePerformance;
using Data.Models.Projects;
using Frontend.Services.Responses;

namespace Frontend.Tests.TestFixtures
{
    public static class GetInformationForProject
    {
        public static GetInformationForProjectResponse GetTestInformationForProject(string projectUrn)
        {
            var foundProject = new Project
            {
                Urn = projectUrn,
                AcademyAndTrustInformation = new TransferAcademyAndTrustInformation
                {
                    Author = "author",
                    Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Defer
                },
                Name = "test name",
                OutgoingTrustName = "trust name",
                OutgoingTrustUkprn = "trust ref number",
                Rationale = new TransferRationale {Project = "project", Trust = "trust"},
                Dates = new TransferDates {Htb = "01/01/2020", FirstDiscussed = "01/01/2020", Target = "01/01/2020"},
                Features = new TransferFeatures
                {
                    WhoInitiatedTheTransfer = TransferFeatures.ProjectInitiators.Dfe,
                    ReasonForTransfer = new ReasonForTransfer
                    {
                        IsSubjectToRddOrEsfaIntervention = true,
                        InterventionDetails = "intervention"
                    },
                    TypeOfTransfer = TransferFeatures.TransferTypes.SatClosure
                },
                Benefits = new TransferBenefits
                {
                    IntendedBenefits = new List<TransferBenefits.IntendedBenefit>
                    {
                        TransferBenefits.IntendedBenefit.StrengtheningGovernance,
                        TransferBenefits.IntendedBenefit.StrongerLeadership
                    },
                    OtherFactors = new Dictionary<TransferBenefits.OtherFactor, string>
                    {
                        {TransferBenefits.OtherFactor.HighProfile, "testHighProfile"},
                        {TransferBenefits.OtherFactor.FinanceAndDebtConcerns, "debtConcerns"},
                    }
                },
                PupilNumbersAdditionalInformation = "pupil numbers additional info",
                KeyStage2PerformanceAdditionalInformation = "ks2 additional info",
                KeyStage4PerformanceAdditionalInformation = "ks4 additional info",
                KeyStage5PerformanceAdditionalInformation = "ks5 additional info",
                LatestOfstedJudgementAdditionalInformation = "ofsted additional info",
                TransferringAcademies = new List<TransferringAcademies>
                    {new TransferringAcademies {IncomingTrustName = "incoming trust name"}}
            };

            var foundAcademy = new Academy
            {
                Name = "academy Name",
                Urn = "academy urn",
                EstablishmentType = "establishment type",
                GeneralInformation = new GeneralInformation
                {
                    SchoolPhase = "school phase",
                    AgeRange = "age range",
                    Capacity = "capacity",
                    Pan = "pan",
                    NumberOnRoll = "100",
                    PercentageFull = "100",
                    ViabilityIssue = "viability issues",
                    Pfi = "pfi",
                    Deficit = "deficit",
                    DiocesesPercent = "100",
                    DistanceToSponsorHq = "distance",
                    MpAndParty = "mp",
                    SchoolType = "school type"
                },
                PupilNumbers = new PupilNumbers
                {
                    EligibleForFreeSchoolMeals = "fsm",
                    BoysOnRoll = "50",
                    GirlsOnRoll = "45",
                    WithStatementOfSen = "sen",
                    WhoseFirstLanguageIsNotEnglish = "lan not english"
                },
                LatestOfstedJudgement = new LatestOfstedJudgement
                {
                    InspectionDate = "01/01/2020",
                    OverallEffectiveness = "overall effectiveness",
                    OfstedReport = "ofsted report"
                },
                LocalAuthorityName = "LA Name"
            };

            var foundEducationPerformance = new EducationPerformance
            {
                KeyStage2Performance = new List<KeyStage2> {new KeyStage2 {Year = "2017-2018"}},
                KeyStage4Performance = new List<KeyStage4> {new KeyStage4 {Year = "2017-2018"}},
                KeyStage5Performance = new List<KeyStage5> {new KeyStage5 {Year = "2017-2018"}}
            };

            return new GetInformationForProjectResponse
            {
                Project = foundProject,
                OutgoingAcademy = foundAcademy,
                EducationPerformance = foundEducationPerformance
            };
        }
    }
}