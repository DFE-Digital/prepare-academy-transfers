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
                Ukprn = input.Ukprn,
                Name = input.EstablishmentName,
                Performance = new AcademyPerformance
                {
                    AgeRange = $"{input.StatutoryLowAge} to {input.StatutoryHighAge}",
                    Capacity = input.SchoolCapacity
                }
            };
        }
    }
}