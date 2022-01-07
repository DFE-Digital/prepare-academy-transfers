using System.Net;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Helpers;

namespace Frontend.Controllers.Projects
{
    [Authorize]
    [Route("project/{urn}/features")]
    public class FeaturesController : Controller
    {
        private readonly IProjects _projectsRepository;

        public FeaturesController(IProjects projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        [Route("type")]
        [AcceptVerbs(WebRequestMethods.Http.Get)]
        public async Task<IActionResult> Type(string urn, bool returnToPreview = false)
        {
            var project = await _projectsRepository.GetByUrn(urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            var projectResult = project.Result;
            var vm = new FeaturesTypeViewModel
            {
                Urn = projectResult.Urn,
                OutgoingAcademyName = projectResult.OutgoingAcademyName,
                ReturnToPreview = returnToPreview,
                TypeOfTransfer = projectResult.Features.TypeOfTransfer,
                OtherType = projectResult.Features.OtherTypeOfTransfer
            };
            return View(vm);
        }

        [Route("type")]
        [ActionName("Type")]
        [AcceptVerbs(WebRequestMethods.Http.Post)]
        public async Task<IActionResult> TypePost(FeaturesTypeViewModel vm)
        {
            var urn = vm.Urn;
            var project = await _projectsRepository.GetByUrn(vm.Urn);
            if (!project.IsValid)
            {
                return View("ErrorPage", project.Error.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var projectResult = project.Result;
            projectResult.Features.TypeOfTransfer = vm.TypeOfTransfer;
            projectResult.Features.OtherTypeOfTransfer = vm.OtherType;

            var result = await _projectsRepository.Update(projectResult);
            if (!result.IsValid)
            {
                return View("ErrorPage", result.Error.ErrorMessage);
            }

            if (vm.ReturnToPreview)
            {
                return RedirectToPage(Links.HeadteacherBoard.Preview.PageName, new { id = urn });
            }

            return RedirectToPage("/Projects/Features/Index", new { urn });
        }
    }
}