namespace Dfe.PrepareTransfers.Web.Models;

public class AutoCompleteSearchModel
{
   public AutoCompleteSearchModel(string label, string searchQuery, string searchEndpoint)
   {
      Label = label;
      SearchQuery = searchQuery?.Replace("'", "\\'");
      SearchEndpoint = searchEndpoint;
   }

   public string Label { get; set; }

   public string SearchQuery { get; set; }

   public string SearchEndpoint { get; set; }
}
