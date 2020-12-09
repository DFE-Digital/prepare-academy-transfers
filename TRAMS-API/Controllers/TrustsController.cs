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
        private readonly TrustRepository _trustRepostiory;
        private readonly IMapper<GetTrustD365Model, GetTrustsModel> _mapper;

        public TrustsController(ILogger<TrustsController> logger, 
                                         IConfiguration config,
                                         TrustRepository trustRepostiory,
                                         IMapper<GetTrustD365Model, GetTrustsModel> mapper)
        {
            _logger = logger;
            _trustRepostiory = trustRepostiory;
            _mapper = mapper;
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

            var formattedResult = _mapper.Map(result);
            
            return Ok(formattedResult);
        }

        [HttpGet]
        public async Task<ActionResult<List<GetTrustsModel>>> GetMany(string search)
        {
            var results = await _trustRepostiory.SearchTrusts(search);

            var formattedOutput = results.Select(r => _mapper.Map(r)).ToList();

            return Ok(formattedOutput);
        }
    }
}
