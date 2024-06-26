﻿using System;
using System.Collections.Generic;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Services;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.ServicesTests
{
    public class ReferenceNumberServiceTests : BaseTests
    {
        private readonly IReferenceNumberService _referenceNumberService;

        public ReferenceNumberServiceTests()
        {
            _referenceNumberService = new ReferenceNumberService();
        }

        [Fact]
        public void GivenNoProject_GenerateReferenceNumber_ThrowsArgumentNullException()
        {
            var exception =
                Assert.Throws<ArgumentNullException>(() => _referenceNumberService.GenerateReferenceNumber(null));
            Assert.Equal("project", exception.ParamName);
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
                    TransferringAcademies = new List<TransferringAcademy>()
                    {
                        new()
                        {
                            IncomingTrustName = "Incoming Trust"
                        },
                        new()
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
                    TransferringAcademies = new List<TransferringAcademy>()
                    {
                        new()
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
            {
                Assert.Contains("MAT", referenceNumber);
            }

            if (project.TransferringAcademies.Count == 1)
            {
                Assert.Contains("SAT", referenceNumber);
            }
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
        public void GivenProject_GenerateReferenceNumber_ReturnsWithCorrectReference()
        {           
            var referenceNumber = _referenceNumberService.GenerateReferenceNumber(FoundProjectFromRepo);
            Assert.Equal($"SAT-{FoundProjectFromRepo.Urn}",referenceNumber);
        }
    }
}