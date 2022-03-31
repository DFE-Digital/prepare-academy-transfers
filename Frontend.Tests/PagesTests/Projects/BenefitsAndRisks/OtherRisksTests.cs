﻿using System;
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
    public class OtherRisksTests : BaseTests
    {
        private readonly OtherRisks _subject;

        public OtherRisksTests()
        {
            _subject = new OtherRisks(ProjectRepository.Object);
        }

        public class PostTests : OtherRisksTests
        {
            [Fact]
            public async void GivenUrnAndDescription_UpdateTheProject()
            {
                _subject.Answer = "Other risks";
                _subject.Urn = ProjectUrn0001;

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Project>(project => project.Benefits.OtherFactors.ContainsValue(_subject.Answer)
                                                       && project.Benefits.OtherFactors.ContainsKey(TransferBenefits
                                                           .OtherFactor.OtherRisks)))
                );
            }
        }
    }
}