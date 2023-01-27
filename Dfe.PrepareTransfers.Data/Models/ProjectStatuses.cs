using System.ComponentModel.DataAnnotations;

namespace Dfe.PrepareTransfers.Data.Models
{
    public enum ProjectStatuses
    {
        Empty = 0,

        [Display(Name = "NOT STARTED")]
        NotStarted,

        [Display(Name = "IN PROGRESS")]
        InProgress,

        [Display(Name = "COMPLETED")]
        Completed
    }
}
