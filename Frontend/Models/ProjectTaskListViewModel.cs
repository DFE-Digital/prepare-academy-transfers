using Data.Models;

namespace Frontend.Models
{
    public class ProjectTaskListViewModel 
    {
        public ProjectStatuses FeatureTransferStatus { get; set; }
        public ProjectStatuses TransferDatesStatus { get; set; }
        public ProjectStatuses BenefitsAndOtherFactorsStatus{ get; set; }
        public ProjectStatuses RationaleStatus{ get; set; }
        public ProjectStatuses AcademyAndTrustInformationStatus { get; set; }
        public bool HasKeyStage2PerformanceInformation { get; set; }  
        public bool HasKeyStage4PerformanceInformation { get; set; }
        public bool HasKeyStage5PerformanceInformation { get; set; }  
        
    }
}