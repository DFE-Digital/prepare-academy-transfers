using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Models
{
    public class MarkSectionCompletedViewModel
    {
        public bool ShowIsCompleted { get; set; }
        
        [BindProperty]
        public bool IsCompleted { get; set; }
    }
}