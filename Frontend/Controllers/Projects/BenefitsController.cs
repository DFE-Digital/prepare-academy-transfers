using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
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

            var model = new BenefitsViewModel
            {
                Project = project.Result,
                ReturnToPreview = returnToPreview
            };

            return View(model);
        }

        [ActionName("IntendedBenefits")]
        [HttpPost("intended-benefits")]
        public async Task<IActionResult> IntendedBenefitsPost(string urn,
            TransferBenefits.IntendedBenefit[] intendedBenefits, string otherBenefit, bool returnToPreview = false)
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

            model.Project.Benefits.IntendedBenefits =
                new List<TransferBenefits.IntendedBenefit>(intendedBenefits);
            model.Project.Benefits.OtherIntendedBenefit = otherBenefit;

            if (model.Project.Benefits.IntendedBenefits.Count == 0)
            {
                var firstBenefit =
                    EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayableValues(TransferBenefits.IntendedBenefit
                        .Empty)[0];
                model.FormErrors.AddError(firstBenefit.ToString(), "intendedBenefits",
                    "Please select at least one intended benefit");
                return View(model);
            }

            if (model.Project.Benefits.IntendedBenefits.Contains(TransferBenefits.IntendedBenefit.Other) &&
                string.IsNullOrEmpty(otherBenefit))
            {
                model.FormErrors.AddError("otherBenefit", "otherBenefit", "Please enter the other benefit");
                return View(model);
            }

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
                    "Please specify the concern further");
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