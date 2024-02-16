using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;
using Dfe.PrepareTransfers.Data.Services.Interfaces;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dfe.PrepareTransfers.Web.Pages.Decision
{
    public class RecordADecision : CommonPageModel
    {
        private readonly ILogger<RecordADecision> _logger;
        private readonly IAcademyTransfersAdvisoryBoardDecisionRepository _decisionRepository;
        private readonly IProjects _projectsRepository;

        public Project Project { get; set; }
        public AdvisoryBoardDecision Decision { get; set; }

        public RecordADecision(IAcademyTransfersAdvisoryBoardDecisionRepository decisionRepository, IProjects projectsRepository, ILogger<RecordADecision> logger)
        {
            _decisionRepository = decisionRepository;
            _projectsRepository = projectsRepository;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Project = (await _projectsRepository.GetByUrn(Urn)).Result;
            Decision = (await _decisionRepository.Get(int.Parse(Urn))).Result;

            return Page();
        }

    }
}
