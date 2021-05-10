using System.Collections.Generic;
using Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Views.Transfers
{
    public class TrustSearch : PageModel
    {
        public List<TrustSearchResult> Trusts;
    }
}