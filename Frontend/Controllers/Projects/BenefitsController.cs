using System.Collections.Generic;
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
        public async Task<IActionResult> IntendedBenefits(string urn)
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

        [ActionName("IntendedBenefits")]
        [HttpPost("intended-benefits")]
        public async Task<IActionResult> IntendedBenefitsPost(string urn,
            TransferBenefits.IntendedBenefit[] intendedBenefits, string otherBenefit)
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

            return RedirectToAction("Index", new {urn});
        }

        [HttpGet("other-factors")]
        public async Task<IActionResult> OtherFactors(string urn)
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

        [ActionName("OtherFactors")]
        [HttpPost("other-factors")]
        public async Task<IActionResult> OtherFactorsPost(string urn, List<TransferBenefits.OtherFactor> otherFactors,
            string highProfileDescription, string complexIssuesDescription, string financeAndDebtDescription)
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
            
            if (otherFactors.Count == 0)
            {
                var firstOtherFactor =
                    EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayableValues(TransferBenefits.OtherFactor
                        .Empty)[0];
                model.FormErrors.AddError(firstOtherFactor.ToString(), "otherFactors",
                    "Please select at least one other factor");
                return View(model);
            }

            var projectFactors = new Dictionary<TransferBenefits.OtherFactor, string>();

            if (otherFactors.Contains(TransferBenefits.OtherFactor.HighProfile))
            {
                projectFactors.Add(TransferBenefits.OtherFactor.HighProfile, highProfileDescription);
            }

            if (otherFactors.Contains(TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues))
            {
                projectFactors.Add(TransferBenefits.OtherFactor.ComplexLandAndBuildingIssues, complexIssuesDescription);
            }

            if (otherFactors.Contains(TransferBenefits.OtherFactor.FinanceAndDebtConcerns))
            {
                projectFactors.Add(TransferBenefits.OtherFactor.FinanceAndDebtConcerns, financeAndDebtDescription);
            }

            model.Project.Benefits.OtherFactors = projectFactors;

            var updateResult = await _projectsRepository.Update(model.Project);
            if (!updateResult.IsValid)
            {
                return View("ErrorPage", updateResult.Error.ErrorMessage);
            }

            return RedirectToAction("Index", new { urn });
        }
    }
}