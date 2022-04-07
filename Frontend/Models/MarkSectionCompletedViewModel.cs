using Microsoft.AspNetCore.Mvc;

namespace Frontend.Models
{
    public class MarkSectionCompletedViewModel
    {
        public bool ShowIsCompleted { get; set; }
        
        [BindProperty]
        public bool IsCompleted { get; set; }
    }
}