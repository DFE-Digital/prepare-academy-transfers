using System.Collections.Generic;
using Data.Models;
using Data.TRAMS.Models;

namespace Data.TRAMS.Mappers.Response
{
    public class TramsTrustMapper : IMapper<TramsTrust, Trust>
    {
        public Trust Map(TramsTrust input)
        {
            var address = input.GiasData.GroupContactAddress;
            return new Trust
            {
                Ukprn = input.GiasData.Ukprn,
                Name = input.GiasData.GroupName,
                CompaniesHouseNumber = input.GiasData.CompaniesHouseNumber,
                GiasGroupId = input.GiasData.GroupId,
                EstablishmentType = "Not available yet",
                Address = new List<string>
                {
                    input.GiasData.GroupName,
                    address.Street,
                    address.Town,
                    $"{address.County}, ${address.Postcode}"
                }
            };
        }
    }
}