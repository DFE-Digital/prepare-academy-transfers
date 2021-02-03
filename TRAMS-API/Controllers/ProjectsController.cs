using API.Mapping;
using API.Models.Downstream.D365;
using API.Models.Upstream.Enums;
using API.Models.Upstream.Request;
using API.Models.Upstream.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    /// <summary>
    /// API controller for retrieving Academy Transfers projects from TRAMS
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status502BadGateway)]
    public class ProjectsController : Controller
    {
        private readonly IProjectsRepository _projectsRepository;
        private readonly IAcademiesRepository _academiesRepository;
        private readonly ITrustsRepository _trustsRepository;
        private readonly IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model> _postProjectsMapper;
        private readonly IMapper<GetProjectsD365Model, GetProjectsResponseModel> _getProjectsMapper;
        private readonly IMapper<AcademyTransfersProjectAcademy, 
                                 Models.Upstream.Response.GetProjectsAcademyResponseModel> _getProjectAcademyMapper;
        private readonly IMapper<PutProjectAcademiesRequestModel, PatchProjectAcademiesD365Model> _putProjectAcademiesMapper;
        private readonly IMapper<SearchProjectsD365PageModel, SearchProjectsPageModel> _searchProjectsMapper;
        private readonly IRepositoryErrorResultHandler _repositoryErrorHandler;
        private readonly IConfiguration _config;

        public ProjectsController(IProjectsRepository projectsRepository,
                                  IAcademiesRepository academiesRepository,
                                  ITrustsRepository trustsRepository,
                                  IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model> postProjectsMapper,
                                  IMapper<GetProjectsD365Model, GetProjectsResponseModel> getProjectsMapper,
                                  IMapper<AcademyTransfersProjectAcademy, Models.Upstream.Response.GetProjectsAcademyResponseModel> getProjectAcademyMapper,
                                  IMapper<PutProjectAcademiesRequestModel, PatchProjectAcademiesD365Model> putProjectAcademiesMapper,
                                  IMapper<SearchProjectsD365PageModel, SearchProjectsPageModel> searchProjectsMapper,
                                  IRepositoryErrorResultHandler repositoryErrorHandler,
                                  IConfiguration config)
        {
            _projectsRepository = projectsRepository;
            _academiesRepository = academiesRepository;
            _trustsRepository = trustsRepository;
            _postProjectsMapper = postProjectsMapper;
            _putProjectAcademiesMapper = putProjectAcademiesMapper;
            _getProjectsMapper = getProjectsMapper;
            _getProjectAcademyMapper = getProjectAcademyMapper;
            _searchProjectsMapper = searchProjectsMapper;
            _repositoryErrorHandler = repositoryErrorHandler;
            _config = config;
        }

        /// <summary>
        /// Search for Academy Transfer Projects.
        /// </summary>
        /// <param name="searchTerm">The search term. The searched fields will be: 
        /// Project Name
        /// Outgoing Trust Name, 
        /// Outgoing Trust Companies House Number
        /// Academy Name</param>
        /// <param name="status">The project status:
        /// 1. In Progress
        /// 2. Completed</param>
        /// <param name="ascending">Determines if the results should be returned in ascending order. Default value is: true</param>
        /// <param name="pageSize">The number of items to be returned per page. Must be larger than zero. Default value is: 10</param>
        /// <param name="pageNumber">The page number to be returned. Must be larger than zero. Default value is: 1</param>
        /// <returns></returns>
        [HttpGet]
        [Route("/projects")]
        [ProducesResponseType(typeof(SearchProjectsPageModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchProjects(string searchTerm, 
                                                        ProjectStatusEnum? status,
                                                        bool? ascending,
                                                        uint? pageSize,
                                                        uint? pageNumber)
        {
            var ascendingOption = ascending ?? true;
            var pageSizeOption = pageSize ?? 10;
            var pageNumberOption = pageNumber ?? 1;

            if (pageSizeOption == 0)
            {
                return BadRequest("Page size cannot be zero");
            }

            if (pageNumberOption == 0)
            {
                return BadRequest("Page number cannot be 0");
            }

            Models.D365.Enums.ProjectStatusEnum projectStatus = default;

            if (status.HasValue)
            {
                if (MappingDictionaries.ProjecStatusEnumMap.TryGetValue(status.Value, out var internalStatus))
                {
                    projectStatus = internalStatus;
                }
                else
                {
                    //If project status cannot be mapped to D365 Project Status, return an error
                    return BadRequest("Project Status not recognised");
                }
            }
                
            var projSearchResult = await _projectsRepository.SearchProject(searchTerm, 
                                                                           projectStatus,
                                                                           ascendingOption,
                                                                           pageSizeOption,
                                                                           pageNumberOption);

            if (!projSearchResult.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(projSearchResult);
            }

            var externalModels = _searchProjectsMapper.Map(projSearchResult.Result);

            return Ok(externalModels);
        }

        /// <summary>
        /// Gets an Academy Transfers project by its ID
        /// </summary>
        /// <param name="id">The ID of the project</param>
        /// <returns></returns>
        [HttpGet]
        [Route("/projects/{id}")]
        [ProducesResponseType(typeof(GetProjectsResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var getProjectRepositoryResult = await _projectsRepository.GetProjectById(id);

            if (!getProjectRepositoryResult.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(getProjectRepositoryResult);
            }

            if (getProjectRepositoryResult.Result == null)
            {
                return NotFound($"Project with id '{id}' not found");
            }

            var externalModel = _getProjectsMapper.Map(getProjectRepositoryResult.Result);

            return Ok(externalModel);
        }

        /// <summary>
        /// Gets a Project Academy entity by its id
        /// </summary>
        /// <param name="projectId">The ID of the project</param>
        /// <param name="projectAcademyId">The ID of the ProjectAcademy entity - this is the id of the custom entity, not the ID of the academy in TRAMS</param>
        /// <returns></returns>
        [HttpGet]
        [Route("/projects/{projectId}/academies/{projectAcademyId}")]
        [ProducesResponseType(typeof(GetProjectsAcademyResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProjectAcademy(Guid projectId, Guid projectAcademyId)
        {
            var getProjectResult = await _projectsRepository.GetProjectById(projectId);

            if (!getProjectResult.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(getProjectResult);
            }

            if (getProjectResult.Result == null)
            {
                return NotFound($"Project with id '{projectId}' not found");
            }

            var getProjectRepositoryResult = await _projectsRepository.GetProjectAcademyById(projectAcademyId);

            if (!getProjectRepositoryResult.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(getProjectRepositoryResult);
            }

            if (getProjectRepositoryResult.Result == null)
            {
                return NotFound($"Project Academy with id '{projectAcademyId}' not found");
            }

            var externalModel = _getProjectAcademyMapper.Map(getProjectRepositoryResult.Result);

            return Ok(externalModel);
        }

        /// <summary>
        /// API endpoint for creating a new Academy Transfers Project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/projects/")]
        [ProducesResponseType(typeof(GetProjectsResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> InsertProject([FromBody]PostProjectsRequestModel model)
        {
            var projectAcademiesIds = model.ProjectAcademies?.Select(a => a.AcademyId).ToList();
            var unprocessableEntityErrors = new List<string>();

            #region Check Referenced Entities

            foreach (var academyId in projectAcademiesIds)
            {
                var academyRepoResult = await _academiesRepository.GetAcademyById(academyId);

                if (!academyRepoResult.IsValid)
                {
                    return _repositoryErrorHandler.LogAndCreateResponse(academyRepoResult);
                }

                if (academyRepoResult.Result == null)
                {
                    unprocessableEntityErrors.Add($"No academy found with the id of: {academyId}");
                }
            }

            var allTrustIds = new List<Guid>();

            if (model.ProjectAcademies != null && model.ProjectAcademies.Any())
            {
                allTrustIds.AddRange(model.ProjectAcademies.Where(a => a.Trusts != null && a.Trusts.Any())
                                                           .SelectMany(s => s.Trusts)
                                                           .Select(s => s.TrustId));
            }

            if (model.ProjectTrusts != null && model.ProjectTrusts.Any())
            {
                allTrustIds.AddRange(model.ProjectTrusts.Select(p => p.TrustId));
            }

            if (allTrustIds.Any())
            {
                foreach (var trustId in allTrustIds.Distinct())
                {
                    var trustsRepositoryResult = await _trustsRepository.GetTrustById(trustId);

                    if (!trustsRepositoryResult.IsValid)
                    {
                        return _repositoryErrorHandler.LogAndCreateResponse(trustsRepositoryResult);
                    }

                    if (trustsRepositoryResult.Result == null)
                    {
                        unprocessableEntityErrors.Add($"No trust found with the id of: {trustId}");
                    }
                }
            }

            #endregion

            //If errors are detected with entity referencing, return an UnprocessableEntity result
            if (unprocessableEntityErrors.Any())
            {
                var error = unprocessableEntityErrors.ToDelimitedString(". ");

                return UnprocessableEntity(error);
            }

            var internalModel = _postProjectsMapper.Map(model);

            var insertProjectRepositoryResult = await _projectsRepository.InsertProject(internalModel);

            if (!insertProjectRepositoryResult.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(insertProjectRepositoryResult);
            }

            var getProjectRepositoryResult = await _projectsRepository.GetProjectById(insertProjectRepositoryResult.Result.Value);

            if (!getProjectRepositoryResult.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(getProjectRepositoryResult);
            }

            var externalModel = _getProjectsMapper.Map(getProjectRepositoryResult.Result);

            var apiBaseUrl = _config["API:Url"];

            return Created($"{apiBaseUrl}/projects/{externalModel.ProjectId}", externalModel);
        }
        
        /// <summary>
        /// Endpoint for updating a Project Academy entity
        /// </summary>
        /// <param name="projectId">THe Academy Transfers Project where the Project Academy entity is nested</param>
        /// <param name="projectAcademyId">The ID of the Project Academy entity. Note: this is the id of the join-table entity, not the id of the academy</param>
        /// <param name="model">The updated Project Academy entity</param>
        /// <returns></returns>
        [HttpPut]
        [Route("/projects/{projectId}/academies/{projectAcademyId}")]
        [ProducesResponseType(typeof(GetProjectsAcademyResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateProjectAcademy(Guid projectId, Guid projectAcademyId, [FromBody]PutProjectAcademiesRequestModel model)
        {
            //Verify that the Id of the ProjectAcademy entity is correct before updating the object
            var projectAcademyRepoResult = await _projectsRepository.GetProjectAcademyById(projectAcademyId);

            if (!projectAcademyRepoResult.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(projectAcademyRepoResult);
            }

            if (projectAcademyRepoResult.Result == null)
            {
                return NotFound($"Project Academy with id '{projectAcademyId}' not found");
            }

            //If the ProjectAcademy doesn't belong to the specified project, return a 422
            if (projectAcademyRepoResult.Result.ProjectId != projectId)
            {
                return UnprocessableEntity($"Project Academy with id '{projectAcademyId}' not found within project with id '{projectId}'");
            }

            //Verify that the referenced academy for the ProjectAcademy entity is correct
            var referencedAcademyResult = await _academiesRepository.GetAcademyById(model.AcademyId);

            if (!referencedAcademyResult.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(referencedAcademyResult);
            }

            if (referencedAcademyResult.Result == null)
            {
                return UnprocessableEntity($"No academy found with the id of: {model.AcademyId}");
            }

            //Map the model to internal representation before updating the entity
            var internalModel = _putProjectAcademiesMapper.Map(model);

            var updateProjectAcademyResult = await _projectsRepository.UpdateProjectAcademy(projectAcademyId, internalModel);

            if (!updateProjectAcademyResult.IsValid || !updateProjectAcademyResult.Result.HasValue)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(updateProjectAcademyResult);
            }

            //Once the entity has been modified, obtain the full entity from D365
            var retrievedFullProjectAcademy = await _projectsRepository.GetProjectAcademyById(updateProjectAcademyResult.Result.Value);

            if (!retrievedFullProjectAcademy.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(retrievedFullProjectAcademy);
            }

            //Map the ProjectAcademy representation to the external model
            var externalModel = _getProjectAcademyMapper.Map(retrievedFullProjectAcademy.Result);

            return Ok(externalModel);
        }
    }
}