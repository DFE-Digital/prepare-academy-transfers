using System;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TRAMS_API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TrustsController : ControllerBase
    {
        private readonly ILogger<TrustsController> _logger;
        private readonly IConfiguration _config;
        private readonly TrustRepository _trustRepostiory;

        public TrustsController(ILogger<TrustsController> logger, 
                                         IConfiguration config,
                                         TrustRepository trustRepostiory)
        {
            _logger = logger;
            _config = config;
            _trustRepostiory = trustRepostiory;
        }

        [HttpGet]
        [Route("/trusts/{id}")]
        public async Task<ActionResult<string>> GetOne(Guid id)
        {
            var result = await _trustRepostiory.GetTrustById(id);

            if (result == null)
            {
                return NotFound($"The trust with the id: '{id}' was not found");
            }
            

            return Ok(result);
        }

        [HttpGet]
        public async Task<string> GetMany(string searchQuery)
        {
            var results = await _trustRepostiory.SearchTrusts(searchQuery);

            var nrOfTrustsWithNullUrn = results.Where(t => !string.IsNullOrEmpty(t.Urn)).ToList();

            var nrOfTrustsWithNullTrn = results.Where(t => !string.IsNullOrEmpty(t.TrustReferenceNumber)).ToList();

            var nrOfTrustsWithNullUkprn = results.Where(t => !string.IsNullOrEmpty(t.Ukprn)).ToList();

            var distinctEstablishmentTypeGroups = results.Select(t => t.EstablishmentTypeGroup).Distinct().ToList();

            var distinctEstablishmentTypes = results.Select(t => t.EstablishmentType).Distinct().ToList();

            var nrOfTrustsWithNonNullAddress = results.Where(t => !string.IsNullOrEmpty(t.Address)).ToList();

            var nrOfTrustsWithCompaniesHouseNumber = results.Where(t => !string.IsNullOrEmpty(t.CompaniesHouseNumber)).ToList();

            var nrOfTurstsWithNonNullUpin = results.Where(t => !string.IsNullOrEmpty(t.Upin)).ToList();

            return "OK";
        }
    }
}
