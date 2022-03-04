using System.Linq;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.TaskList.HtbDocument
{
    public class Download : CommonPageModel
    {
        private readonly ICreateProjectTemplate _createProjectTemplate;
        private readonly IGetInformationForProject _getInformationForProject;
        public Download(ICreateProjectTemplate createProjectTemplate,
            IGetInformationForProject getInformationForProject)
        {
            _createProjectTemplate = createProjectTemplate;
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
            var document = await _createProjectTemplate.Execute(Urn);
            return File(document.Document.ToArray(),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                $"ProjectTemplateFor{projectInformation.Project.IncomingTrustName.ToTitleCase().Replace(" ","")}–{projectInformation.Project.Reference}.docx");
        }
    }
}