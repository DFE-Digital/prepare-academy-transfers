using System.Collections.Generic;
using API.Models.Upstream.Response;
using Data;
using Data.Models;

namespace Frontend.Models
{
    public class LatestOfstedJudgementViewModel
    {
        public Project Project { get; set; }
        public Academy Academy { get; set; }
    }
}