using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.ExtensionMethods;
using Frontend.Models;
using Frontend.Models.Benefits;
using Frontend.Models.Forms;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages.Projects.Benefits
{
    public class IntendedBenefits : CommonPageModel
    {
        private readonly IProjects _projects;

        [BindProperty]
        public IntendedBenefitsViewModel IntendedBenefitsViewModel { get; set; } = new IntendedBenefitsViewModel();
        
        public IList<CheckboxViewModel> Checkboxes { get; set; }
        
        public IntendedBenefits(IProjects projects)
        {
            _projects = projects;
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            if (!project.IsValid)
            {
                return this.View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;

            Urn = projectResult.Urn;
            OutgoingAcademyName = projectResult.OutgoingAcademyName;
            ReturnToPreview = ReturnToPreview;
            IntendedBenefitsViewModel.SelectedIntendedBenefits = projectResult.Benefits.IntendedBenefits;
            IntendedBenefitsViewModel.OtherBenefit = projectResult.Benefits.IntendedBenefits.Contains(TransferBenefits.IntendedBenefit.Other)
                    ? projectResult.Benefits.OtherIntendedBenefit
                    : null;
            Checkboxes = GetIntendedBenefitsCheckboxes();
            return Page();
        }
        
        private IList<CheckboxViewModel> GetIntendedBenefitsCheckboxes()
        {
            IList<CheckboxViewModel> items = new List<CheckboxViewModel>();
            foreach (var intendedBenefit in EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayableValues(TransferBenefits.IntendedBenefit.Empty))
            {
                items.Add(new CheckboxViewModel
                {
                    DisplayName = EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayValue(intendedBenefit),
                    Name = $"{nameof(IntendedBenefitsViewModel)}.{nameof(IntendedBenefitsViewModel.SelectedIntendedBenefits)}",
                    Value = intendedBenefit.ToString(),
                    Checked = IntendedBenefitsViewModel.SelectedIntendedBenefits.Contains(intendedBenefit)
                });
            }

            return items;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var project = await _projects.GetByUrn(Urn);
            if (!project.IsValid)
            {
                return this.View("ErrorPage", project.Error.ErrorMessage);
            }
            
            if (!ModelState.IsValid)
            {
                Checkboxes = GetIntendedBenefitsCheckboxes();
                return Page();
            }
            
            var projectResult = project.Result;
            projectResult.Benefits.IntendedBenefits =
                new List<TransferBenefits.IntendedBenefit>(IntendedBenefitsViewModel.SelectedIntendedBenefits);
            projectResult.Benefits.OtherIntendedBenefit = IntendedBenefitsViewModel.OtherBenefit;
            
            var updateResult = await _projects.Update(projectResult);
            if (!updateResult.IsValid)
            {
                return this.View("ErrorPage", updateResult.Error.ErrorMessage);
            }
            
            if (ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = Urn});
            }
            
            return RedirectToPage("/Projects/Benefits/Index", new {Urn});
        }
    }
}