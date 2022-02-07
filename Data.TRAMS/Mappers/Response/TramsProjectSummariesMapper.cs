using System.Linq;
using Data.Models;
using Data.Models.Projects;
using Data.TRAMS.Models;

namespace Data.TRAMS.Mappers.Response
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
                TransferringAcademies = input.TransferringAcademies.Select(
                    academy => new TransferringAcademies
                    {
                        OutgoingAcademyUkprn = academy.OutgoingAcademyUkprn,
                        IncomingTrustUkprn = academy.IncomingTrustUkprn,
                        IncomingTrustName = academy.IncomingTrustName
                    }).ToList()
            };
        }
    }
}