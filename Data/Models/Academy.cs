using System.Collections.Generic;
using Data.Models.Academies;

namespace Data.Models
{
    public class Academy
    {
        public string Ukprn { get; set; }
        public string Urn { get; set; }
        public string Name { get; set; }
        public string LocalAuthorityName { get; set; }
        public string EstablishmentType { get; set; }
        public string ReligiousCharacter { get; set; }
        public string Pfi { get; set; }
        public AcademyPerformance Performance { get; set; }
        public PupilNumbers PupilNumbers { get; set; }

        public LatestOfstedJudgement LatestOfstedJudgement { get; set; }
        public List<string> Address { get; set; }
    }
}