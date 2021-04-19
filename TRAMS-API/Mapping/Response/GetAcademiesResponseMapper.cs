using API.Models.Downstream.D365;
using API.Models.Upstream.Response;

namespace API.Mapping.Response
{
    public class GetAcademiesResponseMapper : IMapper<GetAcademiesD365Model, GetAcademiesModel>
    {
        private readonly IEstablishmentNameFormatter _establishmentNameFormatter;

        public GetAcademiesResponseMapper(IEstablishmentNameFormatter establishmentNameFormatter)
        {
            _establishmentNameFormatter = establishmentNameFormatter;
        }

        public GetAcademiesModel Map(GetAcademiesD365Model input)
        {
            if (input == null)
            {
                return null;
            }

            return new GetAcademiesModel
            {
                Id = input.Id,
                AcademyName = _establishmentNameFormatter.Format(input.AcademyName),
                Address = input.Address,
                DioceseName = input.DioceseName,
                EstablishmentType = input.EstablishmentType,
                LocalAuthorityName = input.LocalAuthorityName,
                LocalAuthorityNumber = input.LocalAuthorityNumber,
                OfstedInspectionDate = input.OfstedInspectionDate,
                OfstedRating = input.OftstedRating,
                ParentTrustId = input.ParentTrustId.Value,
                Pfi = input.Predecessor?.Pfi,
                ReligiousCharacter = input.ReligiousCharacter,
                ReligiousEthos = input.ReligiousEthos,
                Urn = input.Urn,
                Ukprn = input.Ukprn
            };
        }
    }
}
