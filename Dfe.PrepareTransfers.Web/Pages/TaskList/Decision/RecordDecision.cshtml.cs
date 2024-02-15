using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;
using Dfe.PrepareTransfers.Data.Services.Interfaces;
using Dfe.PrepareTransfers.Pages.TaskList.Decision.Models;
using Dfe.PrepareTransfers.Services;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Pages.TaskList.Decision;

public class RecordDecision : DecisionBaseModel
{
    private readonly IAcademyTransfersAdvisoryBoardDecisionRepository _decisionRepository;
    private readonly ErrorService _errorService;

    public RecordDecision(
    IProjects repository,
    ISession session,
    ErrorService errorService,
    IAcademyTransfersAdvisoryBoardDecisionRepository decisionRepository)
       : base(repository, session)
    {
        _errorService = errorService;
        _decisionRepository = decisionRepository;
        PropagateBackLinkOverride = false;
    }

   [BindProperty]
   [Required(ErrorMessage = "Select a decision")]
   public AdvisoryBoardDecisions? AdvisoryBoardDecision { get; set; }

   public async Task<IActionResult> OnGet(int urn)
   {
        AdvisoryBoardDecision sessionDecision = GetDecisionFromSession(urn);

        if (sessionDecision.Decision == null)
        {
            RepositoryResult<AdvisoryBoardDecision> savedDecision = await _decisionRepository.Get(urn);
            SetDecisionInSession(urn, savedDecision?.Result);
            AdvisoryBoardDecision = savedDecision?.Result?.Decision;
        }
        else AdvisoryBoardDecision = sessionDecision.Decision;

        SetBackLinkModel(Links.Project.Index, urn);

        return Page();
    }

    public async Task<IActionResult> OnPost(int urn)
    {
        if (!ModelState.IsValid)
        {
            _errorService.AddErrors(new[] { "AdvisoryBoardDecision" }, ModelState);
            return await OnGet(urn);
        }

        AdvisoryBoardDecision decision = GetDecisionFromSession(urn) ?? new AdvisoryBoardDecision();
        decision.Decision = AdvisoryBoardDecision.Value;
        SetDecisionInSession(urn, decision);

        return RedirectToPage(Links.Decision.WhoDecided.PageName, LinkParameters);
    }
}
