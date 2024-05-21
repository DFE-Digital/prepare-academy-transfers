using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Projects.TrustTemplate
{
    public class Index : CommonPageModel
    {

        private readonly IProjects _projects;


        public Index(IProjects projects)
        {
            _projects = projects;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            var projectResult = project.Result;

            Urn = projectResult.Urn;
            IncomingTrustName = projectResult.IncomingTrustName;


            return Page();
        }
    }
}