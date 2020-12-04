using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using API.HttpHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace TRAMS_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _config;
        private readonly AuthenticatedHttpClient _client;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, 
                                         IConfiguration config,
                                         AuthenticatedHttpClient httpClient)
        {
            _logger = logger;
            _config = config;
            _client = httpClient;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            await _client.AuthenticateAsync();
            var result = await _client.GetAsync($"accounts?$select=name,sip_companieshousenumber,sip_compositeaddress,_sip_establishmenttypeid_value,_sip_establismenttypegroupid_value,sip_trustreferencenumber,sip_ukprn,sip_upin,sip_urn");

            var resultContent = await result.Content.ReadAsStringAsync();

            return $"Request Status Code: {result.StatusCode}. First 200 chars of response are: {resultContent.Substring(0, 200)}";
        }
    }
}
