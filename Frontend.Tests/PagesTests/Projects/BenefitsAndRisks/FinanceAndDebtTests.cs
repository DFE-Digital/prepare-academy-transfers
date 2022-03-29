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
    public class FinanceAndDebtTests : BaseTests
    {
        private readonly FinanceAndDebt _subject;

        public FinanceAndDebtTests()
        {
            _subject = new FinanceAndDebt(ProjectRepository.Object);
        }

        public class PostTests : FinanceAndDebtTests
        {
            [Fact]
            public async void GivenUrnAndDescription_UpdateTheProject()
            {
                _subject.Answer = "Finance and Debt";
                _subject.Urn = ProjectUrn0001;

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Project>(project => project.Benefits.OtherFactors.ContainsValue(_subject.Answer)
                                                       && project.Benefits.OtherFactors.ContainsKey(TransferBenefits
                                                           .OtherFactor.FinanceAndDebtConcerns)))
                );
            }
        }
    }
}