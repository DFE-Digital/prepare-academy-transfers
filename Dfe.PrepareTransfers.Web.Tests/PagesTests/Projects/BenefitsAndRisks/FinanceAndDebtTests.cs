using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Pages.Projects.BenefitsAndRisks;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.BenefitsAndRisks
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
            [Theory]
            [InlineData(null)]
            [InlineData("Finance Risk")]
            public async void GivenUrnAndDescription_UpdateTheProject(string answer)
            {
                _subject.Answer = answer;
                _subject.Urn = ProjectUrn0001;

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Project>(project => project.Benefits.OtherFactors.ContainsValue(_subject.Answer ?? string.Empty)
                                                       && project.Benefits.OtherFactors.ContainsKey(TransferBenefits
                                                           .OtherFactor.FinanceAndDebtConcerns)))
                );
            }
        }
    }
}