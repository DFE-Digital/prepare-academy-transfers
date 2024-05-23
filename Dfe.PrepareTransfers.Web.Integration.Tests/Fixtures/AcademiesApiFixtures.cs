using System;
using System.Collections.Generic;
using Dfe.Academies.Contracts.V4;
using Dfe.Academies.Contracts.V4.Trusts;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Data.TRAMS.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Models.AcademyTransferProject;
using Dfe.PrepareTransfers.Data.TRAMS.Models.EducationPerformance;

namespace Dfe.PrepareTransfers.Web.Integration.Tests.Fixtures
{
    public class AcademiesApiFixtures
    {
        private static readonly Random RandomGenerator = new Random();
        private const string ProjectUrn = "001";
        private const string OutgoingAcademyUkprn = "AcademyUkprn";
        private const string OutgoingAcademyUrn = "AcademyUrn";
        private const string OutgoingAcademyName = "OutgoingAcademyName";
        private const string IncomingTrustUkprn = "IncomingTrustUkprn";
        private const string IncomingTrustName = "IncomingTrustName";
        private const string OutgoingTrustUkprn = "OutgoingTrustUkprn";
        private const string OutgoingTrustName = "OutgoingTrustName";

        public static List<TramsProjectSummary> Projects()
        {
            var tramsProjectSummary = new TramsProjectSummary
            {
                OutgoingTrustUkprn = OutgoingTrustUkprn,
                OutgoingTrustName = OutgoingTrustName,
                ProjectReference = "REF-SW-123456789",
                ProjectUrn = ProjectUrn,
                TransferringAcademies = new List<Data.TRAMS.Models.AcademyTransferProject.TransferringAcademy>
                {
                    new Data.TRAMS.Models.AcademyTransferProject.TransferringAcademy
                    {
                        IncomingTrustUkprn = IncomingTrustUkprn,
                        IncomingTrustName = IncomingTrustName,
                        OutgoingAcademyUkprn = OutgoingAcademyUkprn
                    }
                }
            };

            return new List<TramsProjectSummary> { tramsProjectSummary };
        }

        public static AcademisationProject Project()
        {
            var tramsProject = new AcademisationProject
            {
                Benefits = new AcademyTransferProjectBenefits
                {
                    IntendedTransferBenefits = new IntendedTransferBenefits
                    {
                        SelectedBenefits = new List<string>
                        {
                            TransferBenefits.IntendedBenefit.ImprovingSafeguarding.ToString(),
                            TransferBenefits.IntendedBenefit.Other.ToString()
                        },
                        OtherBenefitValue = "Other benefit value"
                    },
                    OtherFactorsToConsider = new OtherFactorsToConsider
                    {
                        HighProfile = new OtherFactor
                        {
                            FurtherSpecification = "High profile", ShouldBeConsidered = true},
                        FinanceAndDebt = new OtherFactor
                        {
                            FurtherSpecification = "Finance", ShouldBeConsidered = true},
                        ComplexLandAndBuilding = new OtherFactor
                        {
                            FurtherSpecification = "Complex land and building", ShouldBeConsidered = true},
                        OtherRisks = new OtherFactor
                        {
                            FurtherSpecification = "Other risks", ShouldBeConsidered = true
                        }
                    },
                    IsCompleted = true,
                    AnyRisks = true,
                    EqualitiesImpactAssessmentConsidered = true
                },
                Dates = new AcademyTransferProjectDates
                {
                    HtbDate = "01/01/2023",
                    HasHtbDate = true
                },
                Features = new AcademyTransferProjectFeatures
                {
                    TypeOfTransfer = TransferFeatures.TransferTypes.TrustsMerging.ToString(),
                    OtherTransferTypeDescription = "Other",
                    RddOrEsfaIntervention = true,
                    WhoInitiatedTheTransfer = TransferFeatures.ReasonForTheTransferTypes.Dfe.ToString(),
                    SpecificReasonsForTransfer = new List<string>() { TransferFeatures.SpecificReasonForTheTransferTypes.Safeguarding.ToString() },
                    RddOrEsfaInterventionDetail = "Intervention details",
                    IsCompleted = true
                },
                Rationale = new AcademyTransferProjectRationale
                {
                    ProjectRationale = "Project rationale",
                    TrustSponsorRationale = "Trust rationale",
                    IsCompleted = true
                },
                GeneralInformation = new AcademyTransferProjectAcademyAndTrustInformation
                {
                    Author = "Author",
                    Recommendation = TransferAcademyAndTrustInformation.RecommendationResult.Approve.ToString()
                },
                State = "State",
                Status = "Status",                
                OutgoingTrust = new TrustSummary
                {
                    GroupName = OutgoingTrustName,
                    Ukprn = OutgoingTrustUkprn
                },
                ProjectUrn = ProjectUrn,
                TransferringAcademies = new List<Data.TRAMS.Models.AcademyTransferProject.TransferringAcademy>
                {
                    new Data.TRAMS.Models.AcademyTransferProject.TransferringAcademy
                    {
                        IncomingTrustName = "Test Trust",
                        IncomingTrust = new TrustSummary
                        {
                            GroupName = IncomingTrustName,
                            Ukprn = IncomingTrustUkprn
                        },
                        OutgoingAcademy = new AcademySummary
                        {
                            Name = OutgoingAcademyName,
                            Urn = OutgoingAcademyUrn,
                            Ukprn = OutgoingAcademyUkprn
                        },
                        OutgoingAcademyUkprn = OutgoingAcademyUkprn,
                        PupilNumbersAdditionalInformation = "PupilNumbersAdditionalInformation",
                        LatestOfstedReportAdditionalInformation = "LatestOfstedJudgementAdditionalInformation",
                        KeyStage2PerformanceAdditionalInformation = "KeyStage2PerformanceAdditionalInformation",
                        KeyStage4PerformanceAdditionalInformation = "KeyStage4PerformanceAdditionalInformation",
                        KeyStage5PerformanceAdditionalInformation = "KeyStage5PerformanceAdditionalInformation"
                    }
                },
                
                OutgoingTrustUkprn = OutgoingTrustUkprn,
                
            };
            
            return tramsProject;
        }

