﻿using System;
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

            return $"{referenceNumber}-{project.Urn}";
        }
    }
}