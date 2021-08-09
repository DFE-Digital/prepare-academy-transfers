using System;
using System.Collections.Generic;
using System.Globalization;
using Data.Models;
using Data.Models.Academies;
using Data.TRAMS.Models;

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
                    BoysOnRoll = CalculatePercentage(input.Census.NumberOfBoys, input.Census.NumberOfPupils),
                    GirlsOnRoll = CalculatePercentage(input.Census.NumberOfGirls, input.Census.NumberOfPupils),
                    WithStatementOfSen = DisplayAsPercentage(input.Census.PercentageSen),
                    WhoseFirstLanguageIsNotEnglish = DisplayAsPercentage(input.Census.PercentageEnglishNotFirstLanguage),
                    PercentageEligibleForFreeSchoolMealsDuringLast6Years = DisplayAsPercentage(input.Census.PercentageEligableForFSM6Years)
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
                PercentageFull = CalculatePercentage(input.Census.NumberOfPupils, input.SchoolCapacity),
                SchoolPhase = input.PhaseOfEducation.Name,
                SchoolType = input.EstablishmentType.Name,
                PercentageFsm = input.Census.PercentageFsm
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
        
        private static string CalculatePercentage(string value, string total)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(total))
                return string.Empty;
            return DisplayAsPercentage(Math.Round(decimal.Parse(value) / decimal.Parse(total) * 100, 1)
                .ToString(CultureInfo.InvariantCulture));
        }
        
        private static string DisplayAsPercentage(string value) => string.IsNullOrEmpty(value) ? value : $"{value}%";
    }
}