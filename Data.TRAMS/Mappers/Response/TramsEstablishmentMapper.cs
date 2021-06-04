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
                Performance = Performance(input),
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
                AchievementOfPupils = ParseOfstedRating(input.MisEstablishment.PersonalDevelopment),
                BehaviourAndSafetyOfPupils = ParseOfstedRating(input.MisEstablishment.BehaviourAndAttitudes),
                EarlyYearsProvision = ParseOfstedRating(input.MisEstablishment.EarlyYearsProvision),
                InspectionDate = input.OfstedLastInspection,
                LeadershipAndManagement =
                    ParseOfstedRating(input.MisEstablishment.EffectivenessOfLeadershipAndManagement),
                OverallEffectiveness = ParseOfstedRating(input.MisEstablishment.OverallEffectiveness),
                QualityOfTeaching = ParseOfstedRating(input.MisEstablishment.QualityOfEducation),
                SchoolName = input.EstablishmentName,
                SixthFormProvision = ParseOfstedRating(input.MisEstablishment.SixthFormProvision)
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

        private static AcademyPerformance Performance(TramsEstablishment input)
        {
            return new AcademyPerformance
            {
                AchievementOfPupil = ParseOfstedRating(input.MisEstablishment.PersonalDevelopment),
                AgeRange = $"{input.StatutoryLowAge} to {input.StatutoryHighAge}",
                BehaviourAndSafetyOfPupil = ParseOfstedRating(input.MisEstablishment.BehaviourAndAttitudes),
                Capacity = input.SchoolCapacity,
                LeadershipAndManagement =
                    ParseOfstedRating(input.MisEstablishment.EffectivenessOfLeadershipAndManagement),
                NumberOnRoll = input.Census.NumberOfPupils,
                OfstedJudgementDate = input.OfstedLastInspection,
                OfstedRating = input.OfstedRating,
                PercentageFull = PercentageFull(input),
                QualityOfTeaching = ParseOfstedRating(input.MisEstablishment.QualityOfEducation),
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