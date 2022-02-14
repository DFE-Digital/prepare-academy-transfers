﻿using System.Threading.Tasks;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.AcademyAndTrustInformation
{
    public class Index : CommonPageModel
    {
        public TransferAcademyAndTrustInformation.RecommendationResult Recommendation { get; set; }
        public string Author { get; set; }
        public string AdvisoryBoardDate { get; set; }
        public string TargetDate { get; set; }
        public string FirstDiscussedDate { get; set; }

        private readonly IGetInformationForProject _getInformationForProject;

        public Index(IGetInformationForProject getInformationForProject)
        {
            _getInformationForProject = getInformationForProject;
        }

        public async Task<IActionResult> OnGetAsync(string urn)
        {
            var projectInformation = await _getInformationForProject.Execute(urn);

            ProjectReference = projectInformation.Project.Reference;
            Recommendation = projectInformation.Project.AcademyAndTrustInformation.Recommendation;
            Author = projectInformation.Project.AcademyAndTrustInformation.Author;
            AdvisoryBoardDate = projectInformation.Project.Dates?.Htb;
            IncomingTrustName = projectInformation.Project.IncomingTrustName;
            TargetDate = projectInformation.Project.Dates?.Target;
            FirstDiscussedDate = projectInformation.Project.Dates?.FirstDiscussed;
            OutgoingAcademyUrn = projectInformation.Project.OutgoingAcademyUrn;
            Urn = projectInformation.Project.Urn;

            return Page();
        }
    }
}