using System;
using System.Collections.Generic;
using System.Linq;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Benefits;
using Dfe.PrepareTransfers.Web.Pages.Projects.BenefitsAndRisks;
using Dfe.PrepareTransfers.Web.Tests.Dfe.PrepareTransfers.Helpers;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.BenefitsAndRisks
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
            [Theory]
            [InlineData(null)]
            [InlineData("Finance Risk")]
            public async void GivenUrnAndDescription_UpdateTheProject(string answer)
            {
                _subject.Answer = answer;
                _subject.Urn = ProjectUrn0001;

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.UpdateBenefits(It.Is<Project>(project =>
                        project.Benefits.OtherFactors.ContainsValue(_subject.Answer ?? string.Empty)
                        && project.Benefits.OtherFactors.ContainsKey(TransferBenefits
                            .OtherFactor.OtherRisks)))
                );
            }
        }
    }
}