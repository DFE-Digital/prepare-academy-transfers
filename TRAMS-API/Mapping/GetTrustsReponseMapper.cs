using API.Models.D365;
using API.Models.Response;

namespace API.Mapping
{
    public class GetTrustsReponseMapper : IMapper<GetTrustsD365Model, GetTrustsModel>
    {
        private readonly IEstablishmentNameFormatter _establishmentNameFormatter;

        public GetTrustsReponseMapper(IEstablishmentNameFormatter establishmentNameFormatter)
        {
            _establishmentNameFormatter = establishmentNameFormatter;
        }

        public GetTrustsModel Map(GetTrustsD365Model input)
        {
            if(input == null)
            {
                return null;
            }    

            return new GetTrustsModel
            {
                Id = input.Id,
                Address = input.Address,
                CompaniesHouseNumber = input.CompaniesHouseNumber,
                EstablishmentType = input.EstablishmentType,
                EstablishmentTypeGroup = input.EstablishmentTypeGroup,
                TrustName = _establishmentNameFormatter.Format(input.TrustName),
                TrustReferenceNumber = input.TrustReferenceNumber,
                Ukprn = input.Ukprn,
                Upin = input.Upin
            };
        }
    }
}
