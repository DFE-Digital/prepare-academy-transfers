using System.ComponentModel.DataAnnotations;

namespace Dfe.PrepareTransfers.Data.Models.Projects
{
    public class TransferAcademyAndTrustInformation
    {
        public enum RecommendationResult
        {
            Empty = 0,
            
            [Display(Name = "Approve")]
            Approve,
            
            [Display(Name="Decline")]
            Decline
        }
        
        public RecommendationResult Recommendation { get; set; }
        public string Author { get; set; }
    }
    
}