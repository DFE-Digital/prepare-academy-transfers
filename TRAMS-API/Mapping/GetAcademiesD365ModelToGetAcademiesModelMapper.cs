using API.Models.D365;
using API.Models.Response;

namespace API.Mapping
{
    public class GetAcademiesD365ModelToGetAcademiesModelMapper : IMapper<GetAcademiesD365Model, GetAcademiesModel>
    {
        public GetAcademiesModel Map(GetAcademiesD365Model input)
        {
            if (input == null)
            {
                return null;
            }

            return new GetAcademiesModel
            {
                Id = input.Id,
                AcademyName = input.AcademyName,
                Address = input.Address,
                DioceseName = input.DioceseName,
                EstablishmentType = input.EstablishmentType,
                LocalAuthorityName = input.LocalAuthorityName,
                LocalAuthorityNumber = input.LocalAuthorityNumber,
                OfstedInspectionDate = input.OfstedInspectionDate,
                OfstedRating = input.OftstedRating,
                ParentTrustId = input.ParentTrustId,
                Pfi = input.Predecessor.Pfi,
                ReligiousCharacter = input.ReligiousCharacter,
                ReligiousEthos = input.ReligiousEthos,
                Urn = input.Urn
            };
        }
    }
}
