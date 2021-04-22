using System.Collections.Generic;
using API.Models.Upstream.Response;
using Data;
using Data.Models;

namespace Frontend.Models.AcademyPerformance
{
    public class AcademyPerformanceViewModel
    {
        public GetProjectsResponseModel Project { get; set; }
        public Academy OutgoingAcademy { get; set; }

    }
}