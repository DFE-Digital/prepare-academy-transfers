using System.Collections.Generic;
using System.Linq;
using Dfe.Academies.Contracts.V4.Trusts;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.TRAMS.Models;

namespace Dfe.PrepareTransfers.Data.TRAMS.Mappers.Response
{
    public class TramsTrustMapper : IMapper<TrustDto, Trust>
    {
        private readonly IMapper<TramsEstablishment, Academy> _establishmentMapper;

        public TramsTrustMapper(IMapper<TramsEstablishment, Academy> establishmentMapper)
        {
            _establishmentMapper = establishmentMapper;
        }

        public Trust Map(TrustDto input)
        {
            var address = input.Address;
            return new Trust
            {
                Address = new List<string>
                {
                    input.Name,
                    address.Street,
                    address.Town,
                    $"{address.County}, {address.Postcode}"
                },
                CompaniesHouseNumber = input.CompaniesHouseNumber,
                GiasGroupId = input.ReferenceNumber,
                Name = input.Name,
                Ukprn = input.Ukprn
            };
        }
    }
}