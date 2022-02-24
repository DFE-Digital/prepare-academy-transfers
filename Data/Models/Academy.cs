using System.Collections.Generic;
using Data.Models.Academies;
using Data.Models.KeyStagePerformance;

namespace Data.Models
{
    public class Academy
    {
        public Academy()
        {
            GeneralInformation = new GeneralInformation();
            PupilNumbers = new PupilNumbers();
            LatestOfstedJudgement = new LatestOfstedJudgement();
            Address = new List<string>();
            EducationPerformance = new EducationPerformance();
        }
        
        public string Ukprn { get; set; }
        public string Urn { get; set; }
        public string Name { get; set; }
        public string LocalAuthorityName { get; set; }
        public string EstablishmentType { get; set; }
        public string FaithSchool { get; set; }
        public string Pfi { get; set; }
        public GeneralInformation GeneralInformation { get; set; }
        public PupilNumbers PupilNumbers { get; set; }
        public LatestOfstedJudgement LatestOfstedJudgement { get; set; }
        public List<string> Address { get; set; }
        public EducationPerformance EducationPerformance { get; set; }
        
    }
}