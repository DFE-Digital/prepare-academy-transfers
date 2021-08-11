using System.Collections.Generic;
using Data.Models;
using Data.Models.Academies;
using Data.TRAMS.Models;
using Helpers;

namespace Data.TRAMS.Mappers.Response
{
    public class TramsEstablishmentMapper : IMapper<TramsEstablishment, Academy>
    {
        public Academy Map(TramsEstablishment input)
        {
            return new Academy
            {
                Address = Address(input),
                EstablishmentType = input.EstablishmentType.Name,
                FaithSchool = input.MisEstablishment.ReligiousEthos,
                LatestOfstedJudgement = LatestOfstedJudgement(input),
                LocalAuthorityName = input.LocalAuthorityName,
                Name = input.EstablishmentName,
                GeneralInformation = GeneralInformation(input),
                PupilNumbers = new PupilNumbers
                {
                    BoysOnRoll = PercentageHelper.CalculatePercentageFromStrings(input.Census.NumberOfBoys, input.Census.NumberOfPupils),
                    GirlsOnRoll = PercentageHelper.CalculatePercentageFromStrings(input.Census.NumberOfGirls, input.Census.NumberOfPupils),
                    WithStatementOfSen = PercentageHelper.DisplayAsPercentage(input.Census.PercentageSen),
                    WhoseFirstLanguageIsNotEnglish = PercentageHelper.DisplayAsPercentage(input.Census.PercentageEnglishNotFirstLanguage),
                    PercentageEligibleForFreeSchoolMealsDuringLast6Years = PercentageHelper.DisplayAsPercentage(input.Census.PercentageEligableForFSM6Years)
                },
                Ukprn = input.Ukprn,
                Urn = input.Urn
            };
        }

        private static LatestOfstedJudgement LatestOfstedJudgement(TramsEstablishment input)
        {
            return new LatestOfstedJudgement
            {
                InspectionDate = input.OfstedLastInspection,
                OverallEffectiveness = ParseOfstedRating(input.MisEstablishment.OverallEffectiveness),
                SchoolName = input.EstablishmentName,
                OfstedReport = input.MisEstablishment.WebLink
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
                _ => "N/A"
            };
        }

        private static GeneralInformation GeneralInformation(TramsEstablishment input)
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

            if (input.ViewAcademyConversion == null) return generalInformation;
            
            generalInformation.Pan = input.ViewAcademyConversion.Pan;
            generalInformation.Pfi = input.ViewAcademyConversion.Pfi;
            generalInformation.Deficit = input.ViewAcademyConversion.Deficit;
            generalInformation.ViabilityIssue = input.ViewAcademyConversion.ViabilityIssue;

            return generalInformation;
        }

        private static List<string> Address(TramsEstablishment input)
        {
            return new List<string>
                {input.Address.Street, input.Address.Town, input.Address.County, input.Address.Postcode};
        }
    }
}