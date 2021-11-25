using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.Academies;
using Frontend.Models;
using Frontend.Models.Forms;
using Frontend.Services.Interfaces;
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
        public async Task<IActionResult> Index(string id, bool addOrEditAdditionalInformation = false)
        {
            var projectInformation = await _getInformationForProject.Execute(id);

            if (!projectInformation.IsValid)
            {
                return View("ErrorPage", projectInformation.ResponseError.ErrorMessage);
            }
            

            var model = new GeneralInformationViewModel
            {
                AdditionalInformationModel = new AdditionalInformationViewModel
                {
                    AdditionalInformation = projectInformation.Project.GeneralInformationAdditionalInformation,
                    HintText = "This information will populate into your HTB template under the school performance (Ofsted information) section.",
                    Urn = projectInformation.Project.Urn,
                    AddOrEditAdditionalInformation = addOrEditAdditionalInformation
                },
                NameValues = FieldsToDisplay(projectInformation.OutgoingAcademy.GeneralInformation)
            };

            return View(model);
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
        
        
        private IEnumerable<FormFieldViewModel> FieldsToDisplay(GeneralInformation generalInformation)
        {
            
            return new List<FormFieldViewModel>
            {
                new FormFieldViewModel {Title = "School phase", Value = generalInformation.SchoolPhase},
                new FormFieldViewModel {Title = "Age range", Value = generalInformation.AgeRange},
                new FormFieldViewModel {Title = "Capacity", Value = generalInformation.Capacity},
                new FormFieldViewModel
                {
                    Title = "Number on roll (percentage the school is full)",
                    Value = $"{generalInformation.NumberOnRoll} ({generalInformation.PercentageFull})"
                },
                new FormFieldViewModel {Title = "Percentage of free school meals (%FSM)", Value = generalInformation.PercentageFsm},
                new FormFieldViewModel {Title = "Published admission number (PAN)", Value = generalInformation.Pan},
                new FormFieldViewModel {Title = "Private finance initiative (PFI) scheme", Value = generalInformation.Pfi},
                new FormFieldViewModel {Title = "Viability issues", Value = generalInformation.ViabilityIssue},
                new FormFieldViewModel {Title = "Financial deficit", Value = generalInformation.Deficit},
                new FormFieldViewModel {Title = "School type", Value = generalInformation.SchoolType},
                new FormFieldViewModel
                {
                    Title = "Percentage of good or outstanding academies in the diocesan trust", Value = generalInformation.DiocesesPercent
                },
                new FormFieldViewModel
                    {Title = "Distance from the academy to the trust headquarters", Value = generalInformation.DistanceToSponsorHq},
                new FormFieldViewModel {Title = "MP (Party)", Value = generalInformation.MpAndParty},
            };
        }
    }
}