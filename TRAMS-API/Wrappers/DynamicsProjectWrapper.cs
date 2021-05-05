using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Upstream.Enums;
using API.Models.Upstream.Request;
using API.Models.Upstream.Response;
using API.Repositories.Interfaces;
using Data;
using Data.Models;
using Data.Models.Projects;

namespace API.Wrappers
{
    public class DynamicsProjectWrapper : IProjects
    {
        private readonly IProjectsRepository _projectsRepository;

        public DynamicsProjectWrapper(IProjectsRepository projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<RepositoryResult<Project>> GetByUrn(string urn)
        {
            var guid = Guid.Parse(urn);

            var result = await _projectsRepository.GetProjectById(guid);
            var project = result.Result;

            return new RepositoryResult<Project>
            {
                Result = MapDynamicsProject(project)
            };
        }

        private static Project MapDynamicsProject(GetProjectsResponseModel project)
        {
            return new Project
            {
                Urn = project.ProjectId.ToString(),
                Features = new TransferFeatures(),
                OutgoingTrustUkprn = project.ProjectTrusts[1].TrustId.ToString(),
                TransferringAcademies = new List<TransferringAcademies>
                {
                    new TransferringAcademies
                    {
                        OutgoingAcademyUkprn = project.ProjectAcademies[0].AcademyId.ToString(),
                        IncomingTrustUkprn = project.ProjectTrusts[0].TrustId.ToString()
                    }
                }
            };
        }

        public Task<RepositoryResult<Project>> Update(Project project)
        {
            throw new NotImplementedException();
        }

        public async Task<RepositoryResult<Project>> Create(Project project)
        {
            var dynamicsProject = ConvertProjectToDynamicsProject(project);
            var result = await _projectsRepository.InsertProject(dynamicsProject);

            if (result.Result == null)
            {
                return new RepositoryResult<Project>
                {
                    Error = result.Error
                };
            }

            project.Urn = result.Result.Value.ToString();
            return new RepositoryResult<Project>
            {
                Result = project
            };
        }

        private static PostProjectsRequestModel ConvertProjectToDynamicsProject(Project project)
        {
            var academies = project.TransferringAcademies.Select(projectAcademy => new PostProjectsAcademiesModel
            {
                AcademyId = Guid.Parse(projectAcademy.OutgoingAcademyUkprn),
                Trusts = new List<PostProjectsAcademiesTrustsModel>
                {
                    new PostProjectsAcademiesTrustsModel {TrustId = Guid.Parse(project.OutgoingTrustUkprn)}
                }
            }).ToList();

            var dynamicsProject = new PostProjectsRequestModel
            {
                ProjectInitiatorFullName = "academy",
                ProjectInitiatorUid = Guid.NewGuid().ToString(),
                ProjectAcademies = academies,
                ProjectStatus = ProjectStatusEnum.InProgress,
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel
                        {TrustId = Guid.Parse(project.TransferringAcademies[0].IncomingTrustUkprn)},
                    new PostProjectsTrustsModel {TrustId = Guid.Parse(project.OutgoingTrustUkprn)}
                }
            };
            return dynamicsProject;
        }
    }
}