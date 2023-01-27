using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.KeyStagePerformance;
using Dfe.PrepareTransfers.Web.Pages.TaskList;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Dfe.PrepareTransfers.Web.Tests.PagesTests.TaskList
{
    public class SchoolDataTests : BaseTests
    {
        private readonly SchoolData _subject;
        private Mock<IAcademies> Academies { get; } = new Mock<IAcademies>();

        private Mock<IEducationPerformance> ProjectRepositoryEducationPerformance { get; } =
            new Mock<IEducationPerformance>();

        public SchoolDataTests()
        {
            Academies.Setup(s => s.GetAcademyByUkprn(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResult<Academy> {Result = new Academy()});
            ProjectRepositoryEducationPerformance.Setup(S => S.GetByAcademyUrn(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryResult<EducationPerformance>
                {
                    Result = new EducationPerformance()
                });
            
            _subject = new SchoolData(Academies.Object, ProjectRepository.Object,
                ProjectRepositoryEducationPerformance.Object)
            {
                Urn = ProjectUrn0001,
                AcademyUkprn = "academyukprn"
            };
        }

        [Fact]
        public async void GivenUrn_FetchesProjectFromTheRepository()
        {
            await _subject.OnGetAsync();
            ProjectRepository.Verify(r => r.GetByUrn(ProjectUrn0001), Times.Once);
        }

        [Fact]
        public async void GivenUrn_AssignsModelToThePage()
        {
            var response = await _subject.OnGetAsync();
            Assert.IsType<PageResult>(response);
            Assert.Equal(ProjectUrn0001, _subject.Urn);
        }
        
        [Fact]
        public async void GivenAcademyUkprn_FetchesAcademyFromTheRepository()
        {
            await _subject.OnGetAsync();
            Academies.Verify(r => r.GetAcademyByUkprn(_subject.AcademyUkprn), Times.Once);
        }
        
        [Fact]
        public async void OnGet_FetchesPerformanceDataFromTheRepository()
        {
            await _subject.OnGetAsync();
            ProjectRepositoryEducationPerformance.Verify(r => r.GetByAcademyUrn(AcademyUrn), Times.Once);
        }
    }
}