using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Data.Models;

public class GetProjectSearchModel
{
    public GetProjectSearchModel(int page, int count, string? titleFilter,
        IEnumerable<string>? deliveryOfficerQueryString,
        IEnumerable<string>? statusQueryString)
    {
        Page = page;
        Count = count;
        TitleFilter = titleFilter;
        DeliveryOfficerQueryString = deliveryOfficerQueryString;
        StatusQueryString = statusQueryString;
    }

    public int Page { get; set; }
    public int Count { get; set; }
    public string? TitleFilter { get; set; }
    public IEnumerable<string>? DeliveryOfficerQueryString { get; set; }
    public IEnumerable<string>? StatusQueryString { get; set; }
}
