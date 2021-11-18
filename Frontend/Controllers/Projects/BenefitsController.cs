using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.Benefits;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{urn}/benefits")]
    public class BenefitsController : Controller
    {
        private readonly IProjects _projectsRepository;

        public BenefitsController(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IActionResult> Index(string urn)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }
            
            var projectResult = project.Result;
            var model = new BenefitsSummaryViewModel(
                projectResult.Benefits.IntendedBenefits.ToList(),
                projectResult.Benefits.OtherIntendedBenefit,
                BuildOtherFactorsItemViewModel(projectResult.Benefits.OtherFactors).Where(o => o.Checked).ToList(),
                projectResult.Urn,
                projectResult.OutgoingAcademyUrn
            );

            return View(model);
        }

        [HttpGet("intended-benefits")]
        public async Task<IActionResult> IntendedBenefits(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;
            var vm = new IntendedBenefitsViewModel
            {
               ProjectUrn = projectResult.Urn,
               OutgoingAcademyName = projectResult.OutgoingAcademyName,
               ReturnToPreview = returnToPreview,
               SelectedIntendedBenefits = projectResult.Benefits.IntendedBenefits,
               OtherBenefit = projectResult.Benefits.IntendedBenefits.Contains(TransferBenefits.IntendedBenefit.Other) ?
                projectResult.Benefits.OtherIntendedBenefit :
                null
            };

            return View(vm);
        }

        [ActionName("IntendedBenefits")]
        [HttpPost("intended-benefits")]
        public async Task<IActionResult> IntendedBenefitsPost(IntendedBenefitsViewModel vm)
        {
            var urn = vm.ProjectUrn;
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var projectResult = project.Result;
            projectResult.Benefits.IntendedBenefits =
                new List<TransferBenefits.IntendedBenefit>(vm.SelectedIntendedBenefits);
            projectResult.Benefits.OtherIntendedBenefit = vm.OtherBenefit;

            var updateResult = await _projectsRepository.Update(projectResult);
            if (!updateResult.IsValid)
            {
                return View("ErrorPage", updateResult.Error.ErrorMessage);
            }

            if (vm.ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = urn});
            }

            return RedirectToAction("Index", new {urn});
        }

        [HttpGet("other-factors")]
        public async Task<IActionResult> OtherFactors(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;
            var vm = new OtherFactorsViewModel()
            {
                ProjectUrn = projectResult.Urn,
                OutgoingAcademyName = projectResult.OutgoingAcademyName,
                ReturnToPreview = returnToPreview,
                OtherFactorsVm = BuildOtherFactorsItemViewModel(projectResult.Benefits.OtherFactors)
            };

            return View(vm);
        }

        [ActionName("OtherFactors")]
        [HttpPost("other-factors")]
        public async Task<IActionResult> OtherFactorsPost(OtherFactorsViewModel vm)
        {
            var urn = vm.ProjectUrn;
            var project = await _projectsRepository.GetByUrn(vm.ProjectUrn);

            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }
            
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            
            var projectResult = project.Result;
            projectResult.Benefits.OtherFactors = vm.OtherFactorsVm
                .Where(of => of.Checked)
                .ToDictionary(d => d.OtherFactor, x => x.Description);
            var updateResult = await _projectsRepository.Update(projectResult);
            if (!updateResult.IsValid)
            {
                return View("ErrorPage", updateResult.Error.ErrorMessage);
            }

            if (vm.ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = vm.ProjectUrn});
            }
            
            return RedirectToAction("Index", new {urn});
        }
        
        private static List<OtherFactorsItemViewModel> BuildOtherFactorsItemViewModel(Dictionary<TransferBenefits.OtherFactor, string> otherFactorsToSet)
        {
            List<OtherFactorsItemViewModel> items = new List<OtherFactorsItemViewModel>();
            foreach (TransferBenefits.OtherFactor otherFactor in Enum.GetValues(typeof(TransferBenefits.OtherFactor)))
            {
                if (otherFactor != TransferBenefits.OtherFactor.Empty)
                {
                    var isChecked = otherFactorsToSet.ContainsKey(otherFactor);

                    items.Add(new OtherFactorsItemViewModel()
                    {
                        OtherFactor = otherFactor,
                        Checked = isChecked,
                        Description = isChecked ? otherFactorsToSet[otherFactor] : string.Empty
                    });
                }
            }

            return items;
        }
    }
}