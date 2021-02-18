using System;
using System.Linq;
using System.Threading.Tasks;
using API.Mapping;
using API.Models.Downstream.D365;
using API.Models.Upstream.Response;
using API.Repositories;
using Frontend.Views.Transfers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class TransfersController : Controller
    {
        private readonly ITrustsRepository _trustRepository;
        private readonly IAcademiesRepository _academiesRepository;
        private readonly IMapper<GetTrustsD365Model, GetTrustsModel> _getTrustMapper;
        private readonly IMapper<GetAcademiesD365Model, GetAcademiesModel> _getAcademiesMapper;

        public TransfersController(ITrustsRepository trustRepository, IAcademiesRepository academiesRepository,
            IMapper<GetTrustsD365Model, GetTrustsModel> getTrustMapper,
            IMapper<GetAcademiesD365Model, GetAcademiesModel> getAcademiesMapper)
        {
            _trustRepository = trustRepository;
            _academiesRepository = academiesRepository;
            _getTrustMapper = getTrustMapper;
            _getAcademiesMapper = getAcademiesMapper;
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

        public IActionResult ConfirmOutgoingTrust(Guid trustId)
        {
            HttpContext.Session.SetString("OutgoingTrustId", trustId.ToString());

            return RedirectToAction("OutgoingTrustAcademies");
        }

        public async Task<IActionResult> OutgoingTrustAcademies()
        {
            var sessionGuid = HttpContext.Session.GetString("OutgoingTrustId");
            var outgoingTrustId = Guid.Parse(sessionGuid);
            var academiesRepoResult = await _academiesRepository.GetAcademiesByTrustId(outgoingTrustId);
            var academies = academiesRepoResult.Result
                ?.Select(a => _getAcademiesMapper.Map(a))
                .ToList();

            var model = new OutgoingTrustAcademies {Academies = academies};
            return View(model);
        }

        public IActionResult SubmitOutgoingTrustAcademies(Guid[] academyIds)
        {
            var academyIdsString = string.Join(",", academyIds.Select(id => id.ToString()).ToList());
            HttpContext.Session.SetString("OutgoingAcademyIds", academyIdsString);

            return RedirectToAction("IncomingTrustIdentified");
        }

        public IActionResult IncomingTrustIdentified()
        {
            return View();
        }

        public IActionResult SubmitIncomingTrustIdentified(string incomingTrustIdentified)
        {
            var nextAction = incomingTrustIdentified == "yes" ? "IncomingTrust" : "CheckYourAnswers";
            return RedirectToAction(nextAction);
        }

        public IActionResult IncomingTrust()
        {
            return View();
        }

        public async Task<IActionResult> SearchIncomingTrust()
        {
            return null;
        }

        public IActionResult CheckYourAnswers()
        {
            return View();
        }
    }
}