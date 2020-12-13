using API.Mapping;
using API.Models.D365;
using API.Models.Response;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AcademiesController : ControllerBase
    {
        private readonly AcademiesRepository _academiesRepository;
        private readonly IMapper<GetAcademiesD365Model, GetAcademiesModel> _getAcademiesMapper;

        public AcademiesController(AcademiesRepository academiesRepository,
                                   IMapper<GetAcademiesD365Model, GetAcademiesModel> getAcademiesMapper)
        {
            _academiesRepository = academiesRepository;
            _getAcademiesMapper = getAcademiesMapper;
        }

        [HttpGet]
        [Route("/academies/{id}")]
        public async Task<ActionResult<GetAcademiesD365Model>> GetAcademyById(Guid id)
        {
            var result = await _academiesRepository.GetAcademyById(id);

            if (result == null)
            {
                return NotFound($"The trust with the id: '{id}' was not found");
            }

            var formattedResult = _getAcademiesMapper.Map(result);

            return Ok(formattedResult);
        }

    }
}
