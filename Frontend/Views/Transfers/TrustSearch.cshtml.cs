using System.Collections.Generic;
using API.Models.Upstream.Response;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Views.Transfers
{
    public class TrustSearch : PageModel
    {
        public List<GetTrustsModel> Trusts;
    }
}