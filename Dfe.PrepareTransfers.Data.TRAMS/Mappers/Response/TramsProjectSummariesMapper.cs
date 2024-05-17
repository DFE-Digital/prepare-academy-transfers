using System.Linq;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Data.TRAMS.Models;

namespace Dfe.PrepareTransfers.Data.TRAMS.Mappers.Response
{
    public class TramsProjectSummariesMapper : IMapper<TramsProjectSummary, ProjectSearchResult>
    {
        public ProjectSearchResult Map(TramsProjectSummary input)
        {
            return new ProjectSearchResult
            {
                Urn = input.ProjectUrn,
                Reference = input.ProjectReference,
                OutgoingTrustName = input.OutgoingTrustName,
                OutgoingTrustUkprn = input.OutgoingTrustUkprn,
                Status = input.Status,
                TransferringAcademies = input.TransferringAcademies.Select(
                    academy => new TransferringAcademy
                    {
                        OutgoingAcademyUkprn = academy.OutgoingAcademyUkprn,
                        IncomingTrustUkprn = academy.IncomingTrustUkprn,
                        IncomingTrustName = !string.IsNullOrEmpty(academy.IncomingTrustName) ? academy.IncomingTrustName : input.OutgoingTrustName
                    }).ToList(),
                AssignedUser = input.AssignedUser,
                IsFormAMat = input.IsFormAMat,
            };
        }
    }
}