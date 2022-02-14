using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.Projects;
using Frontend.Services.Interfaces;
using Frontend.Validators.Transfers;
using Frontend.Views.Transfers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Session = Frontend.Helpers.Session;

namespace Frontend.Controllers
{
    [Authorize]
    public class TransfersController : Controller
    {
        private const string OutgoingAcademyIdSessionKey = "OutgoingAcademyIds";
        private const string IncomingTrustIdSessionKey = "IncomingTrustId";
        private const string OutgoingTrustIdSessionKey = "OutgoingTrustId";
        private readonly IAcademies _academiesRepository;
        private readonly IProjects _projectsRepository;
        private readonly ITrusts _trustsRepository;
        private readonly IReferenceNumberService _referenceNumberService;


        public TransfersController(IAcademies academiesRepository, IProjects projectsRepository,
            ITrusts trustsRepository, IReferenceNumberService referenceNumberService)
        {
            _academiesRepository = academiesRepository;
            _projectsRepository = projectsRepository;
            _trustsRepository = trustsRepository;
            _referenceNumberService = referenceNumberService;
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

            var validatorQuery = new OutgoingTrustNameValidator();
            var validationQueryResult = await validatorQuery.ValidateAsync(query);
            if (!validationQueryResult.IsValid)
            {
                TempData["ErrorMessage"] = validationQueryResult.Errors.First().ErrorMessage;
                return RedirectToAction("TrustName");
            }

            var result = await _trustsRepository.SearchTrusts(query);

            var validator = new OutgoingTrustSearchValidator();
            var validationResult = await validator.ValidateAsync(result.Result);
            if (!validationResult.IsValid)
            {
                TempData["ErrorMessage"] = validationResult.Errors.First().ErrorMessage;
                return RedirectToAction("TrustName", new {query});
            }

            var model = new TrustSearch {Trusts = result.Result.Where(t => t.Academies.Any()).ToList()};
            ViewData["Query"] = query;

            ViewData["Error.Exists"] = false;
            if (TempData.Peek("ErrorMessage") == null) return View(model);

            ViewData["Error.Exists"] = true;
            ViewData["Error.Message"] = TempData["ErrorMessage"];

            return View(model);
        }

        public async Task<IActionResult> OutgoingTrustDetails(string trustId, string query = "", bool change = false)
        {
            ViewData["ChangeLink"] = change;

            var validator = new OutgoingTrustConfirmValidator();
            var validationResult = await validator.ValidateAsync(trustId);

            if (!validationResult.IsValid)
            {
                TempData["ErrorMessage"] = validationResult.Errors.First().ErrorMessage;
                return RedirectToAction("TrustSearch", new {query, change});
            }

            var result = await _trustsRepository.GetByUkprn(trustId);

            var model = new OutgoingTrustDetails {Trust = result.Result};
            ViewData["Query"] = query;
            ViewData["ChangeLink"] = change;
            return View(model);
        }

