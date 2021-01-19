using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Downstream.D365
{
    public class SearchProjectsD365PageModel
    {
        public List<SearchProjectsD365Model> Projects { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}
