using Data.Models;

namespace Frontend.Models
{
    public class ProjectViewModel
    {
        public Project Project { get; set; }
        public Academy TransferringAcademy { get; set; }
        public Trust OutgoingTrust { get; set; }
    }
}