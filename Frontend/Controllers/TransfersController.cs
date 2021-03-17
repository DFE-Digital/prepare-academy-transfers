using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Upstream.Enums;
using API.Models.Upstream.Request;
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

        public IActionResult TrustName(string query = "")
        {
            ViewData["Error.Exists"] = false;
            ViewData["Query"] = query;

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
                return RedirectToAction("TrustName", new {query});
            }

            if (result.Result.Count == 0)
            {
                TempData["ErrorMessage"] = "No results found";
                return RedirectToAction("TrustName", new {query});
            }

            var model = new TrustSearch {Trusts = result.Result};

            return View(model);
        }

        public async Task<IActionResult> OutgoingTrustDetails(Guid trustId)
        {
            var result = await _trustRepository.GetTrustById(trustId);
            var model = new OutgoingTrustDetails {Trust = result.Result};
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
            var academies = academiesRepoResult.Result;

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
            ViewData["Error.Exists"] = false;

            if (TempData.Peek("ErrorMessage") == null) return View();

            ViewData["Error.Exists"] = true;
            ViewData["Error.Message"] = TempData["ErrorMessage"];

            return View();
        }

        public async Task<IActionResult> SearchIncomingTrust(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                TempData["ErrorMessage"] = "Please enter a search term";
                return RedirectToAction("IncomingTrust");
            }

            var result = await _trustRepository.SearchTrusts(query);

            if (!result.IsValid)
            {
                TempData["ErrorMessage"] = result.Error.ErrorMessage;
                return RedirectToAction("IncomingTrust");
            }

            var model = new TrustSearch {Trusts = result.Result};

            return View(model);
        }

        public async Task<IActionResult> IncomingTrustDetails(Guid trustId)
        {
            var result = await _trustRepository.GetTrustById(trustId);
            var model = new OutgoingTrustDetails {Trust = result.Result};
            return View(model);
        }

        public IActionResult ConfirmIncomingTrust(Guid trustId)
        {
            HttpContext.Session.SetString("IncomingTrustId", trustId.ToString());

            return RedirectToAction("CheckYourAnswers");
        }

        public async Task<IActionResult> CheckYourAnswers()
        {
            var outgoingTrustId = Guid.Parse(HttpContext.Session.GetString("OutgoingTrustId"));
            var incomingTrustId = Guid.Parse(HttpContext.Session.GetString("IncomingTrustId"));
            var academyIds = Session.GetStringListFromSession(HttpContext.Session, "OutgoingAcademyIds")
                .Select(Guid.Parse);

            var outgoingTrustResponse = await _trustRepository.GetTrustById(outgoingTrustId);

            var incomingTrustResponse = await _trustRepository.GetTrustById(incomingTrustId);

            var academiesForTrust = await _academiesRepository.GetAcademiesByTrustId(outgoingTrustId);
            var selectedAcademies = academiesForTrust.Result.Where(academy => academyIds.Contains(academy.Id)).ToList();

            var model = new CheckYourAnswers
            {
                IncomingTrust = incomingTrustResponse.Result,
                OutgoingTrust = outgoingTrustResponse.Result,
                OutgoingAcademies = selectedAcademies
            };

            return View(model);
        }

        public async Task<IActionResult> SubmitProject()
        {
            var outgoingTrustId = Guid.Parse(HttpContext.Session.GetString("OutgoingTrustId"));
            var incomingTrustId = Guid.Parse(HttpContext.Session.GetString("IncomingTrustId"));
            var academyIds = Session.GetStringListFromSession(HttpContext.Session, "OutgoingAcademyIds")
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
                    new PostProjectsTrustsModel {TrustId = incomingTrustId}
                }
            };

            var result = await _projectsRepository.InsertProject(project);

            HttpContext.Session.Remove("OutgoingTrustId");
            HttpContext.Session.Remove("IncomingTrustId");
            HttpContext.Session.Remove("OutgoingAcademyIds");

            return RedirectToAction("ProjectFeatures", new {projectId = result.Result});
        }

        [Route("/transfers/project/{projectId}")]
        public ActionResult ProjectFeatures([FromRoute] Guid projectId)
        {
            var model = new ProjectFeatures()
            {
                ProjectId = projectId.ToString()
            };

            return View(model);
        }
    }
}