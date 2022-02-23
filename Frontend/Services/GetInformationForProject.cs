using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;

namespace Frontend.Services
{
    public class GetInformationForProject : IGetInformationForProject
    {
        private readonly IProjects _projectsRepository;
        private readonly IAcademies _academiesRepository;
        private readonly IEducationPerformance _educationPerformanceRepository;

        public GetInformationForProject(IAcademies academiesRepository,
            IProjects projectsRepository, IEducationPerformance educationPerformanceRepository)
        {
            _academiesRepository = academiesRepository;
            _projectsRepository = projectsRepository;
            _educationPerformanceRepository = educationPerformanceRepository;
        }

        public async Task<GetInformationForProjectResponse> Execute(string projectUrn)
        {
            var projectResult = await _projectsRepository.GetByUrn(projectUrn);
            
            var outgoingAcademies = new List<Academy>();
            
            foreach (var academy in projectResult.Result.TransferringAcademies)
            {
                var academyResult = await _academiesRepository.GetAcademyByUkprn(academy.OutgoingAcademyUkprn);
                var outgoingAcademyUrn = academy.OutgoingAcademyUrn;
                var educationPerformanceResult =
                    await _educationPerformanceRepository.GetByAcademyUrn(outgoingAcademyUrn);
                var mappedAcademy = academyResult.Result;
                mappedAcademy.EducationPerformance = educationPerformanceResult.Result;
                outgoingAcademies.Add(academyResult.Result);
            }

            return new GetInformationForProjectResponse
            {
                Project = projectResult.Result,
                OutgoingAcademies = outgoingAcademies,
                // todo: remove this when other mat-mat work has been done
                OutgoingAcademy = outgoingAcademies.First(),
                EducationPerformance = outgoingAcademies.First().EducationPerformance
            };
        }
    }
}