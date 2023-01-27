using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data;
using Dfe.PrepareTransfers.Web.Dfe.PrepareTransfers.Helpers;
using Dfe.PrepareTransfers.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.PrepareTransfers.Web.Pages.TaskList
{
    public class SchoolData : CommonPageModel
    {
        private readonly IAcademies _academies;
        private readonly IProjects _projects;
        private readonly IEducationPerformance _projectRepositoryEducationPerformance;

        [BindProperty(SupportsGet = true)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string AcademyUkprn {get; set; }
        public bool HasKeyStage2PerformanceInformation { get; set; }
        public bool HasKeyStage4PerformanceInformation { get; set; }
        public bool HasKeyStage5PerformanceInformation { get; set; }
        public string AcademyName { get; set; }
        
        
        public SchoolData(IAcademies academies, IProjects projects, IEducationPerformance projectRepositoryEducationPerformance)
        {
            _academies = academies;
            _projects = projects;
            _projectRepositoryEducationPerformance = projectRepositoryEducationPerformance;
        }
        
        public async Task<PageResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn); 
            var academy = await _academies.GetAcademyByUkprn(AcademyUkprn);
            AcademyName = academy.Result.Name;
            ProjectReference = project.Result.Reference;
            var educationPerformance =
                _projectRepositoryEducationPerformance.GetByAcademyUrn(project.Result.OutgoingAcademyUrn).Result;
            HasKeyStage2PerformanceInformation =
                PerformanceDataHelpers.HasKeyStage2PerformanceInformation(educationPerformance.Result
                    .KeyStage2Performance);
            HasKeyStage4PerformanceInformation =
                PerformanceDataHelpers.HasKeyStage4PerformanceInformation(educationPerformance.Result
                    .KeyStage4Performance);
            HasKeyStage5PerformanceInformation =
                PerformanceDataHelpers.HasKeyStage5PerformanceInformation(educationPerformance.Result
                    .KeyStage5Performance);
            return Page();
        }
    }
}