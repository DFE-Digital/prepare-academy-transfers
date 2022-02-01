using System;
using System.Linq;
using Data.Models;
using Frontend.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Frontend.Services
{
    public class ReferenceNumberService : IReferenceNumberService
    {
        private readonly IConfiguration _configuration;

        public ReferenceNumberService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public string GenerateReferenceNumber(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            string referenceNumber = "SAT";
            if (project.TransferringAcademies.Count > 1)
            {
                referenceNumber = "MAT";
            }

            var fullRegion = project.TransferringAcademies.First().IncomingTrustLeadRscRegion?.Trim();
            if (fullRegion != null)
            {
                var regionCode = _configuration.GetSection("LeadRscRegionCodes").GetChildren().First(c =>
                    c.Key.Equals(fullRegion, StringComparison.CurrentCultureIgnoreCase)).Value;
                return $"{regionCode}-{referenceNumber}-{project.Urn}";
            }

            return $"{referenceNumber}-{project.Urn}";


        }
    }
}