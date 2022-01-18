﻿using Data.Models;
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
        public ProjectStatuses BenefitsAndOtherFactorsStatus{ get; set; }
        public ProjectStatuses RationaleStatus{ get; set; }
        public ProjectStatuses AcademyAndTrustInformationStatus { get; set; }
        public bool HasKeyStage2PerformanceInformation { get; set; }  
        public bool HasKeyStage4PerformanceInformation { get; set; }
        public bool HasKeyStage5PerformanceInformation { get; set; }  
        
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