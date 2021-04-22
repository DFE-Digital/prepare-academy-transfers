using System.Collections.Generic;
using API.Models.Upstream.Response;
using Data;
using Data.Models;

namespace Frontend.Models
{
    public class PupilNumbersViewModel
    {
        public GetProjectsResponseModel Project { get; set; }
        public Academy OutgoingAcademy { get; set; }

    }
}