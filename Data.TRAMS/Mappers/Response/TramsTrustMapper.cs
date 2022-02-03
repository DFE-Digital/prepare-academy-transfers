using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.TRAMS.Models;

namespace Data.TRAMS.Mappers.Response
{
    public class TramsTrustMapper : IMapper<TramsTrust, Trust>
    {
        private IMapper<TramsEstablishment, Academy> _establishmentMapper;

        public TramsTrustMapper(IMapper<TramsEstablishment, Academy> establishmentMapper)
        {
            _establishmentMapper = establishmentMapper;
        }

        public Trust Map(TramsTrust input)
        {
            var address = input.GiasData.GroupContactAddress;
            return new Trust
            {
                Academies = input.Establishments.Select(e => _establishmentMapper.Map(e)).ToList(),
                Address = new List<string>
                {
                    input.GiasData.GroupName,
                    address.Street,
                    address.Town,
                    $"{address.County}, {address.Postcode}"
                },
                CompaniesHouseNumber = input.GiasData.CompaniesHouseNumber,
                EstablishmentType = "Not available",
                GiasGroupId = input.GiasData.GroupId,
                Name = input.GiasData.GroupName,
                Ukprn = input.GiasData.Ukprn,
                LeadRscRegion = input.IfdData.LeadRscRegion
            };
        }
    }
}