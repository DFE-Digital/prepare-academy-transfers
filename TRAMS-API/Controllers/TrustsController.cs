using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Mapping;
using API.Models.Downstream.D365;
using API.Models.Upstream.Response;
using API.Repositories;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API controller for retrieving information about a trust from TRAMS
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(string), StatusCodes.Status502BadGateway)]
    public class TrustsController : ControllerBase
    {
        private readonly ITrustsRepository _trustRepostiory;
        private readonly IAcademiesRepository _academiesRepository;
        private readonly IMapper<GetTrustsD365Model, GetTrustsModel> _getTrustMapper;
        private readonly IMapper<GetAcademiesD365Model, GetAcademiesModel> _getAcademiesMapper;
        private readonly IRepositoryErrorResultHandler _repositoryErrorHandler;

        public TrustsController(ITrustsRepository trustRepostiory,
                                IAcademiesRepository academiesRepository,
                                IMapper<GetTrustsD365Model, GetTrustsModel> mapper,
                                IMapper<GetAcademiesD365Model, GetAcademiesModel> getAcademiesMapper,
                                IRepositoryErrorResultHandler repositoryErrorHandler)
        {
            _trustRepostiory = trustRepostiory;
            _academiesRepository = academiesRepository;
            _getTrustMapper = mapper;
            _getAcademiesMapper = getAcademiesMapper;
            _repositoryErrorHandler = repositoryErrorHandler;
        }

        /// <summary>
        /// Retrieves information about a trust given its TRAMS Guid
        /// </summary>
        /// <param name="id">The Guid of the trust</param>
        /// <returns><see cref="GetTrustsModel"/>A GetTrustsModel object</returns>
        [HttpGet]
        [Route("/trusts/{id}")]
        [ProducesResponseType(typeof(GetTrustsModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetTrustsModel>> GetById(Guid id)
        {
            var trustsRepositoryResult = await _trustRepostiory.GetTrustById(id);

            if (!trustsRepositoryResult.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(trustsRepositoryResult);
            }

            if (trustsRepositoryResult.Result == null)
            {
                return NotFound($"The trust with the id: '{id}' was not found");
            }

            var formattedResult = _getTrustMapper.Map(trustsRepositoryResult.Result);

            return Ok(formattedResult);
        }

        /// <summary>
        /// Get all trusts or search via query parameters
        /// </summary>
        /// <param name="search">Will search for trusts that contain the search query. The searchable fields are: Name, Companies House Number, and Trust Reference Number</param>
        /// <returns><see cref="GetTrustsModel"/>An array of GetTrustsModel objects</returns>
        [HttpGet]
        [Route("/trusts/")]
        [ProducesResponseType(typeof(List<GetTrustsModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GetTrustsModel>>> SearchTrusts(string search)
        {
            var trustsRepositoryResult = await _trustRepostiory.SearchTrusts(search);

            if (!trustsRepositoryResult.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(trustsRepositoryResult);
            }

            var formattedOutput = trustsRepositoryResult.Result
                                                        .Select(r => _getTrustMapper.Map(r))
                                                        .ToList();

            return Ok(formattedOutput);
        }

        /// <summary>
        /// Gets the academies of a given trust. The trust is identified via its TRAMS Guid
        /// </summary>
        /// <param name="id">The Guid of the trust in TRAMS</param>
        /// <returns>A list of <see cref="GetAcademiesModel"/></returns>
        [HttpGet]
        [Route("/trusts/{id}/academies")]
        [ProducesResponseType(typeof(List<GetAcademiesModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<GetAcademiesModel>>> GetTrustAcademies(Guid id)
        {
            var trustsRepositoryResult = await _trustRepostiory.GetTrustById(id);

            if (!trustsRepositoryResult.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(trustsRepositoryResult);
            }

            if (trustsRepositoryResult.Result == null)
            {
                return NotFound($"The trust with the id: '{id}' was not found");
            }

            var academiesRepoResult = await _academiesRepository.GetAcademiesByTrustId(id);

            if (!academiesRepoResult.IsValid)
            {
                return _repositoryErrorHandler.LogAndCreateResponse(academiesRepoResult);
            }

            var formattedOutput = academiesRepoResult.Result
                                                     .Select(a => _getAcademiesMapper.Map(a))
                                                     .ToList();

            return Ok(formattedOutput);
        }
    }
}
