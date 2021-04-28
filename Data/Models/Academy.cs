using System;

namespace Data.Models
{
    public class Academy
    {
        public Guid DynamicsId { get; set; }
        public string Ukprn { get; set; }
        public string Name { get; set; }
        public AcademyPerformance Performance { get; set; }
        public PupilNumbers PupilNumbers { get; set; }

        public LatestOfstedJudgement LatestOfstedJudgement { get; set; }
    }
}