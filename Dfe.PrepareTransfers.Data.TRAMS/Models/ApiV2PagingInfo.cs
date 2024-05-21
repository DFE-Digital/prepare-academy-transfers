using System.Collections.Generic;
using System.Linq;

namespace Dfe.PrepareTransfers.Data.TRAMS.Models
{
    public class ApiV2PagingInfo
    {
        public int Page { get; set; }
        public int RecordCount { get; set; }
        public string? NextPageUrl { get; set; }
    }

}
