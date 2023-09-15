using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Web.BackgroundServices;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Projects
{
    public class Index : CommonPageModel
    {
        private readonly ITaskListService _taskListService;
        private readonly PerformanceDataChannel _performanceDataChannel;
        public ProjectStatuses FeatureTransferStatus { get; set; }
        public ProjectStatuses TransferDatesStatus { get; set; }
        public ProjectStatuses BenefitsAndOtherFactorsStatus { get; set; }
        public ProjectStatuses LegalRequirementsStatus { get; set; }
        public ProjectStatuses RationaleStatus { get; set; }
        public ProjectStatuses AcademyAndTrustInformationStatus { get; set; }
        public User AssignedUser { get; set; }

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
            _taskListService.BuildTaskListStatuses(this);
            //await RetrievePerformanceData(cancellationToken);
            return Page();
        }

        private async Task RetrievePerformanceData(CancellationToken cancellationToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(90)); // wait max 30 seconds

            foreach (var academyUkprnAndUrn in Academies)
            {
                await _performanceDataChannel.AddAcademyAsync(academyUkprnAndUrn.Item1, cts.Token);
            }
         
        }
    }
}