using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Downstream.D365;
using API.Models.Upstream.Enums;
using API.Models.Upstream.Request;
using API.Models.Upstream.Response;
using API.Repositories;
using API.Repositories.Interfaces;
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
        private readonly ITrustsRepository _trustRepository;
        private readonly IAcademiesRepository _academiesRepository;
        private readonly IProjectsRepository _projectsRepository;

        public TransfersController(ITrustsRepository trustRepository, IAcademiesRepository academiesRepository,
            IProjectsRepository projectsRepository)
        {
            _trustRepository = trustRepository;
            _academiesRepository = academiesRepository;
            _projectsRepository = projectsRepository;
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

            var result = await _trustRepository.SearchTrusts(query);

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

        public async Task<IActionResult> OutgoingTrustDetails(Guid trustId, string query = "", bool change = false)
        {
            var result = await _trustRepository.GetTrustById(trustId);
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
            var sessionGuid = HttpContext.Session.GetString(OutgoingTrustIdSessionKey);
            var sessionAcademyIds = HttpContext.Session.GetString(OutgoingAcademyIdSessionKey);
            var outgoingTrustId = Guid.Parse(sessionGuid);
            ViewData["OutgoingTrustId"] = outgoingTrustId.ToString();
            ViewData["ChangeLink"] = change;
            ViewData["OutgoingAcademyId"] = null;

            if (!string.IsNullOrEmpty(sessionAcademyIds))
            {
                var academyId = sessionAcademyIds.Split(",")[0];
                ViewData["OutgoingAcademyId"] = academyId;
            }

            var academiesRepoResult = await _academiesRepository.GetAcademiesByTrustId(outgoingTrustId);
            var academies = academiesRepoResult.Result;
            var model = new OutgoingTrustAcademies {Academies = academies};

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

            var result = await _trustRepository.SearchTrusts(query);

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

        public async Task<IActionResult> IncomingTrustDetails(Guid trustId, string query = "", bool change = false)
        {
            var result = await _trustRepository.GetTrustById(trustId);
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
            var outgoingTrustId = Guid.Parse(HttpContext.Session.GetString(OutgoingTrustIdSessionKey));
            GetTrustsModel incomingTrust = null;
            var academyIds = Session.GetStringListFromSession(HttpContext.Session, OutgoingAcademyIdSessionKey)
                .Select(Guid.Parse);

            var outgoingTrustResponse = await _trustRepository.GetTrustById(outgoingTrustId);

            var incomingTrustIdString = HttpContext.Session.GetString(IncomingTrustIdSessionKey);

            if (incomingTrustIdString != null)
            {
                var incomingTrustId = Guid.Parse(incomingTrustIdString);
                var incomingTrustResponse = await _trustRepository.GetTrustById(incomingTrustId);
                incomingTrust = incomingTrustResponse.Result;
            }

            var academiesForTrust = await _academiesRepository.GetAcademiesByTrustId(outgoingTrustId);
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
                .Select(Guid.Parse);


            var academies = academyIds.Select(id => new PostProjectsAcademiesModel
            {
                AcademyId = id,
                Trusts = new List<PostProjectsAcademiesTrustsModel>
                {
                    new PostProjectsAcademiesTrustsModel {TrustId = outgoingTrustId}
                }
            }).ToList();

            var project = new PostProjectsRequestModel
            {
                ProjectInitiatorFullName = "academy",
                ProjectInitiatorUid = Guid.NewGuid().ToString(),
                ProjectAcademies = academies,
                ProjectStatus = ProjectStatusEnum.InProgress,
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel {TrustId = incomingTrustId},
                    new PostProjectsTrustsModel {TrustId = outgoingTrustId}
                }
            };

            var result = await _projectsRepository.InsertProject(project);

            HttpContext.Session.Remove(OutgoingTrustIdSessionKey);
            HttpContext.Session.Remove(IncomingTrustIdSessionKey);
            HttpContext.Session.Remove(OutgoingAcademyIdSessionKey);

            return RedirectToAction("Index", "Project", new {id = result.Result});
        }
    }
}