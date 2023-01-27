using Microsoft.AspNetCore.Http;

namespace Dfe.PrepareTransfers.Web.Models;

public class LinkItem
{
   public string PageName { get; init; }
   public string BackText { get; init; }
   public string Urn { get; init; }

   public LinkItem For(string urn)
   {
      return new LinkItem { BackText = BackText, PageName = PageName, Urn = urn };
   }

   public LinkItem OverrideFrom(IQueryCollection query)
   {
      return new LinkItem
      {
         BackText = query.ContainsKey("bt") ? query["bt"] : BackText,
         PageName = query.ContainsKey("bl") ? query["bl"] : PageName,
         Urn = query.ContainsKey("u") ? query["u"] : Urn
      };
   }
}
