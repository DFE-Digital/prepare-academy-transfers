using API.Models.Downstream.D365;
using System;
using System.Threading.Tasks;

namespace API.Repositories.Interfaces
{
    public interface IProjectsRepository
    {
        public Task<RepositoryResult<SearchProjectsD365PageModel>> SearchProject(string searchQuery,
                                                                                 ProjectStatusEnum status,
                                                                                 bool isAscending = true,
                                                                                 uint pageSize = 10,
                                                                                 uint pageNumber = 1);

        public Task<RepositoryResult<GetProjectsD365Model>> GetProjectById(Guid id);

        public Task<RepositoryResult<Guid?>> InsertProject(PostAcademyTransfersProjectsD365Model project);

        public Task<RepositoryResult<AcademyTransfersProjectAcademy>> GetProjectAcademyById(Guid id);

        public Task<RepositoryResult<Guid?>> UpdateProjectAcademy(Guid id, PatchProjectAcademiesD365Model model);
    }
}