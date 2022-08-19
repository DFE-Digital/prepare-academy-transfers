using Data.Models;
using Data.Models.Projects;
using Frontend.Pages.Projects.BenefitsAndRisks;
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
            [Theory]
            [InlineData(null)]
            [InlineData("Complex Land")]
            public async void GivenUrnAndDescription_UpdateTheProject(string answer)
            {
                _subject.Answer = answer;
                _subject.Urn = ProjectUrn0001;

                await _subject.OnPostAsync();

                ProjectRepository.Verify(r =>
                    r.Update(It.Is<Project>(project => project.Benefits.OtherFactors.ContainsValue(_subject.Answer ?? string.Empty)
                                                       && project.Benefits.OtherFactors.ContainsKey(TransferBenefits
                                                           .OtherFactor.ComplexLandAndBuildingIssues)))
                );
            }
        }
    }
}