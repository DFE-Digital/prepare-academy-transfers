using System.Collections.Generic;
using Dfe.Academies.Contracts.V4.Establishments;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Academies;
using Dfe.PrepareTransfers.Helpers;

namespace Dfe.PrepareTransfers.Data.TRAMS.Mappers.Response
{
    public class AcademiesEstablishmentMapper : IMapper<EstablishmentDto, Academy>
    {
        public Academy Map(EstablishmentDto input)
        {
            return new Academy
            {
                Address = Address(input),
                FaithSchool = input.ReligousEthos,
                LatestOfstedJudgement = LatestOfstedJudgement(input),
                LocalAuthorityName = input.LocalAuthorityName,
                Name = input.Name,
                GeneralInformation = GeneralInformation(input),
                EstablishmentType = input.EstablishmentType.Name,
                PupilNumbers = new PupilNumbers
                {
                    BoysOnRoll = PercentageHelper.CalculatePercentageFromStrings(input.NoOfBoys, input.Census.NumberOfPupils),
                    GirlsOnRoll = PercentageHelper.CalculatePercentageFromStrings(input.NoOfGirls, input.Census.NumberOfPupils),
                    WithStatementOfSen = PercentageHelper.DisplayAsPercentage(input.Census.PercentageSen),
                    WhoseFirstLanguageIsNotEnglish = PercentageHelper.DisplayAsPercentage(input.Census.PercentageEnglishAsSecondLanguage),
                    PercentageEligibleForFreeSchoolMealsDuringLast6Years = PercentageHelper.DisplayAsPercentage(input.Census.PercentageFsmLastSixYears)
                },
                Ukprn = input.Ukprn,
                Urn = input.Urn,
                LastChangedDate = input.GiasLastChangedDate,
                Region = input.Gor.Name
            };
        }

        private static LatestOfstedJudgement LatestOfstedJudgement(EstablishmentDto input)
        {
            return new LatestOfstedJudgement
            {
                InspectionEndDate = input.MISEstablishment.InspectionEndDate,
                OverallEffectiveness = ParseOfstedRating(input.MISEstablishment.OverallEffectiveness),
                SchoolName = input.Name,
                OfstedReport = input.MISEstablishment.Weblink,
                QualityOfEducation = ParseOfstedRating(input.MISEstablishment.QualityOfEducation),
                BehaviourAndAttitudes = ParseOfstedRating(input.MISEstablishment.BehaviourAndAttitudes),
                PersonalDevelopment = ParseOfstedRating(input.MISEstablishment.PersonalDevelopment),
                EffectivenessOfLeadershipAndManagement = ParseOfstedRating(input.MISEstablishment.EffectivenessOfLeadershipAndManagement),
                EarlyYearsProvision = ParseOfstedRating(input.MISEstablishment.EarlyYearsProvision),
                SixthFormProvision = ParseOfstedRating(input.MISEstablishment.SixthFormProvision),
                DateOfLatestSection8Inspection = input.MISEstablishment.DateOfLatestSection8Inspection
            };
        }

        private static string ParseOfstedRating(string ofstedRating)
        {
            return ofstedRating switch
            {
                "1" => "Outstanding",
                "2" => "Good",
                "3" => "Requires improvement",
                "4" => "Inadequate",
                "9" => "No data",
                _ => "N/A"
            };
        }

        private static GeneralInformation GeneralInformation(EstablishmentDto input)
        {
            var generalInformation = new GeneralInformation
            {
                AgeRange = $"{input.StatutoryLowAge} to {input.StatutoryHighAge}",
                Capacity = input.SchoolCapacity,
                NumberOnRoll = input.Census.NumberOfPupils,
                PercentageFull = PercentageHelper.CalculatePercentageFromStrings(input.Census.NumberOfPupils, input.SchoolCapacity),
                SchoolPhase = input.PhaseOfEducation.Name,
                SchoolType = input.EstablishmentType.Name,
                PercentageFsm = PercentageHelper.DisplayAsPercentage(input.Census.PercentageFsm)
            };

            //if (input.ViewAcademyConversion == null)
            //{
            //    return generalInformation;
            //}

            generalInformation.Pan = input.Pan;
            generalInformation.Pfi = input.Pfi;
            generalInformation.Deficit = input.Deficit;
            generalInformation.ViabilityIssue = input.ViabilityIssue;

            return generalInformation;
        }

        private static List<string> Address(EstablishmentDto input)
        {
            return new List<string>
                {input.Address.Street, input.Address.Town, input.Address.County, input.Address.Postcode};
        }
    }
}