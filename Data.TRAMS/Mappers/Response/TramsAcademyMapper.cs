using System;
using System.Collections.Generic;
using System.Globalization;
using Data.Models;
using Data.Models.Academies;
using Data.TRAMS.Models;

namespace Data.TRAMS.Mappers.Response
{
    public class TramsAcademyMapper : IMapper<TramsAcademy, Academy>
    {
        public Academy Map(TramsAcademy input)
        {
            return new Academy
            {
                Address = Address(input),
                EstablishmentType = input.EstablishmentType.Name,
                LocalAuthorityName = input.LocalAuthorityName,
                Name = input.EstablishmentName,
                Performance = Performance(input),
                Ukprn = input.Ukprn
            };
        }

        private static AcademyPerformance Performance(TramsAcademy input)
        {
            return new AcademyPerformance
            {
                AgeRange = $"{input.StatutoryLowAge} to {input.StatutoryHighAge}",
                Capacity = input.SchoolCapacity,
                NumberOnRoll = input.Census.NumberOfPupils,
                OfstedJudgementDate = input.OfstedLastInspection,
                OfstedRating = input.OfstedRating,
                PercentageFull = PercentageFull(input),
                SchoolPhase = input.PhaseOfEducation.Name,
                SchoolType = input.EstablishmentType.Name,
            };
        }

        private static List<string> Address(TramsAcademy input)
        {
            return new List<string>
                {input.Address.Street, input.Address.Town, input.Address.County, input.Address.Postcode};
        }

        private static string PercentageFull(TramsAcademy input)
        {
            return Math.Round(
                    decimal.Parse(input.Census.NumberOfPupils) / decimal.Parse(input.SchoolCapacity) * 100,
                    1)
                .ToString(CultureInfo.InvariantCulture);
        }
    }
}