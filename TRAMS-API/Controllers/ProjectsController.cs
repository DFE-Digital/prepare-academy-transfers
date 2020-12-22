using API.Models.D365;
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

        [HttpPost]
        [Route("/projects/")]
        public async Task<IActionResult> InsertTrust()
        {
            var project = new PostAcademyTransfersProjectsD365Model
            {
                ProjectInitiatorFullName = "Mihail Andrici",
                ProjectInitiatorUid = "mihail.andrici@education.gov.uk",
                ProjectStatus = 596500000,
                Academies = new List<PostAcademyTransfersProjectAcademyD365Model>
                {
                    new PostAcademyTransfersProjectAcademyD365Model
                    {
                        AcademyId = "/accounts(26fad515-0ede-e911-a839-000d3a385a1c)",
                        Trusts = new List<PostAcademyTransfersProjectAcademyTrustD365Model>
                        {
                            new PostAcademyTransfersProjectAcademyTrustD365Model
                            {
                                TrustId = "/accounts(26fad515-0ede-e911-a839-000d3a385a1c)"
                            }
                        }
                    },
                    new PostAcademyTransfersProjectAcademyD365Model
                    {
                        AcademyId = "/accounts(9be14625-eaa0-e911-a837-000d3a385a1c)",
                        Trusts = new List<PostAcademyTransfersProjectAcademyTrustD365Model>
                        {
                            new PostAcademyTransfersProjectAcademyTrustD365Model
                            {
                                TrustId = "/accounts(9be14625-eaa0-e911-a837-000d3a385a1c)"
                            }
                        }
                    }
                },
                Trusts = new List<PostAcademyTransfersProjectTrustD365Model>()
            };

            await _projectsRepository.InsertProject(project);

            return null;
        }
        
    }
}
