using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Frontend.BackgroundServices;
using Frontend.Models;
using Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Pages.Projects
{
    public class Index : CommonPageModel
    {
        private readonly ITaskListService _taskListService;
        private readonly PerformanceDataChannel _performanceDataChannel;
        public ProjectStatuses FeatureTransferStatus { get; set; }
        public ProjectStatuses TransferDatesStatus { get; set; }
        public ProjectStatuses BenefitsAndOtherFactorsStatus { get; set; }
        public ProjectStatuses RationaleStatus { get; set; }
        public ProjectStatuses AcademyAndTrustInformationStatus { get; set; }

        /// <summary>
        /// Item1 Academy Ukprn, Item2 Academy Name
        /// </summary>
        public List<Tuple<string, string>> Academies { get; set; }

        public Index(ITaskListService taskListService, PerformanceDataChannel performanceDataChannel)
        {
            _taskListService = taskListService;
            _performanceDataChannel = performanceDataChannel;
        }

        public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
        {
            await RetrievePerformanceData(cancellationToken);

            _taskListService.BuildTaskListStatuses(this);
            return Page();
        }

        private async Task RetrievePerformanceData(CancellationToken cancellationToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(30)); // wait max 30 seconds
            await _performanceDataChannel.AddProjectUrnAsync(Urn, cts.Token);
        }
    }
}