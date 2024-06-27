using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.TransferDates
{
    public class Reason : CommonPageModel
    {
        private readonly IProjects _projectsRepository;
        [BindProperty]
        public string TargetDate { get; set; }
        [BindProperty]
        public List<string> Reasons { get; set; }
        [BindProperty]
        public string DetailsFasterProgress { get; set; }
        [BindProperty]
        public string DetailsErrorCorrection { get; set; }
        [BindProperty]
        public string DetailsIncomingTrust { get; set; }
        [BindProperty]
        public string DetailsOutgoingTrust { get; set; }
        [BindProperty]
        public string DetailsSchool { get; set; }
        [BindProperty]
        public string DetailsLocalAuthority { get; set; }
        [BindProperty]
        public string DetailsDiocese { get; set; }
        [BindProperty]
        public string DetailsTupe { get; set; }
        [BindProperty]
        public string DetailsPensions { get; set; }
        [BindProperty]
        public string DetailsUnion { get; set; }
        [BindProperty]
        public string DetailsNegativePressCoverage { get; set; }
        [BindProperty]
        public string DetailsGovernance { get; set; }
        [BindProperty]
        public string DetailsFinance { get; set; }
        [BindProperty]
        public string DetailsViability { get; set; }
        [BindProperty]
        public string DetailsLand { get; set; }
        [BindProperty]
        public string DetailsBuildings { get; set; }
        [BindProperty]
        public string DetailsLegalDocuments { get; set; }
        [BindProperty]
        public string DetailsVoluntaryDeferral { get; set; }
        [BindProperty]
        public string DetailsFederation { get; set; }

        public List<ReasonChange> ReasonOptions { get; set; }
        public bool IsDateSooner { get; set; }

        public Reason(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> OnGetAsync(string urn, string targetDate)
        {
            Urn = urn;
            TargetDate = targetDate;

            var project = await _projectsRepository.GetByUrn(Urn);
            var projectResult = project.Result;

            DateTime newDate = DateTime.ParseExact(TargetDate, "dd/MM/yyyy", null);
            DateTime existingDate = DateTime.ParseExact(projectResult.Dates.Target, "dd/MM/yyyy", null);
            IsDateSooner = newDate < existingDate;
            IncomingTrustName = projectResult.IncomingTrustName;
            ReasonOptions = GetReasonOptions(IsDateSooner);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projectsRepository.GetByUrn(Urn);
            var projectResult = project.Result;

            projectResult.Dates.Target = TargetDate;
            projectResult.Dates.HasTargetDateForTransfer = true;

            var reasonsChanged = new List<ReasonChange>();

            if (Reasons.Contains("faster-progress"))
            {
                reasonsChanged.Add(new ReasonChange("Project is progressing faster than expected", DetailsFasterProgress));
            }

            if (Reasons.Contains("error-correction"))
            {
                reasonsChanged.Add(new ReasonChange("Correcting an error", DetailsErrorCorrection));
            }

            // Additional reasons if the new date is later
            if (Reasons.Contains("incoming-trust"))
            {
                reasonsChanged.Add(new ReasonChange("Incoming trust", DetailsIncomingTrust));
            }

            if (Reasons.Contains("outgoing-trust"))
            {
                reasonsChanged.Add(new ReasonChange("Outgoing trust", DetailsOutgoingTrust));
            }

            if (Reasons.Contains("school"))
            {
                reasonsChanged.Add(new ReasonChange("School", DetailsSchool));
            }

            if (Reasons.Contains("local-authority"))
            {
                reasonsChanged.Add(new ReasonChange("LA (local authority)", DetailsLocalAuthority));
            }

            if (Reasons.Contains("diocese"))
            {
                reasonsChanged.Add(new ReasonChange("Diocese", DetailsDiocese));
            }

            if (Reasons.Contains("tupe"))
            {
                reasonsChanged.Add(new ReasonChange("TuPE (Transfer of Undertakings Protection of Employment rights)", DetailsTupe));
            }

            if (Reasons.Contains("pensions"))
            {
                reasonsChanged.Add(new ReasonChange("Pensions", DetailsPensions));
            }

            if (Reasons.Contains("union"))
            {
                reasonsChanged.Add(new ReasonChange("Union", DetailsUnion));
            }

            if (Reasons.Contains("negative-press-coverage"))
            {
                reasonsChanged.Add(new ReasonChange("Negative press coverage", DetailsNegativePressCoverage));
            }

            if (Reasons.Contains("governance"))
            {
                reasonsChanged.Add(new ReasonChange("Governance", DetailsGovernance));
            }

            if (Reasons.Contains("finance"))
            {
                reasonsChanged.Add(new ReasonChange("Finance", DetailsFinance));
            }

            if (Reasons.Contains("viability"))
            {
                reasonsChanged.Add(new ReasonChange("Viability", DetailsViability));
            }

            if (Reasons.Contains("land"))
            {
                reasonsChanged.Add(new ReasonChange("Land", DetailsLand));
            }

            if (Reasons.Contains("buildings"))
            {
                reasonsChanged.Add(new ReasonChange("Buildings", DetailsBuildings));
            }

            if (Reasons.Contains("legal-documents"))
            {
                reasonsChanged.Add(new ReasonChange("Legal documents", DetailsLegalDocuments));
            }

            if (Reasons.Contains("voluntary-deferral"))
            {
                reasonsChanged.Add(new ReasonChange("Voluntary deferral", DetailsVoluntaryDeferral));
            }

            if (Reasons.Contains("federation"))
            {
                reasonsChanged.Add(new ReasonChange("In a federation", DetailsFederation));
            }

            await _projectsRepository.UpdateDates(projectResult, reasonsChanged, User.FindFirstValue("preferred_username") ?? string.Empty);

            return RedirectToPage("/Projects/TransferDates/Index", new { Urn });
        }

        private List<ReasonChange> GetReasonOptions(bool isDateSooner)
        {
            if (isDateSooner)
            {
                return new List<ReasonChange>
                {
                    new ReasonChange("Project is progressing faster than expected", string.Empty),
                    new ReasonChange("Correcting an error", string.Empty)
                };
            }
            else
            {
                return new List<ReasonChange>
                {
                    new ReasonChange("Incoming trust", string.Empty),
                    new ReasonChange("Outgoing trust", string.Empty),
                    new ReasonChange("School", string.Empty),
                    new ReasonChange("LA (local authority)", string.Empty),
                    new ReasonChange("Diocese", string.Empty),
                    new ReasonChange("TuPE (Transfer of Undertakings Protection of Employment rights)", string.Empty),
                    new ReasonChange("Pensions", string.Empty),
                    new ReasonChange("Union", string.Empty),
                    new ReasonChange("Negative press coverage", string.Empty),
                    new ReasonChange("Governance", string.Empty),
                    new ReasonChange("Finance", string.Empty),
                    new ReasonChange("Viability", string.Empty),
                    new ReasonChange("Land", string.Empty),
                    new ReasonChange("Buildings", string.Empty),
                    new ReasonChange("Legal documents", string.Empty),
                    new ReasonChange("Voluntary deferral", string.Empty),
                    new ReasonChange("In a federation", string.Empty)
                };
            }
        }
    }
}
