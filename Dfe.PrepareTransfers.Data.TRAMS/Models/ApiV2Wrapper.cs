using System.Collections.Generic;
using System.Linq;

namespace Dfe.PrepareTransfers.Data.TRAMS.Models
{
    public sealed class ApiV2Wrapper<T>
    {
        public T Data { get; set; }
        public ApiV2PagingInfo Paging { get; set; }
    }
}
