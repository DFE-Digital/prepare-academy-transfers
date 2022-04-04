﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models.Projects;
using Frontend.Models;
using Frontend.Models.Benefits;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects.BenefitsAndRisks
{
    public class Index : CommonPageModel
    {
        private readonly IProjects _projects;
        public BenefitsSummaryViewModel BenefitsSummaryViewModel;

        public Index(IProjects projects)
        {
            _projects = projects;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var project = await _projects.GetByUrn(Urn);

            var projectResult = project.Result;
            ProjectReference = projectResult.Reference;
            BenefitsSummaryViewModel = new BenefitsSummaryViewModel(
                projectResult.Benefits.IntendedBenefits.ToList(),
                projectResult.Benefits.OtherIntendedBenefit,
                BuildOtherFactorsItemViewModel(projectResult.Benefits.OtherFactors).Where(o => o.Checked).ToList(),
                projectResult.Urn,
                projectResult.OutgoingAcademyUrn,
                projectResult.Benefits.AnyRisks
            );

            return Page();
        }

        public static List<OtherFactorsItemViewModel> BuildOtherFactorsItemViewModel(
            Dictionary<TransferBenefits.OtherFactor, string> otherFactorsToSet)
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