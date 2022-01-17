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

        public Download(ICreateHtbDocument createHtbDocument,
            IGetInformationForProject getInformationForProject)
        {
            _createHtbDocument = createHtbDocument;
            _getInformationForProject = getInformationForProject;
        }
      
        public async Task<IActionResult> OnGetAsync()
        {
            var projectInformation = await _getInformationForProject.Execute(Urn);
           
            OutgoingAcademyName = projectInformation.OutgoingAcademy.Name;

            return Page();
        }
        
        public async Task<IActionResult> OnGetGenerateDocumentAsync()
        {
            var document = await _createHtbDocument.Execute(Urn);

            return File(document.Document.ToArray(),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                $"ProjectTemplateFor{Urn}_{System.DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentUICulture)}.docx");
        }
    }
}