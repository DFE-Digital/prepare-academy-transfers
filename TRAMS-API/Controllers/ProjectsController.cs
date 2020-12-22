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

        public ProjectsController(IProjectsRepository projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        [HttpGet]
        [Route("/projects/")]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _projectsRepository.GetAll();

            var debug = 0;

            return null;
        }

        [HttpGet]
        [Route("/projects/{projectId}")]
        public async Task<IActionResult> GetProjectById(Guid projectId)
        {
            var project = await _projectsRepository.GetProjectById(projectId);

            return null;
        }

        [HttpGet]
        [Route("/projects/{projectId}/academies")]
        public async Task<IActionResult> GetProjectAcademies()
        {
            return null;
        }

        [HttpGet]
        [Route("/projects/{projectId}/academies/{academyId}")]
        public async Task<IActionResult> GetProjectAcademy()
        {
            return null;
        }

        [HttpGet]
        [Route("/projects/{projectId}/trusts")]
        public async Task<IActionResult> GetProjectTrusts()
        {
            return null;
        }

        [HttpGet]
        [Route("/projects/{projectId}/trusts/{trustId}")]
        public async Task<IActionResult> GetProjectTrust()
        {
            //Returns full trust information
            return null;
        }
        
    }
}
