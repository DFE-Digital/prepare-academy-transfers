using Data.Models;
using Data.Models.KeyStagePerformance;

namespace Frontend.Services.Responses
{
    public class GetInformationForProjectResponse
    {
        public Project Project { get; set; }
        public Academy OutgoingAcademy { get; set; }
        public EducationPerformance EducationPerformance { get; set; }
    }
}