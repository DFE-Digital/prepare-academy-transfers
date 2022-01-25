using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Data.Models.Projects;
using Data.TRAMS;
using Frontend.Services;
using Frontend.Services.Interfaces;
using Frontend.Tests.PagesTests;
using Moq;
using Xunit;
using Index = Frontend.Pages.Projects.Index;

namespace Frontend.Tests.ServicesTests
{
    public class ReferenceNumberServiceTests : BaseTests
    {
        private readonly IReferenceNumberService _referenceNumberService;

        public ReferenceNumberServiceTests()
        {
            //_referenceNumberService = new ReferenceNumberService();
        }

        [Fact]
        public void GivenNoProject_GetReferenceNumber_ThrowsArgumentNullException()
        {
            var exception =
                Assert.Throws<ArgumentNullException>(() => _referenceNumberService.GetReferenceNumber(null));
            Assert.Equal("project", exception.ParamName);
        }
        
        [Fact]
        public void GivenProjectNullRegion_GetReferenceNumber_ReturnsWithoutRegion()
        {
            var referenceNumber = _referenceNumberService.GetReferenceNumber(null);
            
        }
    }
}