using System;
using System.Threading.Tasks;
using Frontend.Services.Responses;

namespace Frontend.Services.Interfaces
{
    public interface IGetInformationForProject
    {
        public Task<GetInformationForProjectResponse> Execute(string projectUrn);
    }
}