using System;
using System.Threading.Tasks;
using API.Models.Upstream.Request;
using API.Models.Upstream.Response;
using API.Models.Upstream.Enums;
using Data;

namespace API.Repositories.Interfaces
{
    public interface IProjectsRepository
    {
        public Task<RepositoryResult<SearchProjectsPageModel>> SearchProject(string searchQuery,
                                                                                 ProjectStatusEnum? status,
                                                                                 bool isAscending = true,
                                                                                 uint pageSize = 10,
                                                                                 uint pageNumber = 1);

        public Task<RepositoryResult<GetProjectsResponseModel>> GetProjectById(Guid id);

        public Task<RepositoryResult<Guid?>> InsertProject(PostProjectsRequestModel project);

        public Task<RepositoryResult<GetProjectsAcademyResponseModel>> GetProjectAcademyById(Guid id);

        public Task<RepositoryResult<Guid?>> UpdateProjectAcademy(Guid id, PutProjectAcademiesRequestModel model);
    }
}