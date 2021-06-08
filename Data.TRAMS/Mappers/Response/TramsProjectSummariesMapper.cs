using System;
using System.Collections.Generic;
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
                OutgoingTrustName = input.OutgoingTrust.GroupName,
                TransferringAcademies = input.TransferringAcademies.Select(
                    academy => new TransferringAcademies
                    {
                        IncomingTrustName = academy.IncomingTrust.GroupName,
                        IncomingTrustUkprn = academy.IncomingTrust.Ukprn,
                        OutgoingAcademyName = academy.OutgoingAcademy.Name,
                        OutgoingAcademyUkprn = academy.OutgoingAcademy.Ukprn,
                        OutgoingAcademyUrn = academy.OutgoingAcademy.Urn
                    }).ToList()
            };
        }
    }
}