using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Data.Models;
using DocumentFormat.OpenXml.Bibliography;
using Frontend.Helpers;
using Frontend.Models;
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
            var model = await GetModel(urn);
            return View(model);
        }

        private async Task<BenefitsViewModel> GetModel(string urn)
        {
            var project = await _projectsRepository.GetByUrn(urn);

            var model = new BenefitsViewModel
            {
                Project = project.Result
            };

            return model;
        }

        [HttpGet("intended-benefits")]
        public async Task<IActionResult> IntendedBenefits(string urn)
        {
            var model = await GetModel(urn);
            return View(model);
        }

        [ActionName("IntendedBenefits")]
        [HttpPost("intended-benefits")]
        public async Task<IActionResult> IntendedBenefitsPost(string urn,
            TransferBenefits.IntendedBenefit[] intendedBenefits, string otherBenefit)
        {
            var model = await GetModel(urn);
            model.Project.TransferBenefits.IntendedBenefits =
                new List<TransferBenefits.IntendedBenefit>(intendedBenefits);
            model.Project.TransferBenefits.OtherIntendedBenefit = otherBenefit;

            if (model.Project.TransferBenefits.IntendedBenefits.Count == 0)
            {
                var firstBenefit =
                    EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayableValues(TransferBenefits.IntendedBenefit
                        .Empty)[0];
                model.FormErrors.AddError(firstBenefit.ToString(), "intendedBenefits",
                    "Please select at least one intended benefit");
                return View(model);
            }

            if (model.Project.TransferBenefits.IntendedBenefits.Contains(TransferBenefits.IntendedBenefit.Other) &&
                string.IsNullOrEmpty(otherBenefit))
            {
                model.FormErrors.AddError("otherBenefit", "otherBenefit", "Please enter the other benefit");
                return View(model);
            }

            await _projectsRepository.Update(model.Project);

            return RedirectToAction("Index", new {urn});
        }
    }
}