        public static TrustDto Trust()
        {
            var tramsTrust = new TrustDto
            {

                    CompaniesHouseNumber = "1231231",
                    Address = new AddressDto
                    {
                        Additional = "Extra line",
                        County = "County",
                        Locality = "Locality",
                        Postcode = "Postcode",
                        Street = "Street",
                        Town = "Town"
                    },
                    ReferenceNumber = "0001",
                    Name = IncomingTrustName,
                    Ukprn = IncomingTrustUkprn

            };
            return tramsTrust;
        }

        public static TramsEstablishment Establishment()
        {
            return new TramsEstablishment
            {
                Address = new Address
                {
                    Street = "Example street",
                    Town = "Town",
                    County = "Fakeshire",
                    Postcode = "FA11 1KE"
                },
                Census = new Census
                {
                    NumberOfPupils = "905",
                    NumberOfBoys = "450",
                    NumberOfGirls = "455",
                    PercentageFsm = "15.9",
                    PercentageSen = "19.5",
                    PercentageEnglishNotFirstLanguage = "4.7",
                    PercentageEligableForFSM6Years = "2.9"
                },
                EstablishmentName = OutgoingAcademyName,
                EstablishmentType = new NameAndCode {Name = "Type of establishment"},
                LocalAuthorityName = "Fake LA",
                MisEstablishment = new MisEstablishment
                {
                    BehaviourAndAttitudes = "1",
                    EarlyYearsProvision = "9",
                    EffectivenessOfLeadershipAndManagement = "2",
                    OverallEffectiveness = "1",
                    PersonalDevelopment = "3",
                    QualityOfEducation = "4",
                    ReligiousEthos = "Does not apply",
                    SixthFormProvision = "1",
                    WebLink = "http://example.com"
                },
                SchoolCapacity = "1000",
                StatutoryLowAge = "4",
                StatutoryHighAge = "11",
                OfstedLastInspection = "01-01-2020",
                OfstedRating = "Good",
                PhaseOfEducation = new NameAndCode {Name = "Primary"},
                Ukprn = OutgoingAcademyUkprn,
                Urn = OutgoingAcademyUrn,
                ViewAcademyConversion = new ViewAcademyConversion
                {
                    Deficit = "Deficit",
                    Pan = "Pan",
                    Pfi = "Pfi",
                    ViabilityIssue = "Viability issue"
                },
                Gor = new NameAndCode { Name = "Region Name" }
            };
        }

