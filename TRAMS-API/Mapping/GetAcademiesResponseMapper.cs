﻿using API.Models.D365;
using API.Models.Response;

namespace API.Mapping
{
    public class GetAcademiesResponseMapper : IMapper<GetAcademiesD365Model, GetAcademiesModel>
    {
        private readonly IEstablishmentNameFormatter _establishmentNameFormatter;

        public GetAcademiesResponseMapper(IEstablishmentNameFormatter establishmentNameFormatter)
        {
            this._establishmentNameFormatter = establishmentNameFormatter;
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
                Urn = input.Urn
            };
        }
    }
}
