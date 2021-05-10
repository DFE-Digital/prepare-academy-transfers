using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models;

namespace Data
{
    public interface IProjects
    {
        public Task<RepositoryResult<List<ProjectSearchResult>>> GetProjects();
        public Task<RepositoryResult<Project>> GetByUrn(string urn);
        public Task<RepositoryResult<Project>> Update(Project project);
        public Task<RepositoryResult<Project>> Create(Project project);
    }
}