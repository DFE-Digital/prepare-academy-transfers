using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Pages.Projects.BenefitsAndRisks;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.Projects.BenefitsAndRisks
{
    public class HighProfileTransferTests : BaseTests
    {
        private readonly HighProfileTransfer _subject;

        public HighProfileTransferTests()
        {
            _subject = new HighProfileTransfer(ProjectRepository.Object);
        }

        public class PostTests : HighProfileTransferTests
        {
            [Theory]
            [InlineData(null)]
            [InlineData("High profile")]
            public async void GivenUrnAndDescription_UpdateTheProject(string answer)
            {
                _subject.Answer = answer;
                _subject.Urn = ProjectUrn0001;

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.UpdateBenefits(It.Is<Project>(project => project.Benefits.OtherFactors.ContainsValue(_subject.Answer ?? string.Empty)
                                                       && project.Benefits.OtherFactors.ContainsKey(TransferBenefits
                                                           .OtherFactor.HighProfile)))
                );
            }
        }
    }
}