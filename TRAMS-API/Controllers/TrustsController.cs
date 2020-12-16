using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Mapping;
using API.Models.D365;
using API.Models.Response;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TRAMS_API.Controllers
{
    /// <summary>
    /// API controller for retrieving information about a trust from TRAMS
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public class TrustsController : ControllerBase
    {
        private readonly ILogger<TrustsController> _logger;
        private readonly TrustsRepository _trustRepostiory;
        private readonly AcademiesRepository _academiesRepository;
        private readonly IMapper<GetTrustsD365Model, GetTrustsModel> _getTrustMapper;
        private readonly IMapper<GetAcademiesD365Model, GetAcademiesModel> _getAcademiesMapper;

        public TrustsController(ILogger<TrustsController> logger, 
                                         IConfiguration config,
                                         TrustsRepository trustRepostiory,
                                         AcademiesRepository academiesRepository,
                                         IMapper<GetTrustsD365Model, GetTrustsModel> mapper,
                                         IMapper<GetAcademiesD365Model, GetAcademiesModel> getAcademiesMapper)
        {
            _logger = logger;
            _trustRepostiory = trustRepostiory;
            _academiesRepository = academiesRepository;
            _getTrustMapper = mapper;
            _getAcademiesMapper = getAcademiesMapper;
        }

        /// <summary>
        /// Retrieves information about a trust given its TRAMS Guid
        /// </summary>
        /// <param name="id">The GUID</param>
        /// <returns><see cref="GetTrustsModel"/>A GetTrustsModel object</returns>
        [HttpGet]
        [Route("/trusts/{id}")]
        [ProducesResponseType(typeof(GetTrustsModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetTrustsModel>> GetById(Guid id)
        {
            var result = await _trustRepostiory.GetTrustById(id);

            if (result == null)
            {
                return NotFound($"The trust with the id: '{id}' was not found");
            }

            var formattedResult = _getTrustMapper.Map(result);
            
            return Ok(formattedResult);
        }

        /// <summary>
        /// Get all trusts or search via query parameters
        /// </summary>
        /// <param name="search">Will search for trusts that contain the search query. The searchable fields are: Name, Companies House Number, and Trust Reference Number</param>
        /// <returns><see cref="GetTrustsModel"/>An array of GetTrustsModel objects</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<GetTrustsModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GetTrustsModel>>> SearchTrusts(string search)
        {
            var results = await _trustRepostiory.SearchTrusts(search);

            var formattedOutput = results.Select(r => _getTrustMapper.Map(r)).ToList();

            return Ok(formattedOutput);
        }

        [HttpGet]
        [Route("/trusts/{id}/academies")]
        public async Task<ActionResult<List<GetAcademiesModel>>> GetTrustAcademies(Guid id)
        {
            var trust = await _trustRepostiory.GetTrustById(id);

            if (trust==null)
            {
                return NotFound($"The trust with the id: '{id}' was not found");
            }

            var academies = await _academiesRepository.GetAcademiesByTrustId(id);

            var formattedOutput = academies.Select(a => _getAcademiesMapper.Map(a)).ToList();

            return Ok(formattedOutput);
        }
    }
}
