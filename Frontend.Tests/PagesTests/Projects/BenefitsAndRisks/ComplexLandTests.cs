using System;
using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.Benefits;
using Frontend.Pages.Projects.BenefitsAndRisks;
using Frontend.Tests.Helpers;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Frontend.Tests.PagesTests.Projects.BenefitsAndRisks
{
    public class ComplexLandTests : BaseTests
    {
        private readonly ComplexLandAndBuilding _subject;

        public ComplexLandTests()
        {
            _subject = new ComplexLandAndBuilding(ProjectRepository.Object);
        }

        public class PostTests : ComplexLandTests
        {
            [Fact]
            public async void GivenUrnAndDescription_UpdateTheProject()
            {
                _subject.Answer = "Complex Land";
                _subject.Urn = ProjectUrn0001;

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Project>(project => project.Benefits.OtherFactors.ContainsValue(_subject.Answer)
                                                       && project.Benefits.OtherFactors.ContainsKey(TransferBenefits
                                                           .OtherFactor.ComplexLandAndBuildingIssues)))
                );
            }
        }
    }
}