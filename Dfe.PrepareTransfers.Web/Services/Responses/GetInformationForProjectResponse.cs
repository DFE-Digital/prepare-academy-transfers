using System.Collections.Generic;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;

namespace Dfe.PrepareTransfers.Web.Services.Responses
{
    public class GetInformationForProjectResponse
    {
        public Project Project { get; set; }
        public List<Academy> OutgoingAcademies { get; set; }
    }
}