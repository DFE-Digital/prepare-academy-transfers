using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.Academies;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
using Frontend.Services.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{id}/general-information")]
    public class GeneralInformationController : Controller
    {
        private readonly IGetInformationForProject _getInformationForProject;
        private readonly IProjects _projectsRepository;

        public GeneralInformationController(IGetInformationForProject getInformationForProject, IProjects projectsRepository)
        {
            _getInformationForProject = getInformationForProject;
            _projectsRepository = projectsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            var projectInformation = await _getInformationForProject.Execute(id);

            if (!projectInformation.IsValid)
            {
                return View("ErrorPage", projectInformation.ResponseError.ErrorMessage);
            }

            FieldsToDisplay(out GeneralInformationViewModel vm,projectInformation);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string id, string additionalInformation)
        {
            var model = await _projectsRepository.GetByUrn(id);

            model.Result.GeneralInformationAdditionalInformation = additionalInformation;
            await _projectsRepository.Update(model.Result);

            return RedirectToAction(nameof(this.Index), 
                "GeneralInformation", 
                new { id }, 
                "additional-information-hint");
        }
        
        private void FieldsToDisplay(out GeneralInformationViewModel vm, GetInformationForProjectResponse projectResponse)
        {
            var generalInformation = projectResponse.OutgoingAcademy.GeneralInformation;
            vm = new GeneralInformationViewModel
            {
                AdditionalInformationModel = new AdditionalInformationViewModel
                {
                    AdditionalInformation = projectResponse.Project.GeneralInformationAdditionalInformation,
                    HintText = "This information will populate into your HTB template under the school performance (Ofsted information) section.",
                    Urn = projectResponse.Project.Urn
                },
                SchoolPhase = generalInformation.SchoolPhase,
                AgeRange = generalInformation.AgeRange,
                Capacity = generalInformation.Capacity,
                NumberOnRoll = $"{generalInformation.NumberOnRoll} ({generalInformation.PercentageFull})",
                FreeSchoolMeals = generalInformation.PercentageFsm,
                PublishedAdmissionNumber = generalInformation.Pan,
                PrivateFinanceInitiative = generalInformation.Pfi,
                ViabilityIssues = generalInformation.ViabilityIssue,
                FinancialDeficit = generalInformation.Deficit,
                SchoolType = generalInformation.SchoolType,
                DiocesePercent = generalInformation.DiocesesPercent,
                DistanceFromAcademyToTrustHq = generalInformation.DistanceToSponsorHq,
                MP = generalInformation.MpAndParty
            };
        }
    }
}