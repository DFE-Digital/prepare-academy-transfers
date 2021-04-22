using Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Repositories.Interfaces
{
    public interface IRepositoryErrorResultHandler
    {
        public ActionResult LogAndCreateResponse(RepositoryResultBase repoResult);
    }
}
