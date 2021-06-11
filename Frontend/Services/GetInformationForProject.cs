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

        public GetInformationForProject(IAcademies academiesRepository,
            IProjects projectsRepository)
        {
            _academiesRepository = academiesRepository;
            _projectsRepository = projectsRepository;
        }

        public async Task<GetInformationForProjectResponse> Execute(string projectUrn)
        {
            var projectResult = await _projectsRepository.GetByUrn(projectUrn);
            var outgoingAcademyUkprn = projectResult.Result.TransferringAcademies[0].OutgoingAcademyUkprn;
            var academyResult = await _academiesRepository.GetAcademyByUkprn(outgoingAcademyUkprn);

            if (academyResult.IsValid)
            {
                return new GetInformationForProjectResponse
                {
                    Project = projectResult.Result,
                    OutgoingAcademy = academyResult.Result
                };
            }

            if (academyResult.Error.StatusCode == HttpStatusCode.NotFound)
            {
                return new GetInformationForProjectResponse
                {
                    ResponseError = new ServiceResponseError
                    {
                        ErrorCode = ErrorCode.NotFound,
                        ErrorMessage = "Outgoing academy not found"
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