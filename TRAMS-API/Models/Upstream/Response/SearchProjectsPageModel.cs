using System.Collections.Generic;

namespace API.Models.Upstream.Response
{
    public class SearchProjectsPageModel : BasePageModel
    {
        public List<SearchProjectsModel> Projects { get; set; }
    }
}