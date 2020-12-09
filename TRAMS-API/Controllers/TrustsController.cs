using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Mapping;
using API.Models.D365;
using API.Models.Response;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TRAMS_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
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

        [HttpGet]
        [Route("/trusts/{id}")]
        public async Task<ActionResult<GetTrustsModel>> GetOne(Guid id)
        {
            var result = await _trustRepostiory.GetTrustById(id);

            if (result == null)
            {
                return NotFound($"The trust with the id: '{id}' was not found");
            }

            var formattedResult = _getTrustMapper.Map(result);
            
            return Ok(formattedResult);
        }

        [HttpGet]
        public async Task<ActionResult<List<GetTrustsModel>>> GetMany(string search)
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
                return NotFound($"Could not find a trust with the id: '{id}'");
            }

            var academies = await _academiesRepository.GetAcademiesByTrustId(id);

            var formattedOutput = academies.Select(a => _getAcademiesMapper.Map(a)).ToList();

            return Ok(formattedOutput);
        }
    }
}
