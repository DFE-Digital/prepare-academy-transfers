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
                    academy => new TransferringAcademies
                    {
                        OutgoingAcademyUkprn = academy.OutgoingAcademyUkprn,
                        IncomingTrustUkprn = academy.IncomingTrustUkprn,
                        IncomingTrustName = academy.IncomingTrustName
                    }).ToList(),
                AssignedUser = input.AssignedUser
            };
        }
    }
}