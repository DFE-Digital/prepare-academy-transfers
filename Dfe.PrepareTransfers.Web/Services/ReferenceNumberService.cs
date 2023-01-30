using System;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Web.Services.Interfaces;

namespace Dfe.PrepareTransfers.Web.Services
{
    public class ReferenceNumberService : IReferenceNumberService
    {
       public string GenerateReferenceNumber(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            string referenceNumber = "SAT";
            if (project.TransferringAcademies.Count > 1)
            {
                referenceNumber = "MAT";
            }

            return $"{referenceNumber}-{project.Urn}";
        }
    }
}