using System.Collections.Generic;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models;

namespace Dfe.PrepareTransfers.Data
{
    public interface IProjects
    {
        public Task<RepositoryResult<List<ProjectSearchResult>>> GetProjects(int page = 1, string title = default,
           int pageSize = 10);
        public Task<RepositoryResult<Project>> GetByUrn(string urn);
        public Task<RepositoryResult<Project>> Update(Project project);
        public Task<RepositoryResult<Project>> Create(Project project);
    }
}