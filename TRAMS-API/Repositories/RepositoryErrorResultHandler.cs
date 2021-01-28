using API.Models.Upstream.Response;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Repositories
{
    public class RepositoryErrorResultHandler : IRepositoryErrorResultHandler
    {
        public ActionResult LogAndCreateResponse(RepositoryResultBase repoResult)
        {
            ObjectResult response;

            if (repoResult.Error.StatusCode == HttpStatusCode.TooManyRequests)
            {
                response = new ObjectResult(ControllerErrorMessages.TooManyRequests)
                {
                    StatusCode = (int)HttpStatusCode.TooManyRequests
                };
            }
            else
            {
                response = new ObjectResult(ControllerErrorMessages.DownstreamServerError)
                {
                    StatusCode = (int)HttpStatusCode.BadGateway
                };
            }

            return response;
        }
    }
}