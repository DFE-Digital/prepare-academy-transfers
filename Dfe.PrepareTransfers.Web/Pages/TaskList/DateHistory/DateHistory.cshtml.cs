using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.TaskList.DateHistory
{
    public class DateHistory : CommonPageModel
    {
        private readonly ILogger<DateHistory> _logger;
        private readonly IProjects _projectsRepository;

        public Project Project { get; set; }
        public AdvisoryBoardDecision Decision { get; set; }
        public IEnumerable<OpeningDateHistoryDto> OpeningDateHistory { get; set; }

        public DateHistory(IProjects projectsRepository, ILogger<DateHistory> logger)
        {
            _projectsRepository = projectsRepository;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Project = (await _projectsRepository.GetByUrn(Urn)).Result;
            OpeningDateHistory = await _projectsRepository.GetOpeningDateHistory(Convert.ToInt32(Urn));

            return Page();
        }
    }
}