        public IActionResult ConfirmOutgoingTrust(string trustId)
        {
            HttpContext.Session.SetString(OutgoingTrustIdSessionKey, trustId);
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

        public async Task<IActionResult> SubmitOutgoingTrustAcademies(string[] academyIds, bool change = false)
        {
            var validator = new OutgoingTrustAcademiesValidator();
            var validationResult = await validator.ValidateAsync(academyIds);

            if (!validationResult.IsValid)
            {
                TempData["ErrorMessage"] = validationResult.Errors.First().ErrorMessage;
                return RedirectToAction("OutgoingTrustAcademies");
            }
            
            var academyIdsString = string.Join(",", academyIds.Select(id => id.ToString()).ToList());
            HttpContext.Session.SetString(OutgoingAcademyIdSessionKey, academyIdsString);

            return RedirectToAction(change ? "CheckYourAnswers" : "IncomingTrust");
        }

        public async Task<IActionResult> IncomingTrust(string query = "", bool change = false)
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

            var validatorQuery = new IncomingTrustNameValidator();
            var validationQueryResult = await validatorQuery.ValidateAsync(query);
            if (!validationQueryResult.IsValid)
            {
                TempData["ErrorMessage"] = validationQueryResult.Errors.First().ErrorMessage;
                return RedirectToAction("IncomingTrust");
            }

            var outgoingTrustId = HttpContext.Session.GetString(OutgoingTrustIdSessionKey);
            var result = await _trustsRepository.SearchTrusts(query, outgoingTrustId);

            var validator = new IncomingTrustSearchValidator();
            var validationResult = await validator.ValidateAsync(result.Result);

            if (!validationResult.IsValid)
            {
                TempData["ErrorMessage"] = validationResult.Errors.First().ErrorMessage;
                return RedirectToAction("IncomingTrust", new {query});
            }

            var model = new TrustSearch {Trusts = result.Result};

            ViewData["Error.Exists"] = false;
            if (TempData.Peek("ErrorMessage") == null) return View(model);

            ViewData["Error.Exists"] = true;
            ViewData["Error.Message"] = TempData["ErrorMessage"];

            return View(model);
        }

        public async Task<IActionResult> ConfirmIncomingTrust(string trustId, string query = "", bool change = false)
        {
            var validator = new IncomingTrustConfirmValidator();
            var validationResult = await validator.ValidateAsync(trustId);

            if (!validationResult.IsValid)
            {
                TempData["ErrorMessage"] = validationResult.Errors.First().ErrorMessage;
                return RedirectToAction("SearchIncomingTrust", new {query, change});
            }

            HttpContext.Session.SetString(IncomingTrustIdSessionKey, trustId);

            return RedirectToAction("CheckYourAnswers");
        }

        public async Task<IActionResult> CheckYourAnswers()
        {
            var outgoingTrustId = HttpContext.Session.GetString(OutgoingTrustIdSessionKey);
            Trust incomingTrust = null;
            var academyIds = Session.GetStringListFromSession(HttpContext.Session, OutgoingAcademyIdSessionKey);

            var outgoingTrustResponse = await _trustsRepository.GetByUkprn(outgoingTrustId);

            var incomingTrustIdString = HttpContext.Session.GetString(IncomingTrustIdSessionKey);

            if (incomingTrustIdString != null)
            {
                var incomingTrustResponse = await _trustsRepository.GetByUkprn(incomingTrustIdString);

                incomingTrust = incomingTrustResponse.Result;
            }

            var selectedAcademies = outgoingTrustResponse.Result.Academies
                .Where(academy => academyIds.Contains(academy.Ukprn)).ToList();

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
            var outgoingTrustId = HttpContext.Session.GetString(OutgoingTrustIdSessionKey);
            var incomingTrustId = HttpContext.Session.GetString(IncomingTrustIdSessionKey);
            var academyIds = Session.GetStringListFromSession(HttpContext.Session, OutgoingAcademyIdSessionKey);


            var project = new Project
            {
                OutgoingTrustUkprn = outgoingTrustId,
                TransferringAcademies = academyIds.Select(id => new TransferringAcademies
                {
                    OutgoingAcademyUkprn = id.ToString(),
                    IncomingTrustUkprn = incomingTrustId
                }).ToList()
            };

            var createResponse = await _projectsRepository.Create(project);

            createResponse.Result.Reference = _referenceNumberService.GenerateReferenceNumber(createResponse.Result);
            await _projectsRepository.Update(createResponse.Result);
            
            HttpContext.Session.Remove(OutgoingTrustIdSessionKey);
            HttpContext.Session.Remove(IncomingTrustIdSessionKey);
            HttpContext.Session.Remove(OutgoingAcademyIdSessionKey);

            return RedirectToPage($"/Projects/{nameof(Pages.Projects.Index)}", new {urn = createResponse.Result.Urn});
        }
    }
}