using System;
using System.Linq;
using System.Threading.Tasks;
using API.Repositories;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Helpers;
using Frontend.Views.Transfers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    [Authorize]
    public class TransfersController : Controller
    {
        private const string OutgoingAcademyIdSessionKey = "OutgoingAcademyIds";
        private const string IncomingTrustIdSessionKey = "IncomingTrustId";
        private const string OutgoingTrustIdSessionKey = "OutgoingTrustId";
        private readonly IAcademiesRepository _dynamicsAcademiesRepository;
        private readonly IProjects _projectsRepository;
        private readonly ITrusts _trustsRepository;

        public TransfersController(IAcademiesRepository dynamicsAcademiesRepository,
            IProjects projectsRepository, ITrusts trustsRepository)
        {
            _dynamicsAcademiesRepository = dynamicsAcademiesRepository;
            _projectsRepository = projectsRepository;
            _trustsRepository = trustsRepository;
        }

        public IActionResult TrustName(string query = "", bool change = false)
        {
            ViewData["Error.Exists"] = false;
            ViewData["Query"] = query;
            ViewData["ChangeLink"] = change;

            if (TempData.Peek("ErrorMessage") == null) return View();

            ViewData["Error.Exists"] = true;
            ViewData["Error.Message"] = TempData["ErrorMessage"];
            return View();
        }

        public async Task<IActionResult> TrustSearch(string query, bool change = false)
        {
            ViewData["ChangeLink"] = change;

            if (string.IsNullOrEmpty(query))
            {
                TempData["ErrorMessage"] = "Please enter a search term";
                return RedirectToAction("TrustName");
            }

            var result = await _trustsRepository.SearchTrusts(query);

            if (!result.IsValid)
            {
                TempData["ErrorMessage"] = result.Error.ErrorMessage;
                return RedirectToAction("TrustName", new {query});
            }

            if (result.Result.Count == 0)
            {
                TempData["ErrorMessage"] = "No results found";
                return RedirectToAction("TrustName", new {query});
            }

            var model = new TrustSearch {Trusts = result.Result};
            ViewData["Query"] = query;

            return View(model);
        }

        public async Task<IActionResult> OutgoingTrustDetails(string trustId, string query = "", bool change = false)
        {
            var result = await _trustsRepository.GetByUkprn(trustId);
            var model = new OutgoingTrustDetails {Trust = result.Result};
            ViewData["Query"] = query;
            ViewData["ChangeLink"] = change;
            return View(model);
        }

        public IActionResult ConfirmOutgoingTrust(Guid trustId)
        {
            HttpContext.Session.SetString(OutgoingTrustIdSessionKey, trustId.ToString());
            HttpContext.Session.Remove(IncomingTrustIdSessionKey);
            HttpContext.Session.Remove(OutgoingAcademyIdSessionKey);

            return RedirectToAction("OutgoingTrustAcademies");
        }

        public async Task<IActionResult> OutgoingTrustAcademies(bool change = false)
        {
            var sessionAcademyIds = HttpContext.Session.GetString(OutgoingAcademyIdSessionKey);
            var outgoingTrustId = HttpContext.Session.GetString(OutgoingTrustIdSessionKey);
            ViewData["OutgoingTrustId"] = outgoingTrustId;
            ViewData["ChangeLink"] = change;
            ViewData["OutgoingAcademyId"] = null;

            if (!string.IsNullOrEmpty(sessionAcademyIds))
            {
                var academyId = sessionAcademyIds.Split(",")[0];
                ViewData["OutgoingAcademyId"] = academyId;
            }

            var trustRepoResult = await _trustsRepository.GetByUkprn(outgoingTrustId);
            var model = new OutgoingTrustAcademies {Academies = trustRepoResult.Result.Academies};

            ViewData["Error.Exists"] = false;
            if (TempData.Peek("ErrorMessage") == null) return View(model);

            ViewData["Error.Exists"] = true;
            ViewData["Error.Message"] = TempData["ErrorMessage"];

            return View(model);
        }

        public IActionResult SubmitOutgoingTrustAcademies(Guid? academyId, bool change = false)
        {
            if (!academyId.HasValue)
            {
                TempData["ErrorMessage"] = "Please select an academy";
                return RedirectToAction("OutgoingTrustAcademies");
            }

            var academyIds = new[] {academyId};
            var academyIdsString = string.Join(",", academyIds.Select(id => id.ToString()).ToList());
            HttpContext.Session.SetString(OutgoingAcademyIdSessionKey, academyIdsString);

            return RedirectToAction(change ? "CheckYourAnswers" : "IncomingTrust");
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

        public IActionResult IncomingTrust(string query = "", bool change = false)
        {
            ViewData["Error.Exists"] = false;
            ViewData["Query"] = query;
            ViewData["ChangeLink"] = change;

            if (TempData.Peek("ErrorMessage") == null) return View();

            ViewData["Error.Exists"] = true;
            ViewData["Error.Message"] = TempData["ErrorMessage"];

            return View();
        }

        public async Task<IActionResult> SearchIncomingTrust(string query, bool change = false)
        {
            ViewData["Query"] = query;
            ViewData["ChangeLink"] = change;
            if (string.IsNullOrEmpty(query))
            {
                TempData["ErrorMessage"] = "Please enter a search term";
                return RedirectToAction("IncomingTrust");
            }

            var result = await _trustsRepository.SearchTrusts(query);

            if (!result.IsValid)
            {
                TempData["ErrorMessage"] = result.Error.ErrorMessage;
                return RedirectToAction("IncomingTrust", new {query});
            }

            if (result.Result.Count == 0)
            {
                TempData["ErrorMessage"] = "No search results";
                return RedirectToAction("IncomingTrust", new {query});
            }

            var model = new TrustSearch {Trusts = result.Result};

            return View(model);
        }

        public async Task<IActionResult> IncomingTrustDetails(string trustId, string query = "", bool change = false)
        {
            var result = await _trustsRepository.GetByUkprn(trustId);
            var model = new OutgoingTrustDetails {Trust = result.Result};
            ViewData["Query"] = query;
            ViewData["ChangeLink"] = change;
            return View(model);
        }

        public IActionResult ConfirmIncomingTrust(Guid trustId)
        {
            HttpContext.Session.SetString(IncomingTrustIdSessionKey, trustId.ToString());

            return RedirectToAction("CheckYourAnswers");
        }

        public async Task<IActionResult> CheckYourAnswers()
        {
            var outgoingTrustId = HttpContext.Session.GetString(OutgoingTrustIdSessionKey);
            Trust incomingTrust = null;
            var academyIds = Session.GetStringListFromSession(HttpContext.Session, OutgoingAcademyIdSessionKey)
                .Select(Guid.Parse);

            var outgoingTrustResponse = await _trustsRepository.GetByUkprn(outgoingTrustId);

            var incomingTrustIdString = HttpContext.Session.GetString(IncomingTrustIdSessionKey);

            if (incomingTrustIdString != null)
            {
                var incomingTrustResponse = await _trustsRepository.GetByUkprn(incomingTrustIdString);
                incomingTrust = incomingTrustResponse.Result;
            }

            var academiesForTrust =
                await _dynamicsAcademiesRepository.GetAcademiesByTrustId(Guid.Parse(outgoingTrustId));
            var selectedAcademies = academiesForTrust.Result.Where(academy => academyIds.Contains(academy.Id)).ToList();

            var model = new CheckYourAnswers
            {
                IncomingTrust = incomingTrust,
                OutgoingTrust = outgoingTrustResponse.Result,
                OutgoingAcademies = selectedAcademies
            };

            return View(model);
        }

        public async Task<IActionResult> SubmitProject()
        {
            var outgoingTrustId = Guid.Parse(HttpContext.Session.GetString(OutgoingTrustIdSessionKey));
            var incomingTrustId = Guid.Parse(HttpContext.Session.GetString(IncomingTrustIdSessionKey));
            var academyIds = Session.GetStringListFromSession(HttpContext.Session, OutgoingAcademyIdSessionKey)
                .Select(Guid.Parse).ToList();

            var project = new Project
            {
                OutgoingTrustUkprn = outgoingTrustId.ToString(),
                TransferringAcademies = academyIds.Select(id => new TransferringAcademies()
                {
                    OutgoingAcademyUkprn = id.ToString(),
                    IncomingTrustUkprn = incomingTrustId.ToString()
                }).ToList()
            };

            var result = await _projectsRepository.Create(project);

            HttpContext.Session.Remove(OutgoingTrustIdSessionKey);
            HttpContext.Session.Remove(IncomingTrustIdSessionKey);
            HttpContext.Session.Remove(OutgoingAcademyIdSessionKey);

            return RedirectToAction("Index", "Project", new {id = result.Result.Urn});
        }
    }
}