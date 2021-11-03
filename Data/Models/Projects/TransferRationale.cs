using System.ComponentModel.DataAnnotations;

namespace Data.Models.Projects
{
    public class TransferRationale
    {
        [Required(ErrorMessage = "WOOHOO THIS IS WRONG")]
        public string Project { get; set; }
        public string Trust { get; set; }
    }
}