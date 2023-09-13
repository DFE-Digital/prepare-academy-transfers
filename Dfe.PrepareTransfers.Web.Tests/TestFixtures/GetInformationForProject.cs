using System.Collections.Generic;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Academies;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Services;
using Dfe.PrepareTransfers.Web.Services.Responses;


namespace Dfe.PrepareTransfers.Web.Tests.TestFixtures
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
                    Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Approve
                },
                OutgoingTrustName = "trust name",
                OutgoingTrustUkprn = "trust ref number",
                Rationale = new TransferRationale {Project = "project", Trust = "trust"},
                Dates = new TransferDates {Htb = "01/01/2020", Target = "01/01/2020"},
                Features = new TransferFeatures
                {
                    ReasonForTheTransfer = TransferFeatures.ReasonForTheTransferTypes.Dfe,
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
                        {TransferBenefits.OtherFactor.FinanceAndDebtConcerns, "debtConcerns"}
                    },
                    AnyRisks = true
                },
                LegalRequirements = new TransferLegalRequirements
                {
                    DiocesanConsent = ThreeOptions.Yes,
                    IncomingTrustAgreement = ThreeOptions.No,
                    OutgoingTrustConsent = ThreeOptions.NotApplicable,
                    
                },
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies
                    {
                        IncomingTrustName = "incoming trust name",
                        PupilNumbersAdditionalInformation = "pupil numbers additional info",
                        KeyStage2PerformanceAdditionalInformation = "ks2 additional info",
                        KeyStage4PerformanceAdditionalInformation = "ks4 additional info",
                        KeyStage5PerformanceAdditionalInformation = "ks5 additional info",
                        LatestOfstedReportAdditionalInformation = "ofsted additional info"
                    }
                }
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
                    PercentageFsm = "80",
                    PercentageFull = "100%",
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
                    PercentageEligibleForFreeSchoolMealsDuringLast6Years = "fsm",
                    BoysOnRoll = "50",
                    GirlsOnRoll = "45",
                    WithStatementOfSen = "13",
                    WhoseFirstLanguageIsNotEnglish = "lan not english"
                },
                LatestOfstedJudgement = new LatestOfstedJudgement
                {
                    InspectionEndDate = "01/01/2020",
                    OverallEffectiveness = "overall effectiveness",
                    OfstedReport = "ofsted report",
                    QualityOfEducation = "Outstanding",
                    BehaviourAndAttitudes = "Outstanding",
                    PersonalDevelopment = "Outstanding",
                    EffectivenessOfLeadershipAndManagement = "Outstanding",
                    EarlyYearsProvision = "Outstanding",
                    SixthFormProvision = "N/A",
                    DateOfLatestSection8Inspection = "01/01/2021",
                    
                },
                EducationPerformance = new EducationPerformance
                {
                    KeyStage2Performance = new List<KeyStage2> {new KeyStage2 {Year = "2017-2018"}},
                    KeyStage4Performance = new List<KeyStage4> {new KeyStage4 {Year = "2017-2018"}},
                    KeyStage5Performance = new List<KeyStage5> {new KeyStage5 {Year = "2017-2018"}}
                },
                LocalAuthorityName = "LA Name"
            };
            
            return new GetInformationForProjectResponse
            {
                Project = foundProject,
                OutgoingAcademies = new List<Academy>
                {
                    foundAcademy
                }
            };
        }
    }
}