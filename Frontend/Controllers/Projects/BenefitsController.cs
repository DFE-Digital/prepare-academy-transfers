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

            var model = new BenefitsViewModel
            {
                Project = project.Result,
            };

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
               OtherBenefit = projectResult.Benefits.OtherIntendedBenefit
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

            var model = new BenefitsViewModel
            {
                Project = project.Result,
                ReturnToPreview = returnToPreview
            };

            BuildOtherFactorsViewModel(model);

            return View(model);
        }

        [ActionName("OtherFactors")]
        [HttpPost("other-factors")]
        public async Task<IActionResult> OtherFactorsPost(string urn, List<BenefitsViewModel.OtherFactorsViewModel> otherFactorsVm, bool returnToPreview)
        {
            var project = await _projectsRepository.GetByUrn(urn);

            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var model = new BenefitsViewModel
            {
                Project = project.Result,
            };

            var formHasErrors = false;
            foreach (var otherFactor in otherFactorsVm.Where(otherFactor =>
                otherFactor.Checked &&
                string.IsNullOrEmpty(otherFactor.Description)))
            {
                model.FormErrors.AddError(otherFactor.OtherFactor.ToString(), otherFactor.OtherFactor.ToString(),
                    "Specify the concern further");
                formHasErrors = true;
            }

            if (formHasErrors)
            {
                BuildOtherFactorsViewModel(model);
                return View(model);
            }

            model.Project.Benefits.OtherFactors = otherFactorsVm
                .Where(of => of.Checked)
                .ToDictionary(d => d.OtherFactor, x => x.Description);
            var updateResult = await _projectsRepository.Update(model.Project);
            if (!updateResult.IsValid)
            {
                return View("ErrorPage", updateResult.Error.ErrorMessage);
            }

            if (returnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new {id = urn});
            }
            
            return RedirectToAction("Index", new {urn});
        }
        
        private static void BuildOtherFactorsViewModel(BenefitsViewModel model)
        {
            var otherFactors = model.Project.Benefits.OtherFactors;
            model.OtherFactorsVm = new List<BenefitsViewModel.OtherFactorsViewModel>();
            foreach (TransferBenefits.OtherFactor otherFactor in Enum.GetValues(typeof(TransferBenefits.OtherFactor)))
            {
                if (otherFactor != TransferBenefits.OtherFactor.Empty)
                {
                    var isChecked = otherFactors.ContainsKey(otherFactor);

                    model.OtherFactorsVm.Add(new BenefitsViewModel.OtherFactorsViewModel
                    {
                        OtherFactor = otherFactor,
                        Checked = isChecked,
                        Description = isChecked ? otherFactors[otherFactor] : string.Empty
                    });
                }
            }
        }
    }
}