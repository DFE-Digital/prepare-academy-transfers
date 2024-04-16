using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Data.Models.AdvisoryBoardDecision;
using Dfe.PrepareTransfers.Extensions;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Pages.TaskList.Decision.Models;

public abstract class DecisionBaseModel : PageModel
{
    public const string DECISION_SESSION_KEY = "Decision";
    protected readonly IProjects _repository;
    protected readonly ISession _session;

    /// <summary>
    ///    Whether the <c>obl</c> query parameter should be passed on to subsequent pages
    /// </summary>
    protected bool PropagateBackLinkOverride = true;

    protected DecisionBaseModel(IProjects repository, ISession session)
    {
        _repository = repository;
        _session = session;
    }

    public BackLinkModel BackLinkModel { get; set; }
    public string TrustName { get; set; }
    public int Urn { get; set; }
    public int Id { get; set; }

    protected object LinkParameters =>
      PropagateBackLinkOverride && Request.Query.ContainsKey("obl")
         ? new { Urn, obl = Request.Query["obl"] }
         : new { Urn };

   private async Task SetDefaults(int urn)
   {
        Urn = urn;
        var project = await _repository.GetByUrn($"{Urn}");
        TrustName = project.Result.IncomingTrustName;
        Id = project.Result.Id;
    }

   public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
   {
      if (context.RouteData.Values.ContainsKey("urn") &&
          int.TryParse(context.RouteData.Values["urn"] as string, out int urn))
      {
         await SetDefaults(urn);
      }

      await next();
   }

   protected void SetBackLinkModel(LinkItem linkItem, int linkRouteId)
   {
        BackLinkModel = new BackLinkModel { LinkPage = linkItem.PageName, LinkText = linkItem.BackText, LinkRouteId = linkRouteId };
   }

    /// <summary>
    ///    Returns the active <see cref="AdvisoryBoardDecision" /> from the current session or a new instance if one is not available
    /// </summary>
    /// <param name="id">The ID of the <see cref="AdvisoryBoardDecision" /> to retrieve.</param>
    /// <returns>Either the <see cref="AdvisoryBoardDecision" /> instance currently stored in the session, or a new instance.</returns>
    /// <remarks>
    ///    <p>If the session does not contain an instance of <see cref="AdvisoryBoardDecision" /> this call will create a new instance but will not store it in the session.</p>
    ///    <p>The ID parameter becomes part of the Session key with a prefix of <code>DECISION_SESSION_KEY_</code></p>
    /// </remarks>
    protected AdvisoryBoardDecision GetDecisionFromSession(int id)
    {
        return _session.Get<AdvisoryBoardDecision>($"{DECISION_SESSION_KEY}_{id}") ?? new AdvisoryBoardDecision();
    }

    /// <summary>
    ///    Stores the provided <see cref="AdvisoryBoardDecision" /> in the current session.
    /// </summary>
    /// <param name="id">The ID of the <see cref="AdvisoryBoardDecision" /> to store in the session</param>
    /// <param name="decision">An instance of <see cref="AdvisoryBoardDecision" /> to be persisted</param>
    protected void SetDecisionInSession(int id, AdvisoryBoardDecision decision)
    {
        _session.Set($"{DECISION_SESSION_KEY}_{id}", decision);
    }
}
