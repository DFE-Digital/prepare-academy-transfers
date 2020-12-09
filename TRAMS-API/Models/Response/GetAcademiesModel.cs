using System;

namespace API.Models.Response
{
    public class GetAcademiesModel
    {
        public Guid Id { get; set; }

        public Guid ParentTrustId { get; set; }

        public string AcademyName { get; set; }

        public string Urn { get; set; }

        public string Address { get; set; }

        public string EstablishmentType { get; set; }

        public string LocalAuthorityNumber { get; set; }

        public string LocalAuthorityName { get; set; }

        public string ReligiousCharacter { get; set; }

        public string DioceseName { get; set; }

        public string ReligiousEthos { get; set; }

        public string OftstedRating { get; set; }

        public DateTime? OfstedInspectionDate { get; set; }
    }
}
