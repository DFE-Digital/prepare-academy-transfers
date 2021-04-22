using API.Models.Upstream.Response;
using Data.Models;

namespace Frontend.Services.Responses
{
    public class GetInformationForProjectResponse
    {
        public GetProjectsResponseModel Project { get; set; }
        public Academy OutgoingAcademy { get; set; }
    }
}