using System.Collections.Generic;
using API.Models.Upstream.Response;
using Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Views.Transfers
{
    public class OutgoingTrustAcademies : PageModel
    {
        public List<Academy> Academies;
    }
}