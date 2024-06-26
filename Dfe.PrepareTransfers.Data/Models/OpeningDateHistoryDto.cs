using System;

namespace Dfe.PrepareTransfers.Data.Models
{
    public class OpeningDateHistoryDto
    {
        public DateTime ChangedAt { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? OldDate { get; set; }
        public DateTime? NewDate { get; set; }
        public string ReasonForChange { get; set; }
        public string ReasonForChangeDetails { get; set; }
    }
}