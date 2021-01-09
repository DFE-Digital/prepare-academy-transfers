using API.Mapping;
using API.Models.D365;
using API.Models.Request;
using API.Repositories;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [ProducesResponseType(typeof(string), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public class ProjectsController : Controller
    {
        private readonly IProjectsRepository _projectsRepository;
        private readonly IAcademiesRepository _academiesRepository;
        private readonly ITrustsRepository _trustsRepository;
        private readonly IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model> _mapper;
        private readonly IRepositoryErrorResultHandler _repositoryErrorHandler;

        public ProjectsController(IProjectsRepository projectsRepository,
                                  IAcademiesRepository academiesRepository,
                                  ITrustsRepository trustsRepository,
                                  IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model> mapper,
                                  IRepositoryErrorResultHandler repositoryErrorHandler)
        {
            _projectsRepository = projectsRepository;
            _academiesRepository = academiesRepository;
            _trustsRepository = trustsRepository;
            _mapper = mapper;
            _repositoryErrorHandler = repositoryErrorHandler;
        }

        [HttpGet]
        [Route("/projects/{id}")]
        [ProducesResponseType(typeof(GetAcademyTransfersProjectsD365Model), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTrustById(Guid id)
        {
            var getProjectRepositoryResult = await _projectsRepository.GetProjectById(id);

            if (!getProjectRepositoryResult.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(getProjectRepositoryResult);
            }

            return Ok(getProjectRepositoryResult.Result);
        }

        /// <summary>
        /// API endpoint for creating a new Academy Transfers Project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/projects/")]
        [ProducesResponseType(typeof(GetAcademyTransfersProjectsD365Model), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> InsertTrust([FromBody]PostProjectsRequestModel model)
        {
            var projectAcademiesIds = model.ProjectAcademies.Select(a => a.AcademyId).ToList();
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
                var error = string.Join(". ", unprocessableEntityErrors);

                return UnprocessableEntity(error);
            }

            var internalModel = _mapper.Map(model);

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

            return Created("entityLocation", getProjectRepositoryResult.Result);
        }   
    }
}