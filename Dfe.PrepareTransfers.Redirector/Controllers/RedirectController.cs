using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Dfe.PrepareTransfers.Redirector.Models;

namespace Dfe.PrepareTransfers.Redirector.Controllers
{
    public class RedirectController : Controller
    {
        private readonly string _destinationHost;
        private readonly int _redirectDelay;

        public RedirectController(IConfiguration configuration)
        {
            _destinationHost = new UriBuilder(configuration[Configuration.DestinationHost]).Host;
            _redirectDelay = int.Parse(configuration[Configuration.RedirectDelay], NumberStyles.Integer);
        }
        public IActionResult Index(string path)
        {
            return View(new Redirection(_destinationHost, path, Request.QueryString.Value, _redirectDelay));
        }
    }
}
