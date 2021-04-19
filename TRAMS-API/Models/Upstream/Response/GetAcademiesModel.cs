using System;

namespace API.Models.Upstream.Response
{
    public class GetAcademiesModel
    {
        /// <summary>
        /// The GUID of the Academy in TRAMS
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The GUID of the parent trust in TRAMS
        /// </summary>
        public Guid ParentTrustId { get; set; }

        /// <summary>
        /// The name of the academy
        /// </summary>
        /// <example>Imaginary Academy</example>
        public string AcademyName { get; set; }

        /// <summary>
        /// The URN of an academy
        /// </summary>
        /// <example>424242</example>
        public string Urn { get; set; }
        
        public string Ukprn { get; set; }

        /// <summary>
        /// The multi-line address of an academy
        /// </summary>
        /// <example>Imaginary Trust \r\n Sample Road \r\n London SW14 5FJ</example>
        public string Address { get; set; }

        /// <summary>
        /// The Establishment Type of the academy
        /// </summary>
        /// <example>Academy Converter</example>
        public string EstablishmentType { get; set; }

        /// <summary>
        /// The Local Authority Number of the academy
        /// </summary>
        /// <example>4242</example>
        public string LocalAuthorityNumber { get; set; }

        /// <summary>
        /// The Local Authority Name of the academy
        /// </summary>
        /// <example>Authority West</example>
        public string LocalAuthorityName { get; set; }

        /// <summary>
        /// The Religious Character of the academy
        /// </summary>
        /// <example>Does not apply</example>
        public string ReligiousCharacter { get; set; }

        /// <summary>
        /// The name of the diocese the academy belongs to
        /// </summary>
        /// <example>Not applicable</example>
        public string DioceseName { get; set; }

        /// <summary>
        /// The religious ethos of the academy
        /// </summary>
        /// <example>Does not apply</example>
        public string ReligiousEthos { get; set; }

        /// <summary>
        /// The Ofsted Rating of the academy
        /// </summary>
        /// <example>Good</example>
        public string OfstedRating { get; set; }

        /// <summary>
        /// The date of the last Oftsed Insepction in ISO format
        /// </summary>
        public DateTime? OfstedInspectionDate { get; set; }

        /// <summary>
        /// The PFI of the academy
        /// </summary>
        /// <example>Full PFI</example>
        public string Pfi { get; set; }
    }
}
