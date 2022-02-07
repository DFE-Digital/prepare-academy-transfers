using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Frontend.ExtensionMethods;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.TaskList.HtbDocument
{
    public class Download : CommonPageModel
    {
        private readonly ICreateHtbDocument _createHtbDocument;
        private readonly IGetInformationForProject _getInformationForProject;
        public string IncomingTrustName { get; set; }
        public Download(ICreateHtbDocument createHtbDocument,
            IGetInformationForProject getInformationForProject)
        {
            _createHtbDocument = createHtbDocument;
            _getInformationForProject = getInformationForProject;
        }
      
        public async Task<IActionResult> OnGetAsync()
        {
            var projectInformation = await _getInformationForProject.Execute(Urn);
            ProjectReference = projectInformation.Project.Reference;
            IncomingTrustName = projectInformation.Project.IncomingTrustName.ToTitleCase();
            return Page();
        }
        
        public async Task<IActionResult> OnGetGenerateDocumentAsync()
        {
            var projectInformation = await _getInformationForProject.Execute(Urn);
            var document = await _createHtbDocument.Execute(Urn);
            return File(document.Document.ToArray(),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                $"ProjectTemplateFor{projectInformation.Project.IncomingTrustName.ToTitleCase().Replace(" ","-")}–{projectInformation.Project.Reference}.docx");
        }
    }
}