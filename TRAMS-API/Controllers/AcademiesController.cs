using API.Mapping;
using API.Models.D365;
using API.Models.Response;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public class AcademiesController : ControllerBase
    {
        private readonly IAcademiesRepository _academiesRepository;
        private readonly IMapper<GetAcademiesD365Model, GetAcademiesModel> _getAcademiesMapper;
        private readonly ILogger _logger;

        public AcademiesController(IAcademiesRepository academiesRepository,
                                   IMapper<GetAcademiesD365Model, GetAcademiesModel> getAcademiesMapper,
                                   ILogger logger)
        {
            _academiesRepository = academiesRepository;
            _getAcademiesMapper = getAcademiesMapper;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves information about an academy given its TRAMS Guid
        /// </summary>
        /// <param name="id">The Guid of the academy</param>
        [HttpGet]
        [Route("/academies/{id}")]
        [ProducesResponseType(typeof(GetAcademiesModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetAcademiesModel>> GetAcademyById(Guid id)
        {
            var repoResult = await _academiesRepository.GetAcademyById(id);

            if (repoResult.Error != null)
            {
                _logger.LogError("Downstream API failed with status code of {0}. Error: {1}", 
                                 repoResult.Error.StatusCode, repoResult.Error.ErrorMessage);

                if (repoResult.Error.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    return StatusCode(429, "Too many requests, please try again later");
                }
            
                return StatusCode(502, repoResult.Error.ErrorMessage);
            }

            if (repoResult.Result == null)
            {
                return NotFound($"The academy with the id: '{id}' was not found");
            }

            var formattedResult = _getAcademiesMapper.Map(repoResult.Result);

            return Ok(formattedResult);
        }
    }
}
