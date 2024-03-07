using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.Web.Dfe.PrepareTransfers.Helpers;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.Transfers
{
    public class CheckYourAnswersModel : TransfersPageModel
    {
        private readonly ITrusts _trustsRepository;
        private readonly IProjects _projectsRepository;
        private readonly IReferenceNumberService _referenceNumberService;
        private readonly IAcademies _academyRepository;

        public CheckYourAnswersModel(ITrusts trustsRepository, IProjects projectsRepository,
            IReferenceNumberService referenceNumberService, IAcademies academyRepository)
        {
            _trustsRepository = trustsRepository;
            _projectsRepository = projectsRepository;
            _referenceNumberService = referenceNumberService;
            _academyRepository = academyRepository;
        }

        [BindProperty]
        public Trust OutgoingTrust { get; set; }
        [BindProperty]
        public List<Academy> OutgoingAcademies { get; set; }
        [BindProperty]
        public Trust IncomingTrust { get; set; }
        [BindProperty]
        public string ProposedTrustName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var outgoingTrustId = HttpContext.Session.GetString(OutgoingTrustIdSessionKey);
            IncomingTrust = null;
            var academyIds = Session.GetStringListFromSession(HttpContext.Session, OutgoingAcademyIdSessionKey);

            var outgoingTrustResponse = await _trustsRepository.GetByUkprn(outgoingTrustId);
            OutgoingTrust = outgoingTrustResponse;

            var incomingTrustIdString = HttpContext.Session.GetString(IncomingTrustIdSessionKey);
            ProposedTrustName = HttpContext.Session.GetString(ProposedTrustNameSessionKey);

            if (incomingTrustIdString != null)
            {
                var incomingTrustResponse = await _trustsRepository.GetByUkprn(incomingTrustIdString);

                IncomingTrust = incomingTrustResponse;
            }
            
            IsFormAMAT = !string.IsNullOrEmpty(ProposedTrustName);

            OutgoingAcademies = await _academyRepository.GetAcademiesByTrustUkprn(outgoingTrustResponse.Ukprn);
            OutgoingAcademies = OutgoingAcademies.Where(academy => academyIds.Contains(academy.Ukprn)).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var outgoingTrustId = HttpContext.Session.GetString(OutgoingTrustIdSessionKey);
            var incomingTrustId = HttpContext.Session.GetString(IncomingTrustIdSessionKey);
            var proposedTrustName = HttpContext.Session.GetString(ProposedTrustNameSessionKey);
            var academyIds = Session.GetStringListFromSession(HttpContext.Session, OutgoingAcademyIdSessionKey);

            //Redirect any duplicate requests (Session has been cleared)
            if (string.IsNullOrWhiteSpace(outgoingTrustId) || (string.IsNullOrWhiteSpace(incomingTrustId) && string.IsNullOrWhiteSpace(proposedTrustName) )||
                !academyIds.Any())
            {
                var urnCreated = HttpContext.Session.GetString(ProjectCreatedSessionKey);
                return !string.IsNullOrWhiteSpace(urnCreated)
                    ? RedirectToPage($"/Projects/{nameof(Projects.Index)}", new
                    {
                        urn = urnCreated
                    })
                    : throw new Exception("Cannot create project");
            }

            var project = new Project
            {
                OutgoingTrustUkprn = outgoingTrustId,
                OutgoingTrustName = OutgoingTrust.Name,
                TransferringAcademies = academyIds.Select(id => new TransferringAcademies
                {
                    OutgoingAcademyUkprn = id.ToString(),
                    IncomingTrustUkprn = IsFormAMAT ? null:  incomingTrustId,
                    IncomingTrustName = IsFormAMAT ? proposedTrustName : IncomingTrust.Name,
                }).ToList()
            };

            var createResponse = await _projectsRepository.Create(project);
            HttpContext.Session.SetString(ProjectCreatedSessionKey, createResponse.Result.Urn);
            HttpContext.Session.Remove(OutgoingTrustIdSessionKey);
            HttpContext.Session.Remove(IncomingTrustIdSessionKey);
            HttpContext.Session.Remove(OutgoingAcademyIdSessionKey);
            HttpContext.Session.Remove(ProposedTrustNameSessionKey);

            return RedirectToPage($"/Projects/{nameof(Projects.Index)}", new
            {
                urn = createResponse.Result.Urn
            });
        }
    }
}
