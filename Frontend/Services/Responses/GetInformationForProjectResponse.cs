using System.Collections.Generic;
using Data.Models;
using Data.Models.KeyStagePerformance;

namespace Frontend.Services.Responses
{
    public class GetInformationForProjectResponse
    {
        public Project Project { get; set; }
        public List<Academy> OutgoingAcademies { get; set; }
    }
}