using System;
using System.Threading.Tasks;
using API.Repositories;
using API.Repositories.Interfaces;
using Data;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;

namespace Frontend.Services
{
    public class GetInformationForProject : IGetInformationForProject
    {
        private readonly IProjectsRepository _projectsRepository;
        private readonly IAcademiesRepository _dynamicsAcademiesRepository;
        private readonly IAcademies _academiesRepository;

        public GetInformationForProject(IProjectsRepository projectsRepository,
            IAcademiesRepository dynamicsAcademiesRepository, IAcademies academiesRepository)
        {
            _projectsRepository = projectsRepository;
            _dynamicsAcademiesRepository = dynamicsAcademiesRepository;
            _academiesRepository = academiesRepository;
        }

        public async Task<GetInformationForProjectResponse> Execute(Guid projectId)
        {
            var projectResult = await _projectsRepository.GetProjectById(projectId);
            var projectAcademyId = projectResult.Result.ProjectAcademies[0].AcademyId;
            var dynamicsAcademyResult = await _dynamicsAcademiesRepository.GetAcademyById(projectAcademyId);
            var dynamicsAcademy = dynamicsAcademyResult.Result;
            var academyResult = await _academiesRepository.GetAcademyByUkprn(dynamicsAcademy.Ukprn);
            
            return new GetInformationForProjectResponse
            {
                Project = projectResult.Result,
                OutgoingAcademy = academyResult.Result
            };
        }
    }
}