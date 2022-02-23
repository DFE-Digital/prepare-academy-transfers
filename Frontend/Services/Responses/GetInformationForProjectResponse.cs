using System.Collections.Generic;
using Data.Models;
using Data.Models.KeyStagePerformance;

namespace Frontend.Services.Responses
{
    public class GetInformationForProjectResponse
    {
        public Project Project { get; set; }
        
        // todo: remove this when other mat-mat work has been done
        public Academy OutgoingAcademy { get; set; }
        
        public List<Academy> OutgoingAcademies { get; set; }

        // todo: remove this when other mat-mat work has been done (this is now in Academy model)
        public EducationPerformance EducationPerformance { get; set; }
    }
}