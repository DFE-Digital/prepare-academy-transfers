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
using Session = Frontend.Helpers.Session;

namespace Frontend.Controllers
{
    [Authorize]
    public class TransfersController : Controller
    {
        private const string OutgoingAcademyIdSessionKey = "OutgoingAcademyIds";
        private const string IncomingTrustIdSessionKey = "IncomingTrustId";
        private const string OutgoingTrustIdSessionKey = "OutgoingTrustId";
        private const string ProjectCreatedSessionKey = "ProjectCreated";
        private readonly IProjects _projectsRepository;
        private readonly ITrusts _trustsRepository;
        private readonly IReferenceNumberService _referenceNumberService;

        public TransfersController(IProjects projectsRepository,
            ITrusts trustsRepository, IReferenceNumberService referenceNumberService)
        {
            _projectsRepository = projectsRepository;
            _trustsRepository = trustsRepository;
            _referenceNumberService = referenceNumberService;
        }

        public IActionResult ConfirmOutgoingTrust(string trustId)
        {
            HttpContext.Session.SetString(OutgoingTrustIdSessionKey, trustId);
            HttpContext.Session.Remove(IncomingTrustIdSessionKey);
            HttpContext.Session.Remove(OutgoingAcademyIdSessionKey);

            return RedirectToPage("/Transfers/OutgoingTrustAcademies");
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

        [HttpPost]
        public async Task<IActionResult> SubmitProject()
        {
            var outgoingTrustId = HttpContext.Session.GetString(OutgoingTrustIdSessionKey);
            var incomingTrustId = HttpContext.Session.GetString(IncomingTrustIdSessionKey);
            var academyIds = Session.GetStringListFromSession(HttpContext.Session, OutgoingAcademyIdSessionKey);
            
            //Redirect any duplicate requests (Session has been cleared)
            if (string.IsNullOrWhiteSpace(outgoingTrustId) || string.IsNullOrWhiteSpace(incomingTrustId) ||
                !academyIds.Any())
            {
                var urnCreated = HttpContext.Session.GetString(ProjectCreatedSessionKey);
                return !string.IsNullOrWhiteSpace(urnCreated)
                    ? RedirectToPage($"/Projects/{nameof(Pages.Projects.Index)}", new
                    {
                        urn = urnCreated
                    })
                    : throw new Exception("Cannot create project");
            }

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
            HttpContext.Session.SetString(ProjectCreatedSessionKey, createResponse.Result.Urn);

            createResponse.Result.Reference =
                _referenceNumberService.GenerateReferenceNumber(createResponse.Result);
            await _projectsRepository.Update(createResponse.Result);

            HttpContext.Session.Remove(OutgoingTrustIdSessionKey);
            HttpContext.Session.Remove(IncomingTrustIdSessionKey);
            HttpContext.Session.Remove(OutgoingAcademyIdSessionKey);

            return RedirectToPage($"/Projects/{nameof(Pages.Projects.Index)}", new
            {
                urn = createResponse.Result.Urn
            });
        }
    }
}