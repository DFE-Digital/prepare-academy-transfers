using System;
using System.Collections.Generic;
using Data.Models;
using Data.Models.Projects;
using Frontend.Services;
using Frontend.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Frontend.Tests.ServicesTests
{
    public class ReferenceNumberServiceTests : BaseTests
    {
        private readonly IReferenceNumberService _referenceNumberService;
        private readonly IConfigurationRoot _configuration;
        public ReferenceNumberServiceTests()
        {
            var config = new Dictionary<string, string>
            {
                {"LeadRscRegionCodes:London", "L"},
                {"LeadRscRegionCodes:North", "N"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();
            
            _referenceNumberService = new ReferenceNumberService(_configuration);
        }

        [Fact]
        public void GivenNoProject_GenerateReferenceNumber_ThrowsArgumentNullException()
        {
            var exception =
                Assert.Throws<ArgumentNullException>(() => _referenceNumberService.GenerateReferenceNumber(null));
            Assert.Equal("project", exception.ParamName);
        }
        
        [Fact]
        public void GivenNoTransferringAcademies_GenerateReferenceNumber_ThrowsException()
        {
            FoundProjectFromRepo.TransferringAcademies = new List<TransferringAcademies>(0);
            Assert.Throws<InvalidOperationException>(() => _referenceNumberService.GenerateReferenceNumber(FoundProjectFromRepo));
        }

        [Fact]
        public void GivenProject_NullRegion_GenerateReferenceNumber_ReturnsWithoutRegion()
        {
            var referenceNumber = _referenceNumberService.GenerateReferenceNumber(FoundProjectFromRepo);
            Assert.True(referenceNumber.StartsWith("MAT") || referenceNumber.StartsWith("SAT"));
        }

        public static IEnumerable<object[]> ProjectsSAT_and_MAT()
        {
            yield return new object[]
            {
                new Project
                {
                    Urn = ProjectUrn0001,
                    TransferringAcademies = new List<TransferringAcademies>()
                    {
                        new TransferringAcademies()
                        {
                            IncomingTrustName = "Incoming Trust"
                        },
                        new TransferringAcademies()
                        {
                            IncomingTrustName = "Incoming Trust Two"
                        },
                    }
                }
            };
            yield return new object[]
            {
                new Project
                {
                    Urn = ProjectUrn0001,
                    TransferringAcademies = new List<TransferringAcademies>()
                    {
                        new TransferringAcademies()
                        {
                            IncomingTrustName = "Incoming Trust"

                        }
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(ProjectsSAT_and_MAT))]
        public void GivenProject_WithAcademies_GenerateReferenceNumber_ReturnsWithType(Project project)
        {
            FoundProjectFromRepo = project;
            var referenceNumber = _referenceNumberService.GenerateReferenceNumber(FoundProjectFromRepo);
            if (project.TransferringAcademies.Count > 1)
                Assert.Contains("MAT", referenceNumber);
            if (project.TransferringAcademies.Count == 1)
                Assert.Contains("SAT", referenceNumber);
        }

        [Theory]
        [MemberData(nameof(ProjectsSAT_and_MAT))]
        public void GivenProject_SingleAcademy_GenerateReferenceNumber_ReturnsWithUrn(Project project)
        {
            FoundProjectFromRepo = project;
            var referenceNumber = _referenceNumberService.GenerateReferenceNumber(FoundProjectFromRepo);
            Assert.Contains(FoundProjectFromRepo.Urn, referenceNumber);
        }
        
        [Fact]
        public void GivenProject_GenerateReferenceNumber_ReturnsWithRegionCode()
        {
            var regionName = "london";
            FoundProjectFromRepo.TransferringAcademies = new List<TransferringAcademies>()
            {
                new TransferringAcademies
                {
                    IncomingTrustLeadRscRegion = regionName
                },
                new TransferringAcademies
                {
                    IncomingTrustLeadRscRegion = regionName
                }
            };
            var referenceNumber = _referenceNumberService.GenerateReferenceNumber(FoundProjectFromRepo);
            Assert.StartsWith(_configuration.GetSection("LeadRscRegionCodes")[regionName], referenceNumber);
        }

        [Fact]
        public void GivenProject_GenerateReferenceNumber_ReturnsWithCorrectReference()
        {
            var regionName = "london";
            FoundProjectFromRepo.TransferringAcademies = new List<TransferringAcademies>()
            {
                new TransferringAcademies
                {
                    IncomingTrustLeadRscRegion = regionName
                },
                new TransferringAcademies
                {
                    IncomingTrustLeadRscRegion = regionName
                }
            };
            var referenceNumber = _referenceNumberService.GenerateReferenceNumber(FoundProjectFromRepo);
            Assert.Equal($"L-MAT-{FoundProjectFromRepo.Urn}",referenceNumber);
        }
    }
}