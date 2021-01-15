using API.Models.D365;
using API.Models.Downstream.D365;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories.Interfaces
{
    public interface IProjectsRepository
    {
        public Task<List<GetProjectsD365Model>> GetAll(string academyId);

        public Task<RepositoryResult<GetProjectsD365Model>> GetProjectById(Guid id);

        public Task<RepositoryResult<Guid?>> InsertProject(PostAcademyTransfersProjectsD365Model project);

        public Task<RepositoryResult<AcademyTransfersProjectAcademy>> GetProjectAcademyById(Guid id);
    }
}
