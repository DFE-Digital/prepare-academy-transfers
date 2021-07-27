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
                    BoysOnRoll = input.Census.NumberOfBoys,
                    GirlsOnRoll = input.Census.NumberOfGirls
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
            return new GeneralInformation
            {
                AgeRange = $"{input.StatutoryLowAge} to {input.StatutoryHighAge}",
                Capacity = input.SchoolCapacity,
                NumberOnRoll = input.Census.NumberOfPupils,
                PercentageFull = PercentageFull(input),
                SchoolPhase = input.PhaseOfEducation.Name,
                SchoolType = input.EstablishmentType.Name
            };
        }

        private static List<string> Address(TramsEstablishment input)
        {
            return new List<string>
                {input.Address.Street, input.Address.Town, input.Address.County, input.Address.Postcode};
        }

        private static string PercentageFull(TramsEstablishment input)
        {
            return Math.Round(
                    decimal.Parse(input.Census.NumberOfPupils) / decimal.Parse(input.SchoolCapacity) * 100,
                    1)
                .ToString(CultureInfo.InvariantCulture);
        }
    }
}