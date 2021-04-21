using System.Runtime.InteropServices;

namespace Data.Models
{
    public class Academy
    {
        public string Ukprn { get; set; }
        public AcademyPerformance Performance { get; set; }
        public PupilNumbers PupilNumbers { get; set; }
    }
}