        public static TramsEducationPerformance EducationPerformance()
        {
            return new TramsEducationPerformance
            {
                KeyStage2 = new List<KeyStage2>
                {
                    new KeyStage2
                    {
                        Year = "2021",
                        PercentageMeetingExpectedStdInRWM = GetTestResult(),
                        PercentageAchievingHigherStdInRWM = GetTestResult(),
                        ReadingProgressScore = GetTestResult(),
                        WritingProgressScore = GetTestResult(),
                        MathsProgressScore = GetTestResult(),
                        NationalAveragePercentageMeetingExpectedStdInRWM = GetTestResult(),
                        NationalAveragePercentageAchievingHigherStdInRWM = GetTestResult(),
                        NationalAverageReadingProgressScore = GetTestResult(),
                        NationalAverageWritingProgressScore = GetTestResult(),
                        NationalAverageMathsProgressScore = GetTestResult(),
                        LAAveragePercentageMeetingExpectedStdInRWM = GetTestResult(),
                        LAAveragePercentageAchievingHigherStdInRWM = GetTestResult(),
                        LAAverageReadingProgressScore = GetTestResult(),
                        LAAverageWritingProgressScore = GetTestResult(),
                        LAAverageMathsProgressScore = GetTestResult(),
                    }
                },
                KeyStage4 = new List<KeyStage4>
                {
                    new KeyStage4
                    {
                        Year = "2021",
                        SipAttainment8score = GetTestResult(),
                        SipAttainment8scoreenglish = GetTestResult(),
                        SipAttainment8scoremaths = GetTestResult(),
                        SipAttainment8scoreebacc = GetTestResult(),
                        SipNumberofpupilsprogress8 = GetTestResult(),
                        SipProgress8upperconfidence = new decimal(RandomGenerator.NextDouble()),
                        SipProgress8lowerconfidence = new decimal(RandomGenerator.NextDouble()),
                        SipProgress8english = GetTestResult(),
                        SipProgress8maths = GetTestResult(),
                        SipProgress8ebacc = GetTestResult(),
                        SipProgress8Score = GetTestResult(),
                        NationalAverageA8Score = GetTestResult(),
                        NationalAverageA8English = GetTestResult(),
                        NationalAverageA8Maths = GetTestResult(),
                        NationalAverageA8EBacc = GetTestResult(),
                        NationalAverageP8PupilsIncluded =
                            GetTestResult(),
                        NationalAverageP8Score = GetTestResult(),
                        NationalAverageP8LowerConfidence = new decimal(RandomGenerator.NextDouble()),
                        NationalAverageP8UpperConfidence = new decimal(RandomGenerator.NextDouble()),
                        NationalAverageP8English = GetTestResult(),
                        NationalAverageP8Maths = GetTestResult(),
                        NationalAverageP8Ebacc = GetTestResult(),
                        LAAverageA8Score = GetTestResult(),
                        LAAverageA8English = GetTestResult(),
                        LAAverageA8Maths = GetTestResult(),
                        LAAverageA8EBacc = GetTestResult(),
                        LAAverageP8PupilsIncluded = GetTestResult(),
                        LAAverageP8Score = GetTestResult(),
                        LAAverageP8LowerConfidence = new decimal(RandomGenerator.NextDouble()),
                        LAAverageP8UpperConfidence = new decimal(RandomGenerator.NextDouble()),
                        LAAverageP8English = GetTestResult(),
                        LAAverageP8Maths = GetTestResult(),
                        LAAverageP8Ebacc = GetTestResult(),
                        Enteringebacc = new decimal(RandomGenerator.NextDouble()),
                        LAEnteringEbacc = new decimal(RandomGenerator.NextDouble()),
                        NationalEnteringEbacc = new decimal(RandomGenerator.NextDouble())
                    }
                }, 
                KeyStage5 = new List<KeyStage5>()
                {
                    new KeyStage5
                    {
                        Year = "2018-2019",
                        AcademicQualificationAverage = "12.12",
                        AppliedGeneralQualificationAverage = "21.21",
                        NationalAcademicQualificationAverage = "11.11",
                        NationalAppliedGeneralQualificationAverage = "22.22"
                    },
                    new KeyStage5
                    {
                        Year = "2019-2020",
                        AcademicQualificationAverage = "12.12",
                        AppliedGeneralQualificationAverage = "21.21",
                        NationalAcademicQualificationAverage = "11.11",
                        NationalAppliedGeneralQualificationAverage = "22.22"
                    }
                }
            };
        }

        private static DisadvantagedPupilsResponse GetTestResult()
        {
            return new DisadvantagedPupilsResponse
            {
                NotDisadvantaged = RandomGenerator.NextDouble().ToString(),
                Disadvantaged = RandomGenerator.NextDouble().ToString()
            };
        }
    }
}