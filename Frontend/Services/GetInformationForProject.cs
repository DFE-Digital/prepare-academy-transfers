using System.Net;
using System.Threading.Tasks;
using Data;
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

            if (!projectResult.IsValid)
            {
                return CreateErrorResponse(projectResult);
            }

            var outgoingAcademyUkprn = projectResult.Result.TransferringAcademies[0].OutgoingAcademyUkprn;
            var academyResult = await _academiesRepository.GetAcademyByUkprn(outgoingAcademyUkprn);

            if (!academyResult.IsValid)
            {
                return CreateErrorResponse(academyResult);
            }

            var outgoingAcademyUrn = projectResult.Result.TransferringAcademies[0].OutgoingAcademyUrn;
            var educationPerformanceResult =
                await _educationPerformanceRepository.GetByAcademyUrn(outgoingAcademyUrn);

            if (!educationPerformanceResult.IsValid)
            {
                return CreateErrorResponse(educationPerformanceResult);
            }

            return new GetInformationForProjectResponse
            {
                Project = projectResult.Result,
                OutgoingAcademy = academyResult.Result,
                EducationPerformance = educationPerformanceResult.Result
            };
        }

        private static GetInformationForProjectResponse CreateErrorResponse<T>(RepositoryResult<T> repositoryResult)
        {
            if (repositoryResult.Error.StatusCode == HttpStatusCode.NotFound)
            {
                return new GetInformationForProjectResponse
                {
                    ResponseError = new ServiceResponseError
                    {
                        ErrorCode = ErrorCode.NotFound,
                        ErrorMessage = "Not found"
                    }
                };
            }

            return new GetInformationForProjectResponse
            {
                ResponseError = new ServiceResponseError
                {
                    ErrorCode = ErrorCode.ApiError,
                    ErrorMessage = "API has encountered an error"
                }
            };
        }
    }
}