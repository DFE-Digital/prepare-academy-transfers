using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects
{
    public class Index : CommonPageModel
    {
        private readonly ITaskListService _taskListService;
        public ProjectStatuses FeatureTransferStatus { get; set; }
        public ProjectStatuses TransferDatesStatus { get; set; }
        public ProjectStatuses BenefitsAndOtherFactorsStatus { get; set; }
        public ProjectStatuses RationaleStatus { get; set; }
        public ProjectStatuses AcademyAndTrustInformationStatus { get; set; }

        /// <summary>
        /// Item1 Academy Ukprn, Item2 Academy Name
        /// </summary>
        public List<Tuple<string,string>> Academies { get; set; }
        
        public Index(ITaskListService taskListService)
        {
            _taskListService = taskListService;
        }

        public IActionResult OnGet()
        {
            _taskListService.BuildTaskListStatuses(this);
            return Page();
        }
    }
}