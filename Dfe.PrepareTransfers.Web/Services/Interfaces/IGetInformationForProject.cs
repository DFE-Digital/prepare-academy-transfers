using System.Threading.Tasks;
using Dfe.PrepareTransfers.Web.Services.Responses;

namespace Dfe.PrepareTransfers.Web.Services.Interfaces
{
    public interface IGetInformationForProject
    {
        public Task<GetInformationForProjectResponse> Execute(string projectUrn);
    }
}