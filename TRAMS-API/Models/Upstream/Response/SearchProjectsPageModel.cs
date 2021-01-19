using System.Collections.Generic;

namespace API.Models.Upstream.Response
{
    public class SearchProjectsPageModel
    {
        public List<SearchProjectsModel> Projects { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}