using System;
using System.Linq;
using System.Threading.Tasks;
using API.Mapping;
using API.Models.Downstream.D365;
using API.Models.Upstream.Response;
using API.Repositories;
using Frontend.Views.Transfers;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class TransfersController : Controller
    {
        private readonly ITrustsRepository _trustRepository;
        private readonly IMapper<GetTrustsD365Model, GetTrustsModel> _getTrustMapper;

        public TransfersController(ITrustsRepository trustRepository,
            IMapper<GetTrustsD365Model, GetTrustsModel> getTrustMapper)
        {
            _trustRepository = trustRepository;
            _getTrustMapper = getTrustMapper;
        }

        public IActionResult TrustName()
        {
            ViewData["Error.Exists"] = false;

            if (TempData.Peek("ErrorMessage") == null) return View();

            ViewData["Error.Exists"] = true;
            ViewData["Error.Message"] = TempData["ErrorMessage"];
            return View();
        }

        public async Task<IActionResult> TrustSearch(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                TempData["ErrorMessage"] = "Please enter a search term";
                return RedirectToAction("TrustName");
            }

            var result = await _trustRepository.SearchTrusts(query);

            if (!result.IsValid)
            {
                TempData["ErrorMessage"] = result.Error.ErrorMessage;
                return RedirectToAction("TrustName");
            }

            var mappedResults = result.Result.Select(r => _getTrustMapper.Map(r)).ToList();
            var model = new TrustSearch {Trusts = mappedResults};

            return View(model);
        }

        public async Task<IActionResult> OutgoingTrustDetails(Guid trustId)
        {
            var result = await _trustRepository.GetTrustById(trustId);
            var mappedResult = _getTrustMapper.Map(result.Result);
            var model = new OutgoingTrustDetails {Trust = mappedResult};
            return View(model);
        }
    }
}