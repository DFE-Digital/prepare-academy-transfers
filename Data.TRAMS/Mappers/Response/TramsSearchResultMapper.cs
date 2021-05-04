using System.Linq;
using Data.Models;
using Data.TRAMS.Models;

namespace Data.TRAMS.Mappers.Response
{
    public class TramsSearchResultMapper : IMapper<TramsTrustSearchResult, TrustSearchResult>
    {
        public TrustSearchResult Map(TramsTrustSearchResult input)
        {
            return new TrustSearchResult
            {
                Ukprn = input.Urn,
                TrustName = input.GroupName,
                CompaniesHouseNumber = input.CompaniesHouseNumber,
                Academies = input.Academies.Select(academy => new TrustSearchAcademy
                {
                    Name = academy.Name, Ukprn = academy.Urn
                }).ToList()
            };
        }
    }
}