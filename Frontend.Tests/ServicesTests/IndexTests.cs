using System.Collections.Generic;
using Data;
using Data.Models.KeyStagePerformance;
using Frontend.Services;
using Frontend.Tests.PagesTests;
using Moq;

namespace Frontend.Tests.ServicesTests
{
    public class TaskListServiceTests : PageTests
    {
        private readonly TaskListService _subject;

        public TaskListServiceTests()
        {
            var projectsRepository = new Mock<IProjects>();
            var educationPerformanceRepository = new Mock<IEducationPerformance>();
            educationPerformanceRepository.Setup(r => r.GetByAcademyUrn(AcademyUrn))
                .ReturnsAsync(new RepositoryResult<EducationPerformance>
                {
                    Result = new EducationPerformance
                    {
                        KeyStage2Performance = new List<KeyStage2>()
                    }
                });

            _subject = new TaskListService(projectsRepository.Object, educationPerformanceRepository.Object);
        }

        // [Fact]
        // public async void GivenGetByUrnReturnsError()
        // {
        //     Assert.Throws<NullReferenceException>(() =>  _subject.BuildTaskListStatuses(ProjectErrorUrn, _subject));
        // }
        
        // [Fact]
        // public async void GivenAProjectID_PutsTheOutgoingAcademyNameInTheViewModel()
        // {
        //     var viewModel = await _subject.BuildTaskListStatuses(ProjectUrn0001);
        //     Assert.Equal(OutgoingAcademyName, viewModel.ac.OutgoingTrustName);
        // }
    }
}