using System.Linq;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Models;

namespace Dfe.PrepareTransfers.Data.TRAMS.Mappers.Response
{
    public class TramsSearchResultMapper : IMapper<TramsTrustSearchResult, TrustSearchResult>
    {
        public TrustSearchResult Map(TramsTrustSearchResult input)
        {
            return new TrustSearchResult
            {
                Ukprn = input.Ukprn,
                TrustName = input.GroupName,
                CompaniesHouseNumber = input.CompaniesHouseNumber,
                Academies = input.Establishments.Select(establishment => new TrustSearchAcademy
                {
                    Name = establishment.Name, Ukprn = establishment.Ukprn, Urn = establishment.Urn
                }).ToList()
            };
        }
    }
}