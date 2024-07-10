using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public Dictionary<string, string> Details { get; set; } = [];

        public List<ReasonChange> ReasonOptions { get; set; }
        public bool IsDateSooner { get; set; }

        private readonly Dictionary<string, string> reasonMappings = new()
        {
            { "project-is-progressing-faster-than-expected", "Project is progressing faster than expected" },
            { "error-correction", "Correcting an error" },
            { "incoming-trust", "Incoming trust" },
            { "outgoing-trust", "Outgoing trust" },
            { "school", "School" },
            { "la-(local-authority)", "LA (local authority)" },
            { "diocese", "Diocese" },
            { "tupe-(transfer-of-undertakings-protection-of-employment-rights)", "TuPE (Transfer of Undertakings Protection of Employment rights)" },
            { "pensions", "Pensions" },
            { "union", "Union" },
            { "negative-press-coverage", "Negative press coverage" },
            { "governance", "Governance" },
            { "finance", "Finance" },
            { "viability", "Viability" },
            { "land", "Land" },
            { "buildings", "Buildings" },
            { "legal-documents", "Legal documents" },
            { "voluntary-deferral", "Voluntary deferral" },
            { "in-a-federation", "In a federation" }
        };

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

            var reasonsChanged = Reasons
                .Where(reason => reasonMappings.ContainsKey(reason))
                .Select(reason => new ReasonChange(reasonMappings[reason], Details.TryGetValue(reason, out string value) ? value : string.Empty))
                .ToList();

            await _projectsRepository.UpdateDates(projectResult, reasonsChanged, User.Identity.Name ?? string.Empty);

            return RedirectToPage("/Projects/TransferDates/Index", new { Urn });
        }


        private List<ReasonChange> GetReasonOptions(bool isDateSooner)
        {
            var soonerReasons = new List<string> { "project-is-progressing-faster-than-expected", "error-correction" };
            var laterReasons = reasonMappings.Keys.Except(soonerReasons).ToList();

            return (isDateSooner ? soonerReasons : laterReasons)
                .Select(reason => new ReasonChange(reasonMappings[reason], string.Empty))
                .ToList();
        }
    }
}
