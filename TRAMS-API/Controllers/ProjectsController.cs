﻿using API.Mapping;
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
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public class ProjectsController : Controller
    {
        private readonly IProjectsRepository _projectsRepository;
        private readonly IAcademiesRepository _academiesRepository;
        private readonly ITrustsRepository _trustsRepository;
        private readonly IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model> _mapper;

        public ProjectsController(IProjectsRepository projectsRepository,
                                  IAcademiesRepository academiesRepository,
                                  ITrustsRepository trustsRepository,
                                  IMapper<PostProjectsRequestModel, PostAcademyTransfersProjectsD365Model> mapper)
        {
            _projectsRepository = projectsRepository;
            _academiesRepository = academiesRepository;
            _trustsRepository = trustsRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("/projects/")]
        public async Task<IActionResult> InsertTrust([FromBody]PostProjectsRequestModel model)
        {
            var debug = 0;

            var projectAcademiesIds = model.ProjectAcademies.Select(a => a.AcademyId).ToList();

            foreach(var academyId in projectAcademiesIds)
            {
                var academy = await _academiesRepository.GetAcademyById(academyId);

                if (academy == null)
                {
                    return new UnprocessableEntityObjectResult($"No academy found with the id of: {academyId}");
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
                    var trust = _trustsRepository.GetTrustById(trustId);

                    if (trust == null)
                    {
                        return new UnprocessableEntityObjectResult($"No trust found with the id of: {trustId}");
                    }
                }
            }

            var internalModel = _mapper.Map(model);

            await _projectsRepository.InsertProject(internalModel);

            return Accepted();
        }   
    }
}